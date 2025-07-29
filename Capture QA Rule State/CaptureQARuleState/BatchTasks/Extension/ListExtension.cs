using System.Collections.Generic;

namespace CaptureQARuleState.BatchTasks.Extension;

public static class ListExtension
{
    public static List<T> AddIfNotPresent<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
            list.Add(item);
        return list;
    }
}