using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.ModelBinding;

/// <summary>
/// Simple model binder for IFormFile that allows null/empty values without validation errors.
/// </summary>
public class OptionalFormFileModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(IFormFile))
            return Task.CompletedTask;

        var file = bindingContext.HttpContext.Request.Form.Files
            .FirstOrDefault(f => f.Name.Equals(bindingContext.ModelName, StringComparison.OrdinalIgnoreCase));

        // If file exists and has content, bind it; otherwise bind null
        var result = (file?.Length > 0) ? file : null;
        bindingContext.Result = ModelBindingResult.Success(result);

        return Task.CompletedTask;
    }
}

