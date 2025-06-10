using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters
{
    public class ModelTypeToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ModelType modelType)
                return value?.ToString() ?? string.Empty;

            return modelType switch
            {
                ModelType.Latency_Optimized => "Uses lower latency “classic” translation models, which support all language pairs; default value",
                ModelType.Quality_Optimized => "Uses higher latency, improved quality “next-gen” translation models, which support only a subset of language pairs; if a language pair that is not supported by next-gen models is included in the request, it will fail. Consider using prefer_quality_optimized instead.",
                ModelType.Prefer_Quality_Optimized => "Prioritizes use of higher latency, improved quality “next-gen” translation models, which support only a subset of DeepL languages; if a request includes a language pair not supported by next-gen models, the request will fall back to latency_optimized classic models",
                _ => modelType.ToString()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}