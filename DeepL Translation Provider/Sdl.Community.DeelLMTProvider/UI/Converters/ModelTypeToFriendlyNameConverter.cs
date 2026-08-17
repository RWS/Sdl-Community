using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters
{
    public class ModelTypeToFriendlyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ModelType modelType)
                return value?.ToString() ?? string.Empty;

            return modelType switch
            {
                ModelType.Latency_Optimized => "Latency Optimized",
                ModelType.Quality_Optimized => "Quality Optimized",
                ModelType.Prefer_Quality_Optimized => "Prefer Quality Optimized",
                _ => modelType.ToString()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}