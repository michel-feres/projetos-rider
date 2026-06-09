using Biblioteca.Data;
using Biblioteca.Filters;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers;

[RequireLogin]
public class LivrosController : Controller
{
    private const int ItensPorPagina = 5;
    private readonly BibliotecaContext _context;

    public LivrosController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? busca, string? categoria, bool somenteDisponiveis = false, int pagina = 1)
    {
        var livrosQuery = _context.Livros.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            livrosQuery = livrosQuery.Where(livro =>
                livro.Titulo.ToLower().Contains(termo) ||
                livro.Autor.ToLower().Contains(termo) ||
                (livro.Isbn != null && livro.Isbn.ToLower().Contains(termo)));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            livrosQuery = livrosQuery.Where(livro => livro.Categoria == categoria);
        }

        if (somenteDisponiveis)
        {
            livrosQuery = livrosQuery.Where(livro => livro.QuantidadeDisponivel > 0);
        }

        var totalItens = await livrosQuery.CountAsync();
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(totalItens / (double)ItensPorPagina));
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var livros = await livrosQuery
            .OrderBy(livro => livro.Titulo)
            .Skip((pagina - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();

        ViewBag.Busca = busca;
        ViewBag.Categoria = categoria;
        ViewBag.SomenteDisponiveis = somenteDisponiveis;
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = totalPaginas;
        ViewBag.ActionName = ViewBag.TituloPagina == "Livros disponíveis" ? nameof(Disponiveis) : nameof(Index);
        ViewBag.Categorias = await _context.Livros
            .Select(livro => livro.Categoria)
            .Distinct()
            .OrderBy(item => item)
            .ToListAsync();

        return View(livros);
    }

    public Task<IActionResult> Disponiveis(string? busca, string? categoria, int pagina = 1)
    {
        ViewBag.TituloPagina = "Livros disponíveis";
        return Index(busca, categoria, true, pagina);
    }

    public IActionResult Create()
    {
        return View(new Livro());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Livro livro)
    {
        NormalizarLivro(livro);
        await ValidarLivro(livro);

        if (!ModelState.IsValid)
        {
            return View(livro);
        }

        _context.Add(livro);
        await _context.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Livro cadastrado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var livro = await _context.Livros.FindAsync(id);
        if (livro is null)
        {
            return NotFound();
        }

        return View(livro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Livro livro)
    {
        if (id != livro.Id)
        {
            return NotFound();
        }

        NormalizarLivro(livro);
        await ValidarLivro(livro, livro.Id);

        if (!ModelState.IsValid)
        {
            return View(livro);
        }

        try
        {
            _context.Update(livro);
            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Livro atualizado com sucesso.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await LivroExists(livro.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var livro = await _context.Livros
            .FirstOrDefaultAsync(livro => livro.Id == id);

        if (livro is null)
        {
            return NotFound();
        }

        ViewBag.PossuiEmprestimoAtivo = await _context.Emprestimos
            .AnyAsync(emprestimo => emprestimo.LivroId == livro.Id && emprestimo.DataDevolucao == null);

        return View(livro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro is not null)
        {
            var possuiEmprestimoAtivo = await _context.Emprestimos
                .AnyAsync(emprestimo => emprestimo.LivroId == livro.Id && emprestimo.DataDevolucao == null);

            if (possuiEmprestimoAtivo)
            {
                TempData["MensagemErro"] = "Não é possível excluir um livro com empréstimo ativo.";
                return RedirectToAction(nameof(Index));
            }

            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Livro excluído com sucesso.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static void NormalizarLivro(Livro livro)
    {
        livro.Titulo = livro.Titulo.Trim();
        livro.Autor = livro.Autor.Trim();
        livro.Isbn = string.IsNullOrWhiteSpace(livro.Isbn) ? null : livro.Isbn.Trim();
        livro.Categoria = livro.Categoria.Trim();
    }

    private async Task ValidarLivro(Livro livro, int? livroId = null)
    {
        if (livro.QuantidadeDisponivel > livro.QuantidadeTotal)
        {
            ModelState.AddModelError(nameof(Livro.QuantidadeDisponivel), "A quantidade disponível não pode ser maior que a total.");
        }

        if (!string.IsNullOrWhiteSpace(livro.Isbn))
        {
            var isbnJaExiste = await _context.Livros.AnyAsync(item =>
                item.Isbn == livro.Isbn && (!livroId.HasValue || item.Id != livroId.Value));

            if (isbnJaExiste)
            {
                ModelState.AddModelError(nameof(Livro.Isbn), "Já existe um livro cadastrado com este ISBN.");
            }
        }
    }

    private async Task<bool> LivroExists(int id)
    {
        return await _context.Livros.AnyAsync(livro => livro.Id == id);
    }
}
