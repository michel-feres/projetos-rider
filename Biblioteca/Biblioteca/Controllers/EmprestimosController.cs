using Biblioteca.Data;
using Biblioteca.Filters;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers;

[RequireLogin]
public class EmprestimosController : Controller
{
    private const int ItensPorPagina = 5;
    private const decimal ValorMultaPorDia = 2m;
    private readonly BibliotecaContext _context;

    public EmprestimosController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? usuarioId, string? status, int pagina = 1)
    {
        var emprestimosQuery = _context.Emprestimos
            .Include(emprestimo => emprestimo.Usuario)
            .Include(emprestimo => emprestimo.Livro)
            .AsQueryable();

        if (usuarioId.HasValue)
        {
            emprestimosQuery = emprestimosQuery.Where(emprestimo => emprestimo.UsuarioId == usuarioId.Value);
        }

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        emprestimosQuery = status switch
        {
            "Emprestado" => emprestimosQuery.Where(emprestimo => emprestimo.DataDevolucao == null && emprestimo.DataPrevistaDevolucao >= hoje),
            "Atrasado" => emprestimosQuery.Where(emprestimo => emprestimo.DataDevolucao == null && emprestimo.DataPrevistaDevolucao < hoje),
            "Devolvido" => emprestimosQuery.Where(emprestimo => emprestimo.DataDevolucao != null),
            _ => emprestimosQuery
        };

        var totalItens = await emprestimosQuery.CountAsync();
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(totalItens / (double)ItensPorPagina));
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var emprestimos = await emprestimosQuery
            .OrderByDescending(emprestimo => emprestimo.DataEmprestimo)
            .Skip((pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();

        await CarregarUsuariosFiltro(usuarioId);
        ViewBag.UsuarioId = usuarioId;
        ViewBag.StatusFiltro = status;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.ActionName = ViewBag.TituloPagina == "Histórico por usuário" ? nameof(Historico) : nameof(Index);

        return View(emprestimos);
    }

    public async Task<IActionResult> Historico(int? usuarioId, int pagina = 1)
    {
        ViewBag.TituloPagina = "Histórico por usuário";
        return await Index(usuarioId, null, pagina);
    }

    public async Task<IActionResult> Create()
    {
        await CarregarListas();
        return View(new Emprestimo());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Emprestimo emprestimo)
    {
        await ValidarEmprestimo(emprestimo);

        if (!ModelState.IsValid)
        {
            await CarregarListas(emprestimo.UsuarioId, emprestimo.LivroId);
            return View(emprestimo);
        }

        var livro = await _context.Livros.FindAsync(emprestimo.LivroId);
        if (livro is null)
        {
            return NotFound();
        }

        livro.QuantidadeDisponivel--;
        _context.Add(emprestimo);
        await _context.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Empréstimo registrado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var emprestimo = await _context.Emprestimos.FindAsync(id);
        if (emprestimo is null)
        {
            return NotFound();
        }

        await CarregarListas(emprestimo.UsuarioId, emprestimo.LivroId);
        return View(emprestimo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Emprestimo emprestimo)
    {
        if (id != emprestimo.Id)
        {
            return NotFound();
        }

        await ValidarDatas(emprestimo);

        if (!ModelState.IsValid)
        {
            await CarregarListas(emprestimo.UsuarioId, emprestimo.LivroId);
            return View(emprestimo);
        }

        var emprestimoAtual = await _context.Emprestimos.FindAsync(id);
        if (emprestimoAtual is null)
        {
            return NotFound();
        }

        emprestimoAtual.DataEmprestimo = emprestimo.DataEmprestimo;
        emprestimoAtual.DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao;

        await _context.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Empréstimo atualizado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Devolver(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(item => item.Usuario)
            .Include(item => item.Livro)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (emprestimo is null)
        {
            return NotFound();
        }

        return View(emprestimo);
    }

    [HttpPost, ActionName("Devolver")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarDevolucao(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(item => item.Livro)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (emprestimo is null)
        {
            return NotFound();
        }

        if (!emprestimo.DataDevolucao.HasValue)
        {
            emprestimo.DataDevolucao = DateOnly.FromDateTime(DateTime.Today);
            emprestimo.Multa = CalcularMulta(emprestimo.DataPrevistaDevolucao, emprestimo.DataDevolucao.Value);
            if (emprestimo.Livro is not null)
            {
                emprestimo.Livro.QuantidadeDisponivel++;
            }

            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Devolução registrada com sucesso.";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var emprestimo = await _context.Emprestimos
            .Include(item => item.Usuario)
            .Include(item => item.Livro)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (emprestimo is null)
        {
            return NotFound();
        }

        return View(emprestimo);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(item => item.Livro)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (emprestimo is not null)
        {
            if (!emprestimo.DataDevolucao.HasValue && emprestimo.Livro is not null)
            {
                emprestimo.Livro.QuantidadeDisponivel++;
            }

            _context.Emprestimos.Remove(emprestimo);
            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Empréstimo excluído com sucesso.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarListas(int? usuarioId = null, int? livroId = null)
    {
        var usuarios = await _context.Usuarios
            .Where(usuario => usuario.Ativo)
            .OrderBy(usuario => usuario.NomeCompleto)
            .ToListAsync();

        var livros = await _context.Livros
            .Where(livro => livro.QuantidadeDisponivel > 0 || livro.Id == livroId)
            .OrderBy(livro => livro.Titulo)
            .ToListAsync();

        ViewBag.Usuarios = new SelectList(usuarios, "Id", "NomeCompleto", usuarioId);
        ViewBag.Livros = new SelectList(livros, "Id", "Titulo", livroId);
    }

    private async Task ValidarEmprestimo(Emprestimo emprestimo)
    {
        await ValidarDatas(emprestimo);

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.Id == emprestimo.UsuarioId && usuario.Ativo);
        if (usuario is null)
        {
            ModelState.AddModelError(nameof(Emprestimo.UsuarioId), "Selecione um usuário ativo.");
        }

        var livro = await _context.Livros.FindAsync(emprestimo.LivroId);
        if (livro is null)
        {
            ModelState.AddModelError(nameof(Emprestimo.LivroId), "Selecione um livro.");
        }
        else if (livro.QuantidadeDisponivel <= 0)
        {
            ModelState.AddModelError(nameof(Emprestimo.LivroId), "Este livro não possui exemplares disponíveis.");
        }
        else if (livro.FaixaEtariaPermitida >= 18 && usuario is not null && usuario.Idade < 18)
        {
            ModelState.AddModelError(nameof(Emprestimo.LivroId), "Livro 18+ só pode ser emprestado para usuário maior de idade.");
        }
    }

    private Task ValidarDatas(Emprestimo emprestimo)
    {
        if (emprestimo.DataPrevistaDevolucao < emprestimo.DataEmprestimo)
        {
            ModelState.AddModelError(nameof(Emprestimo.DataPrevistaDevolucao), "A data prevista deve ser posterior ou igual à data do empréstimo.");
        }

        if (emprestimo.DataDevolucao.HasValue && emprestimo.DataDevolucao.Value < emprestimo.DataEmprestimo)
        {
            ModelState.AddModelError(nameof(Emprestimo.DataDevolucao), "A data de devolução não pode ser anterior ao empréstimo.");
        }

        return Task.CompletedTask;
    }

    private async Task CarregarUsuariosFiltro(int? usuarioId = null)
    {
        var usuarios = await _context.Usuarios
            .OrderBy(usuario => usuario.NomeCompleto)
            .ToListAsync();

        ViewBag.UsuariosFiltro = new SelectList(usuarios, "Id", "NomeCompleto", usuarioId);
    }

    private static decimal CalcularMulta(DateOnly dataPrevista, DateOnly dataDevolucao)
    {
        var diasAtraso = dataDevolucao > dataPrevista ? dataDevolucao.DayNumber - dataPrevista.DayNumber : 0;
        return diasAtraso * ValorMultaPorDia;
    }
}
