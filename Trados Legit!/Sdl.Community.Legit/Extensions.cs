using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.Legit
{
    internal static class Extensions
    {
        internal static string LogMessage(this TextBox txt, string message)
        {
            txt.Text = txt.Text.AppendLine(message);
            return txt.Text;
        }

        internal static string AppendLine(this string text, string line)
        {
            return string.IsNullOrEmpty(text) ? line : string.Format("{0}\r\n{1}", text, line);
        }

        internal static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var items = collection as T[] ?? collection.ToArray();
            foreach (var item in items)
            {
                action(item);
            }

            return items;
        }

    }
}
