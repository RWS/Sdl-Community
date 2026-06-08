using LanguageWeaverProvider.Converters;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace LanguageWeaverProvider.SubscriptionJourney.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = value is not null && (bool)value;
            return parameter.ToVisibility(isTrue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
