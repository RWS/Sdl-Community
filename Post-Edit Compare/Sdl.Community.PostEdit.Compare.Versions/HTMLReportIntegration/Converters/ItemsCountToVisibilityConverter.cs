using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters
{
    public class ItemsCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int count) return Binding.DoNothing;
            // Check if the value is a collection (e.g., List<T>)
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No need to implement ConvertBack
            throw new NotImplementedException();
        }
    }

}