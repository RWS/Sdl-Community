using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InterpretBank.TermbaseViewer.UI.Converters
{
    public class LanguageToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var brushConverter = new BrushConverter();
            return values[0]?.ToString() == values[1]?.ToString() ? brushConverter.ConvertFrom("#88dcba")
                : values[0]?.ToString() == values[2]?.ToString() ? brushConverter.ConvertFrom("#acc2eb")
                : brushConverter.ConvertFrom("#cfd8dc");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}