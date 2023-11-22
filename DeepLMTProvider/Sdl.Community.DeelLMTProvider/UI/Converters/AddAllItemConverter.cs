using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters
{
    public class AddAllItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is IEnumerable<GlossaryLanguagePair> collection
                ? collection.Prepend(new GlossaryLanguagePair { Label = PluginResources.AllLanguagePairs_Label })
                : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}