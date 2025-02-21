using System;
using System.Windows;

namespace InterpretBank.TermbaseViewer.UI.Controls
{
    public static class EditBoxHelper
    {
        public static event Action<DependencyObject> GotFocus;

        public static void RaiseGotFocus(DependencyObject obj) => GotFocus?.Invoke(obj);
    }
}