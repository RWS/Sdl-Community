using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters
{
    // Binds a group of RadioButtons to one enum property: ConverterParameter is the button's value.
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Equals(value, parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is true ? parameter : Binding.DoNothing;
    }
}
