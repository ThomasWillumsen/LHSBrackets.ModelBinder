using System;
using System.Collections.Generic;
using System.Linq;

namespace LHSBrackets.ModelBinder
{
    public abstract class FilterRequest
    {
        public abstract IEnumerable<(string PropertyName, Action<string> BindValue)> GetPropertyBinders();

        protected IEnumerable<(string PropertyName, Action<string> Bind)> GetPropertyBinder<T>(
            FilterOperations<T> filterWrapper,
            string dictName)
        {
            var t = typeof(T);
            if (t != typeof(string)
                && (t.IsGenericType == false || t.GetGenericTypeDefinition() != typeof(Nullable<>)))
                throw new ArgumentException("generic parameter T must be of type string or a nullable struct type");

            var operations = Enum.GetValues(typeof(FilterOperationEnum)).Cast<FilterOperationEnum>();
            List<(string PropertyName, Action<string> Bind)> binders = operations
                .Select(operation =>
                {
                    Action<string> bindValue = (string value) => filterWrapper.SetValue(operation, value);
                    return (BuildBinderPropertyName(dictName, operation), bindValue);
                }).ToList();

            // standard query param without operation i.e categoryId=2
            binders.Add((
                BuildBinderPropertyName(dictName),
                (string value) => filterWrapper.SetValue(FilterOperationEnum.Eq, value)
            ));

            return binders;
        }

        private string BuildBinderPropertyName(string propertyName, FilterOperationEnum operation) => $"{propertyName}[{operation.ToString()}]".ToLower();
        private string BuildBinderPropertyName(string propertyName) => $"{propertyName}".ToLower();
    }
}