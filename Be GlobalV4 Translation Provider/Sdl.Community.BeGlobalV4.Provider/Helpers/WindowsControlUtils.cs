using System;
using System.Linq;
using System.Windows;
using Application = System.Windows.Application;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
    public static class WindowsControlUtils
    {
        public static void ForWindowFromFrameworkElement(this object frameworkElement, Action<Window> action)
        {
            var element = frameworkElement as FrameworkElement;

            while (!(element is Window))
            {
                if (element == null)
                {
                    break;
                }

                element = element.Parent as FrameworkElement;
            }

            action(element as Window);
        }
    }
}