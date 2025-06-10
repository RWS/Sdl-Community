using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters
{
    public class FormalityToFriendlyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Formality formality)
                return value?.ToString() ?? string.Empty;

            return formality switch
            {
                Formality.Default => "Default",
                Formality.Less => "Less Formal",
                Formality.More => "More Formal",
                Formality.Prefer_Less => "Prefer Less Formal",
                Formality.Prefer_More => "Prefer More Formal",
                _ => formality.ToString()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}