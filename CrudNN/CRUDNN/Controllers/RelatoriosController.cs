using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRUDNN.Data;
using CRUDNN.ViewModels;

namespace VendasMvc.Controllers;

public class RelatoriosController : Controller
{
    private readonly AppDbContext _ctx;

    public RelatoriosController(AppDbContext ctx) => _ctx = ctx;

    // GET: /Relatorios/VendasPorProduto?dataIni=2025-10-01&dataFim=2025-10-31
    public async Task<IActionResult> VendasPorProduto(DateTime? dataIni, DateTime? dataFim)
    {
        // Se você salva em UTC (recomendado), converta os filtros locais para UTC:
        DateTime? iniUtc = dataIni?.Date.ToUniversalTime();               // 00:00 local -> UTC
        DateTime? fimUtc = dataFim?.Date.AddDays(1).AddTicks(-1).ToUniversalTime(); // 23:59:59 -> UTC

        var vendas = _ctx.Vendas
            .Include(v => v.Itens).ThenInclude(i => i.Produto)
            .AsNoTracking()
            .AsQueryable();

        if (iniUtc.HasValue) vendas = vendas.Where(v => v.Data >= iniUtc.Value);
        if (fimUtc.HasValue) vendas = vendas.Where(v => v.Data <= fimUtc.Value);

        var linhas = await vendas
            .SelectMany(v => v.Itens)
            .GroupBy(i => new { i.ProdutoId, Nome = i.Produto!.Nome })
            .Select(g => new ProdutoVendaReportRow
            {
                ProdutoId = g.Key.ProdutoId,
                Produto   = g.Key.Nome,
                Quantidade= g.Sum(x => x.Quantidade),
                Receita   = g.Sum(x => x.PrecoUnitario * x.Quantidade)
            })
            .OrderByDescending(r => r.Receita)
            .ToListAsync();

        var vm = new RelatorioVendasVM
        {
            DataIni = dataIni?.Date,
            DataFim = dataFim?.Date,
            Linhas = linhas
        };

        ViewData["Title"] = "Relatório | Vendas por Produto";
        return View(vm);
    }

    // Exportar CSV com o mesmo filtro
    public async Task<IActionResult> VendasPorProdutoCsv(DateTime? dataIni, DateTime? dataFim)
    {
        DateTime? iniUtc = dataIni?.Date.ToUniversalTime();
        DateTime? fimUtc = dataFim?.Date.AddDays(1).AddTicks(-1).ToUniversalTime();

        var vendas = _ctx.Vendas
            .Include(v => v.Itens).ThenInclude(i => i.Produto)
            .AsNoTracking()
            .AsQueryable();

        if (iniUtc.HasValue) vendas = vendas.Where(v => v.Data >= iniUtc.Value);
        if (fimUtc.HasValue) vendas = vendas.Where(v => v.Data <= fimUtc.Value);

        var linhas = await vendas
            .SelectMany(v => v.Itens)
            .GroupBy(i => new { i.ProdutoId, Nome = i.Produto!.Nome })
            .Select(g => new
            {
                Produto   = g.Key.Nome,
                Quantidade= g.Sum(x => x.Quantidade),
                Receita   = g.Sum(x => x.PrecoUnitario * x.Quantidade)
            })
            .OrderByDescending(r => r.Receita)
            .ToListAsync();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Produto;Quantidade;Receita");
        foreach (var r in linhas)
            sb.AppendLine($"{r.Produto};{r.Quantidade};{r.Receita:0.00}");
        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"vendas_por_produto_{DateTime.Now:yyyyMMddHHmm}.csv");
    }
}
