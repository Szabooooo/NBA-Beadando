using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NBADatabase.Infrastructure
{
    // Binds a number field that was left empty as 0 instead of failing with "The value '' is invalid."
    public class EmptyAsZeroModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
            var text = value.FirstValue;

            try
            {
                var number = string.IsNullOrWhiteSpace(text)
                    ? Convert.ChangeType(0, bindingContext.ModelType)
                    : Convert.ChangeType(text, bindingContext.ModelType, value.Culture);
                bindingContext.Result = ModelBindingResult.Success(number);
            }
            catch (Exception e) when (e is FormatException or OverflowException)
            {
                var name = bindingContext.ModelMetadata.GetDisplayName();
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"The value '{text}' is not valid for {name}.");
            }

            return Task.CompletedTask;
        }
    }
}
