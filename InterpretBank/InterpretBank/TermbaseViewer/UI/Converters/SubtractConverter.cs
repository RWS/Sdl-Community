using System;
using System.Globalization;
using System.Windows.Data;

namespace InterpretBank.TermbaseViewer.UI.Converters
{
    public class SubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()) || string.IsNullOrWhiteSpace(parameter.ToString()))
                return Binding.DoNothing;

            var result = double.Parse(value.ToString()) - double.Parse(parameter.ToString());
            return result > 0 ? result : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}