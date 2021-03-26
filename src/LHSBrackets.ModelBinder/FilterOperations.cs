using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LHSBrackets.ModelBinder
{
    public class FilterOperations<T> where T : struct
    {
        public Nullable<T> EQ { get; set; }
        public Nullable<T> NE { get; set; }
        public Nullable<T> GT { get; set; }
        public Nullable<T> GTE { get; set; }
        public Nullable<T> LT { get; set; }
        public Nullable<T> LTE { get; set; }
        public List<T> IN { get; set; } = new List<T>();
        public List<T> NIN { get; set; } = new List<T>();

        internal void SetValue(FilterOperationEnum operation, string value)
        {
            if (operation == FilterOperationEnum.Eq)
                EQ = (T)ConvertValue(value);
            else if (operation == FilterOperationEnum.Ne)
                NE = (T)ConvertValue(value);
            else if (operation == FilterOperationEnum.Gt)
                GT = (T)ConvertValue(value);
            else if (operation == FilterOperationEnum.Gte)
                GTE = (T)ConvertValue(value);
            else if (operation == FilterOperationEnum.Lt)
                LT = (T)ConvertValue(value);
            else if (operation == FilterOperationEnum.Lte)
                LTE = (T)ConvertValue(value);
            else if (operation == FilterOperationEnum.In)
            {
                var items = value.Split(",");
                IN.AddRange(items.Select(x => (T)ConvertValue(x.Trim(' '))));
            }
            else if (operation == FilterOperationEnum.Nin)
            {
                var items = value.Split(",");
                NIN.AddRange(items.Select(x => (T)ConvertValue(x.Trim(' '))));
            }
        }

        private object ConvertValue(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            object convertedValue;
            try
            {
                convertedValue = converter.ConvertFromString(value);
            }
            catch (NotSupportedException)
            {
                throw;
                // do stuff
            }

            return convertedValue;
        }
    }
}