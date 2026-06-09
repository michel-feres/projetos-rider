using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Filters;

public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var usuarioId = context.HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId is null)
        {
            context.Result = new RedirectToActionResult("Login", "Usuarios", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
