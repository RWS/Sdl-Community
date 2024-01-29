using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InterpretBank.Converters
{
    public class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum.TryParse(value.ToString(), true, out Visibility visibility);
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}