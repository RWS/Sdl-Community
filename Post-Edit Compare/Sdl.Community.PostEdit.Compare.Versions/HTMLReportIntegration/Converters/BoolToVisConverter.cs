using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters
{
    public class BoolToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (parameter?.ToString() == "Invert")
                flag = !flag;
            return flag ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (Visibility)value == Visibility.Visible;
            if (parameter?.ToString() == "Invert")
                flag = !flag;
            return flag;
        }
    }

}