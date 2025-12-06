using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.ModelBinding;

/// <summary>
/// Binder provider for optional IFormFile fields.
/// </summary>
public class OptionalFormFileModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var modelType = context.Metadata.ModelType;
        
        // Check for IFormFile or IFormFile?
        if (modelType == typeof(IFormFile) || Nullable.GetUnderlyingType(modelType) == typeof(IFormFile))
            return new OptionalFormFileModelBinder();

        return null;
    }
}

