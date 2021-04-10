using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace LHSBrackets.ModelBinder
{
    public record FilterOperation<T>(
        FilterModifier Type,
        T Operand);

    [TypeConverter(typeof(FiltersTypeConverter))]
    public class Filters<T>
    {
        public T? EQ { get; private set; }
        public T? NE { get; private set; }
        public T? GT { get; private set; }
        public T? GTE { get; private set; }
        public T? LT { get; private set; }
        public T? LTE { get; private set; }
        public List<T> IN { get; private set; } = new List<T>();
        public List<T> NIN { get; private set; } = new List<T>();

        public Filters(params FilterOperation<T>[] operations)
        {
            // foreach(var operation in operations) {
            //     if (operation.Type == FilterModifier.Eq)
            //         EQ = operation.Operand;
            //     else if (operation.Type == FilterModifier.Ne)
            //         NE = operation.Operand;
            //     else if (operation.Type == FilterModifier.Gt)
            //         GT = operation.Operand;
            //     else if (operation.Type == FilterModifier.Gte)
            //         GTE = operation.Operand;
            //     else if (operation.Type == FilterModifier.Lt)
            //         LT = operation.Operand;
            //     else if (operation.Type == FilterModifier.Lte)
            //         LTE = operation.Operand;
            //     else if (operation.Type == FilterModifier.In)
            //     {
            //         //TODO: what if the individual values themselves contain a comma - that'll not work.
            //         var items = value.Split(",");
            //         IN.AddRange(items.Select(x => (T)ConvertValue(x.Trim(' '))));
            //     }
            //     else if (operation.Type == FilterModifier.Nin)
            //     {
            //         var items = value.Split(",");
            //         NIN.AddRange(items.Select(x => (T)ConvertValue(x.Trim(' '))));
            //     }
            // }
        }
    }
}