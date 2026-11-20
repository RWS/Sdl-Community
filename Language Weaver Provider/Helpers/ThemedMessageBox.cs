using LanguageWeaverProvider.View;
using System.Linq;
using System.Windows;

namespace LanguageWeaverProvider.Helpers
{
    /// <summary>
    /// MessageBox replacement that uses the native dialog in light theme and a themed window in dark theme.
    /// </summary>
    internal static class ThemedMessageBox
    {
        public static MessageBoxResult Show(
            string text,
            string caption = "",
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                return dispatcher.Invoke(() => Show(text, caption, button, icon));
            }

            var owner = GetOwner();

            if (!WindowThemeHelper.IsDarkTheme())
            {
                return owner is null
                    ? MessageBox.Show(text, caption, button, icon)
                    : MessageBox.Show(owner, text, caption, button, icon);
            }

            return ThemedMessageBoxWindow.Show(owner, text, caption, button, icon);
        }

        private static Window GetOwner()
        {
            return Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(window => window.IsLoaded && window.IsVisible && window.IsActive);
        }
    }
}
