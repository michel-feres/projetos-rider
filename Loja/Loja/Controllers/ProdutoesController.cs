// Controller responsável pelas operações CRUD de Produtos.
//
// Comentários didáticos em português foram adicionados para facilitar a compreensão
// do fluxo: listagem, detalhes, criação, edição e exclusão de produtos.
// Explicações destacam o uso do Entity Framework para acesso a dados, o padrão
// de actions do MVC e como a View recebe dados auxiliares (por exemplo a lista
// de fornecedores para popular um combobox na view de Create/Edit).
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrudProdutoFornecedor.Data;
using CrudProdutoFornecedor.Models;

namespace Loja.Controllers
{
    public class ProdutoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Injeção de dependência do DbContext (ApplicationDbContext) para
        // acesso ao banco de dados via Entity Framework Core. O framework
        // cria uma instância e a fornece ao controller.
        public ProdutoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Produtoes
        // Lista todos os produtos. Aqui usamos eager loading (Include) para
        // carregar também a entidade Fornecedor relacionada, assim as views
        // podem exibir o nome do fornecedor sem precisar de consultas adicionais.
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Produtos.Include(p => p.Fornecedor);
            // ToListAsync executa a query de forma assíncrona e retorna a lista
            // de produtos para a view.
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Produtoes/Details/5
        // Exibe os detalhes de um produto específico. O id é opcional (int?).
        // Se não for fornecido ou o produto não existir, retornamos NotFound().
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Novamente, usamos Include para trazer o Fornecedor relacionado
            // e poder mostrar informações completas na view de detalhes.
            var produto = await _context.Produtos
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtoes/Create
        // Exibe o formulário de criação de produto. Para popular o combobox
        // de fornecedores usamos ViewData["FornecedorId"] com um SelectList;
        // a view irá ler essa ViewData e montar o <select>.
        public IActionResult Create()
        {
            // valueField = "FornecedorId", textField = "Nome"
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome");
            return View();
        }

        // POST: Produtoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Produtoes/Create
        // Recebe os dados do formulário para criar um produto. O atributo
        // [ValidateAntiForgeryToken] protege contra CSRF. O [Bind(...)] limita
        // quais propriedades serão vinculadas (boa prática contra overposting).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProdutoId,Nome,Valor,FornecedorId")] Produto produto)
        {
            // Verifica se os dados validados no model estão corretos
            if (ModelState.IsValid)
            {
                // Adiciona e salva de forma assíncrona
                _context.Add(produto);
                await _context.SaveChangesAsync();
                // Redireciona para a lista após sucesso
                return RedirectToAction(nameof(Index));
            }
            // Se houver erro de validação, recarregamos a lista de fornecedores
            // para que o combobox continue disponível na view retornada.
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", produto.FornecedorId);
            return View(produto);
        }

        // GET: Produtoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", produto.FornecedorId);
            return View(produto);
        }

        // POST: Produtoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Produtoes/Edit/5
        // Recebe os dados do formulário de edição. Verifica se o id da rota
        // coincide com o id do produto enviado e aplica as validações do ModelState.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProdutoId,Nome,Valor,FornecedorId")] Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza a entidade no contexto e salva as alterações
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Tratamento de concorrência: checa se o produto ainda existe
                    if (!ProdutoExists(produto.ProdutoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Se não for um caso tratado, rethrow para ser logado/diagnosticado
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Se houver erro, recarregamos a lista de fornecedores para a view
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedores, "FornecedorId", "Nome", produto.FornecedorId);
            return View(produto);
        }

        // GET: Produtoes/Delete/5
        // Exibe a confirmação de exclusão para o produto informado.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtoes/Delete/5
        // A action DeleteConfirmed é invocada após o usuário confirmar a
        // exclusão. Removemos a entidade e salvamos as alterações.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Verifica se um produto existe no banco de dados (usado no tratamento
        // de concorrência durante a edição).
        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.ProdutoId == id);
        }
    }
}
