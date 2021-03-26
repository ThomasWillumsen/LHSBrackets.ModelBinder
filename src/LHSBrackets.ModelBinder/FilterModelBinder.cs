using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LHSBrackets.ModelBinder
{
    public class FilterModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var resultModel = Activator.CreateInstance(bindingContext.ModelType);
            if (resultModel is FilterRequest == false)
                throw new ArgumentException($"The modeltype {resultModel?.GetType()} does not inherit from {typeof(FilterRequest)}");

            var binders = ((FilterRequest)resultModel).GetBinders();
            foreach (var binder in binders)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(binder.PropertyName);
                if (valueProviderResult.Length == 0)
                    continue;

                binder.BindValue.Invoke(valueProviderResult.Values);
            }

            bindingContext.Result = ModelBindingResult.Success(resultModel);
            return Task.CompletedTask;
        }


    }
}