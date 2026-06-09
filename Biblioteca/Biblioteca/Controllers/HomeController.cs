using System.Diagnostics;
using Biblioteca.Data;
using Biblioteca.Filters;
using Microsoft.AspNetCore.Mvc;
using Biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers;

public class HomeController : Controller
{
    private readonly BibliotecaContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(BibliotecaContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [RequireLogin]
    public async Task<IActionResult> Index()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
        ViewBag.TotalLivros = await _context.Livros.CountAsync();
        ViewBag.LivrosDisponiveis = await _context.Livros.CountAsync(livro => livro.QuantidadeDisponivel > 0);
        ViewBag.EmprestimosAtivos = await _context.Emprestimos.CountAsync(emprestimo => emprestimo.DataDevolucao == null);
        ViewBag.EmprestimosAtrasados = await _context.Emprestimos.CountAsync(emprestimo =>
            emprestimo.DataDevolucao == null && emprestimo.DataPrevistaDevolucao < hoje);

        return View();
    }

    [RequireLogin]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
