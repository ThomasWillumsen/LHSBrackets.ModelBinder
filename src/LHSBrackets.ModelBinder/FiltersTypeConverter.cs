using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LHSBrackets.ModelBinder
{
    public class FiltersTypeConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, 
            CultureInfo culture, 
            object value)
        {
            var stringValue = (string)value;
            
            var operandSplit = stringValue.Split('=');

            var leftOperand = operandSplit[0];
            var rightOperand = operandSplit[1];

            var modifierSplit = leftOperand.Split('[', ']');
            var modifier = Enum.Parse<FilterModifier>(modifierSplit[1]);

            // if (modifier == FilterModifier.Eq)
            //     EQ = operation.Operand;
            // else if (modifier == FilterModifier.Ne)
            //     NE = operation.Operand;
            // else if (modifier == FilterModifier.Gt)
            //     GT = operation.Operand;
            // else if (modifier == FilterModifier.Gte)
            //     GTE = operation.Operand;
            // else if (modifier == FilterModifier.Lt)
            //     LT = operation.Operand;
            // else if (modifier == FilterModifier.Lte)
            //     LTE = operation.Operand;
            // else if (modifier == FilterModifier.In)
            // {
            //     //TODO: what if the individual values themselves contain a comma - that'll not work.
            //     var items = value.Split(",");
            //     IN.AddRange(items.Select(x => (T)ConvertValue(x.Trim(' '))));
            // }
            // else if (operation.Type == FilterModifier.Nin)
            // {
            //     var items = value.Split(",");
            //     NIN.AddRange(items.Select(x => (T)ConvertValue(x.Trim(' '))));
            // }

            throw new Exception();
        }
    }
}