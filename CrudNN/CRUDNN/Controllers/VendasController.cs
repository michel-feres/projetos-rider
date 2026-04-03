using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRUDNN.Data;
using CRUDNN.Models;
using CRUDNN.ViewModels;
namespace CRUDNN.Controllers;
public class VendasController : Controller
{
    private readonly AppDbContext _context;

    public VendasController(AppDbContext context)
    {
        _context = context;
    }
    // GET: Vendas
    public async Task<IActionResult> Index()
    {
        var vendas = await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens).ThenInclude(i => i.Produto)
            .AsNoTracking()
            .OrderByDescending(v => v.Data)
            .ToListAsync();

        return View(vendas);
    }
    // GET: Vendas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var venda = await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venda == null) return NotFound();

        return View(venda);
    }
    // GET: Vendas/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Clientes = await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync();
        var produtos = await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
        var vm = new VendaFormVM
        {
            Itens = produtos.Select(p => new VendaItemVM
            {
                ProdutoId = p.Id,
                ProdutoNome = p.Nome,
                PrecoAtual = p.Preco,
                Quantidade = 0
            }).ToList()
        };
        return View(vm);
    }
    // POST: Vendas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendaFormVM vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            return View(vm);
        }
        // Carrega preços atuais dos produtos selecionados
        var idsSelecionados = vm.Itens.Where(i => i.Quantidade > 0).Select(i => i.ProdutoId).ToList();
        var produtos = await _context.Produtos
            .Where(p => idsSelecionados.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
        if (!idsSelecionados.Any())
        {
            ModelState.AddModelError("", "Selecione pelo menos um produto com quantidade > 0.");
            ViewBag.Clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            return View(vm);
        }
        var venda = new Venda
        {
            ClienteId = vm.ClienteId,
            Data = DateTime.Now
        };
        foreach (var item in vm.Itens.Where(i => i.Quantidade > 0))
        {
            var produto = produtos[item.ProdutoId];
            venda.Itens.Add(new VendaProduto
            {
                ProdutoId = produto.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco // captura preço na venda
            });
            // (Opcional) baixa de estoque:
            // produto.Estoque -= item.Quantidade;
        }
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Vendas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var venda = await _context.Vendas
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (venda == null) return NotFound();
        ViewBag.Clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        var produtos = await _context.Produtos.AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
        var vm = new VendaFormVM
        {
            ClienteId = venda.ClienteId,
            Itens = produtos.Select(p =>
            {
                var existente = venda.Itens.FirstOrDefault(i => i.ProdutoId == p.Id);
                return new VendaItemVM
                {
                    ProdutoId = p.Id,
                    ProdutoNome = p.Nome,
                    PrecoAtual = p.Preco,
                    Quantidade = existente?.Quantidade ?? 0
                };
            }).ToList()
        };
        ViewBag.VendaId = venda.Id;
        return View(vm);
    }

    // POST: Vendas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VendaFormVM vm)
    {
        var venda = await _context.Vendas
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (venda == null) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.Clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
            ViewBag.VendaId = id;
            return View(vm);
        }
        venda.ClienteId = vm.ClienteId;
        // Remove todos os itens e recria (estratégia simples e segura)
        _context.VendasProdutos.RemoveRange(venda.Itens);
        venda.Itens.Clear();
        var idsSelecionados = vm.Itens.Where(i => i.Quantidade > 0).Select(i => i.ProdutoId).ToList();
        var produtos = await _context.Produtos
            .Where(p => idsSelecionados.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
        foreach (var item in vm.Itens.Where(i => i.Quantidade > 0))
        {
            var produto = produtos[item.ProdutoId];
            venda.Itens.Add(new VendaProduto
            {
                VendaId = venda.Id,
                ProdutoId = produto.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco
            });
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // GET: Vendas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var venda = await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (venda == null) return NotFound();

        return View(venda);
    }
    // POST: Vendas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venda = await _context.Vendas
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venda != null)
        {
            _context.VendasProdutos.RemoveRange(venda.Itens);
            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
