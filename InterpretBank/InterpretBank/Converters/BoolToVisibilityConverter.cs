using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InterpretBank.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Binding.DoNothing;
            var result = bool.Parse(parameter.ToString()) ? bool.Parse(value.ToString()) : !bool.Parse(value.ToString());
            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}