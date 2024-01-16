using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InterpretBank.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringNullOrEmpty = !string.IsNullOrWhiteSpace((string)value);
            var visibility = (Visibility)Enum.Parse(typeof(Visibility), parameter.ToString());
            return stringNullOrEmpty ? Invert(visibility) : visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Visibility Invert(Visibility visibility) =>
                    visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }
}