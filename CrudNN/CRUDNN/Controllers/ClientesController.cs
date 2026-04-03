using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRUDNN.Data;
using CRUDNN.Models;
namespace CRUDNN.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // helper de mensagens
        private void Success(string m) => TempData["Success"] = m;
        private void Error(string m) => TempData["Error"] = m;

        // GET: Clientes
        public async Task<IActionResult> Index(string? q)
        {
            ViewData["Title"] = "Clientes";
            ViewData["q"] = q;

            var query = _context.Clientes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                // Postgres: ILIKE faz case-insensitive
                query = query.Where(c =>
                    EF.Functions.ILike(c.Nome, $"%{q}%") ||
                    EF.Functions.ILike(c.Email ?? "", $"%{q}%") ||
                    EF.Functions.ILike(c.Telefone ?? "", $"%{q}%"));
            }

            var list = await query.OrderBy(c => c.Nome).ToListAsync();
            return View(list);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();
            ViewData["Title"] = "Detalhes do Cliente";
            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Novo Cliente";
            return View(new Cliente());
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            _context.Add(cliente);
            await _context.SaveChangesAsync();
            Success("Cliente cadastrado com sucesso!");
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();
            ViewData["Title"] = "Editar Cliente";
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();
            if (!ModelState.IsValid) return View(cliente);

            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                Success("Cliente atualizado com sucesso!");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clientes.Any(e => e.Id == cliente.Id)) return NotFound();
                Error("Não foi possível salvar as alterações.");
                throw;
            }
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null) return NotFound();
            ViewData["Title"] = "Excluir Cliente";
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                Success("Cliente excluído com sucesso!");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
