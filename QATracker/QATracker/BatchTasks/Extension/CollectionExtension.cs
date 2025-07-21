using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QATracker.BatchTasks.Extension;

public static class CollectionExtension
{
    public static Collection<T> AddIfNotPresent<T>(this Collection<T> collection, T item)
    {
        if (!collection.Contains(item))
        {
            collection.Add(item);
        }

        return collection;
    }
}

public static class ListExtension
{
    public static List<T> AddIfNotPresent<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }

        return list;
    }
}