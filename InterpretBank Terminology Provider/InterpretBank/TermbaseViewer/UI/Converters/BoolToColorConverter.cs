using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InterpretBank.TermbaseViewer.UI.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var toColor = parameter is not null ? Brushes.LightGray : new BrushConverter().ConvertFrom("#198a8b");
            return (bool)value ? toColor : Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}