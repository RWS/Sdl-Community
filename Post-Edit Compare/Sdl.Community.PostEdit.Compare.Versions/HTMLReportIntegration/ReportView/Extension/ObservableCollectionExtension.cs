using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Extension
{
    public static class ObservableCollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (collection.Contains(item)) continue;
                collection.Add(item);
            }
        }

        public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items) collection.Remove(item);
        }
    }
}