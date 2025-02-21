using System;
using System.Globalization;
using System.Windows.Data;

namespace InterpretBank.SettingsService.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is null, return false; otherwise, return true.
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is typically used for read-only scenarios, so ConvertBack is not implemented.
            throw new NotImplementedException();
        }
    }
}