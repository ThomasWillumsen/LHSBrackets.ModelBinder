using System;
using System.Collections.Generic;
using System.Reflection;
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

            var requestModel = Activator.CreateInstance(bindingContext.ModelType);
            if (requestModel is FilterRequest filterRequest == false)
                throw new ArgumentException($"The modeltype {requestModel?.GetType()} does not inherit from {typeof(FilterRequest)}");

            var properties = bindingContext.ModelType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.PropertyType.GetGenericTypeDefinition() != typeof(FilterOperations<>))
                    continue;

                EnsurePrimitiveType(prop);

                foreach (var operation in Enum.GetValues<FilterOperationEnum>())
                {
                    var valueProviderResult = bindingContext.ValueProvider.GetValue($"{prop.Name}[{operation.ToString()}]".ToLower()); // ex: "author[eq]"
                    if (valueProviderResult.Length > 0)
                        SetValueOnProperty(requestModel, prop, operation, (string)valueProviderResult.Values);
                }

                // support regular query param without operation ex: categoryId=2
                var valueProviderResult2 = bindingContext.ValueProvider.GetValue($"{prop.Name}".ToLower()); // ex: "author[eq]"
                if (valueProviderResult2.Length > 0)
                    SetValueOnProperty(requestModel, prop, FilterOperationEnum.Eq, (string)valueProviderResult2.Values);
            }

            bindingContext.Result = ModelBindingResult.Success(requestModel);
            return Task.CompletedTask;
        }

        private static void EnsurePrimitiveType(PropertyInfo prop)
        {
            // TODO: validate for primitive types, string, datetime and stuff
            return;
        }

        private static void SetValueOnProperty(object? requestModel, PropertyInfo prop, FilterOperationEnum operation, string value)
        {
            var propertyObject = prop.GetValue(requestModel, null);
            if(propertyObject == null) // property not instantiated
            {
                propertyObject = Activator.CreateInstance(prop.PropertyType);
                prop.SetValue(requestModel, propertyObject);
            }

            var method = prop.PropertyType.GetMethod(nameof(FilterOperations<object>.SetValue), BindingFlags.Instance | BindingFlags.NonPublic);
            method!.Invoke(propertyObject, new object[] { operation, value });
        }
    }
}