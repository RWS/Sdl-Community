using System;
using System.Globalization;
using System.Windows.Data;
namespace InterpretBank.Converters
{
    public class StringEmptyToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !string.IsNullOrWhiteSpace((string)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}