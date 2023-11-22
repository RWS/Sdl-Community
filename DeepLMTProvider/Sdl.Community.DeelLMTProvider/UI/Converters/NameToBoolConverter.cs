using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters
{
    public class NameToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.Contains(".csv");
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}