using InterpretBank.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace InterpretBank.TermbaseViewer.UI.Converters
{
    public class OrBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValues = values.Select(v => v as bool?);

            var orResult = boolValues.Where(boolValue => boolValue != null)
                .Aggregate(false, (current, boolValue) => current | (boolValue.Value));

            return orResult ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
