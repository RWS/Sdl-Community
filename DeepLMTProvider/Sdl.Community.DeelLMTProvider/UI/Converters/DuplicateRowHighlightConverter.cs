using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Sdl.Community.DeepLMTProvider.UI.Converters;

public class DuplicateRowHighlightConverter : IValueConverter
{
    public ObservableCollection<GlossaryEntry> GlossaryEntries { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var entry = value as GlossaryEntry;
        if (entry == null || GlossaryEntries == null) return false;

        return GlossaryEntries.Count(e => e.IsDuplicate(entry)) > 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}