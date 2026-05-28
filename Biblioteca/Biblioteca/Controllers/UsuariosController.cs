using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers;

/// <summary>
/// Controlador responsável pelo CRUD de usuários e pelo login simples.
/// </summary>
public class UsuariosController : Controller
{
    private const string UsuarioIdSessionKey = "UsuarioId";
    private const string UsuarioNomeSessionKey = "UsuarioNome";
    private readonly BibliotecaContext _context;

    public UsuariosController(BibliotecaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios
            .OrderBy(usuario => usuario.NomeCompleto)
            .ToListAsync();

        return View(usuarios);
    }

    public IActionResult Create()
    {
        return View(new Usuario { DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Usuario usuario)
    {
        usuario.Email = usuario.Email.Trim();
        await ValidarEmailDuplicado(usuario.Email);

        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        _context.Add(usuario);
        await _context.SaveChangesAsync();
        TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Usuario usuario)
    {
        if (id != usuario.Id)
        {
            return NotFound();
        }

        usuario.Email = usuario.Email.Trim();
        await ValidarEmailDuplicado(usuario.Email, usuario.Id);

        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        try
        {
            _context.Update(usuario);
            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Usuário atualizado com sucesso.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UsuarioExists(usuario.Id))
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

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Id == id);

        if (usuario is null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is not null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            TempData["MensagemSucesso"] = "Usuário excluído com sucesso.";
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        var email = login.Email.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email.ToLower() == email && usuario.Senha == login.Senha);

        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(login);
        }

        if (!usuario.Ativo)
        {
            ModelState.AddModelError(string.Empty, "Usuário inativo. Procure a administração da biblioteca.");
            return View(login);
        }

        HttpContext.Session.SetInt32(UsuarioIdSessionKey, usuario.Id);
        HttpContext.Session.SetString(UsuarioNomeSessionKey, usuario.NomeCompleto);
        TempData["MensagemSucesso"] = $"Bem-vindo(a), {usuario.NomeCompleto}!";

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["MensagemSucesso"] = "Logout realizado com sucesso.";

        return RedirectToAction(nameof(Login));
    }

    private async Task ValidarEmailDuplicado(string email, int? usuarioId = null)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        var emailJaExiste = await _context.Usuarios.AnyAsync(usuario =>
            usuario.Email.ToLower() == emailNormalizado && (!usuarioId.HasValue || usuario.Id != usuarioId.Value));

        if (emailJaExiste)
        {
            ModelState.AddModelError(nameof(Usuario.Email), "Já existe um usuário cadastrado com este e-mail.");
        }
    }

    private async Task<bool> UsuarioExists(int id)
    {
        return await _context.Usuarios.AnyAsync(usuario => usuario.Id == id);
    }
}