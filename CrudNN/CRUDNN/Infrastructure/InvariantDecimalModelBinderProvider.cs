using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace CRUDNN.Infrastructure;

public class InvariantDecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var t = context.Metadata.UnderlyingOrModelType;
        if (t == typeof(decimal) || t == typeof(decimal?))
            return new InvariantDecimalModelBinder();
        return null;
    }
}