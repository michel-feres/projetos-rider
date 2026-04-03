using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
namespace CRUDNN.Infrastructure;

public class InvariantDecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext ctx)
    {
        var valueResult = ctx.ValueProvider.GetValue(ctx.ModelName);
        if (valueResult == ValueProviderResult.None) return Task.CompletedTask;
        ctx.ModelState.SetModelValue(ctx.ModelName, valueResult);
        var raw = valueResult.FirstValue?.Trim();
        if (string.IsNullOrEmpty(raw))
            return Task.CompletedTask;
        // Regra robusta: escolhe o último separador como decimal e remove o outro como milhar
        int lastComma = raw.LastIndexOf(',');
        int lastDot   = raw.LastIndexOf('.');
        if (lastComma > lastDot)
            raw = raw.Replace(".", "");           // ponto = milhar
        else if (lastDot > lastComma)
            raw = raw.Replace(",", "");           // vírgula = milhar
        raw = raw.Replace(',', '.');              // decimal final = ponto
        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var dec))
        {
            ctx.Result = ModelBindingResult.Success(dec);
        }
        else
        {
            ctx.ModelState.TryAddModelError(ctx.ModelName, "Valor numérico inválido.");
        }
        return Task.CompletedTask;
    }
}