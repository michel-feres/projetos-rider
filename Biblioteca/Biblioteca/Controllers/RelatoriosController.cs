using Biblioteca.Data;
using Biblioteca.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers;

[RequireLogin]
public class RelatoriosController : Controller
{
    private readonly BibliotecaContext _context;

    public RelatoriosController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);

        ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
        ViewBag.TotalLivros = await _context.Livros.CountAsync();
        ViewBag.TotalExemplares = await _context.Livros.SumAsync(livro => (int?)livro.QuantidadeTotal) ?? 0;
        ViewBag.TotalDisponiveis = await _context.Livros.SumAsync(livro => (int?)livro.QuantidadeDisponivel) ?? 0;
        ViewBag.EmprestimosAtivos = await _context.Emprestimos.CountAsync(emprestimo => emprestimo.DataDevolucao == null);
        ViewBag.EmprestimosAtrasados = await _context.Emprestimos.CountAsync(emprestimo =>
            emprestimo.DataDevolucao == null && emprestimo.DataPrevistaDevolucao < hoje);
        ViewBag.TotalMultasRecebidas = await _context.Emprestimos.SumAsync(emprestimo => (decimal?)emprestimo.Multa) ?? 0m;

        var livrosPorCategoria = await _context.Livros
            .GroupBy(livro => livro.Categoria)
            .Select(grupo => new { Categoria = grupo.Key, Total = grupo.Count() })
            .OrderByDescending(item => item.Total)
            .ToListAsync();

        ViewBag.LivrosPorCategoria = livrosPorCategoria
            .Select(item => new KeyValuePair<string, int>(item.Categoria, item.Total))
            .ToList();

        ViewBag.EmprestimosRecentes = await _context.Emprestimos
            .Include(emprestimo => emprestimo.Usuario)
            .Include(emprestimo => emprestimo.Livro)
            .OrderByDescending(emprestimo => emprestimo.DataEmprestimo)
            .Take(5)
            .ToListAsync();

        return View();
    }
}
