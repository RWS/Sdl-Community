using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters;

public class ExpandArrowConverter : IValueConverter
{
    private const string DownArrow = "M 0 0 L 10 0 L 5 7 Z";
    private const string UpArrow = "M 0 7 L 10 7 L 5 0 Z";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? UpArrow : DownArrow;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}