using System;
using System.Globalization;
using System.Windows.Data;

namespace InterpretBank.TermbaseViewer.UI.Converters
{
    public class SubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value.ToString()) || string.IsNullOrWhiteSpace(parameter.ToString())
                ? Binding.DoNothing
                : double.Parse(value.ToString()) - double.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}