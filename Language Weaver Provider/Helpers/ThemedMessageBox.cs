using LanguageWeaverProvider.View;
using System.Linq;
using System.Windows;
using WinForms = System.Windows.Forms;

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

            return ThemedMessageBoxWindow.Show(owner, text, caption, button);
        }

        public static MessageBoxResult Show(
            string text,
            string caption,
            WinForms.MessageBoxButtons buttons,
            WinForms.MessageBoxIcon icon)
        {
            return Show(text, caption, ConvertButtons(buttons), ConvertIcon(icon));
        }

        private static Window GetOwner()
        {
            var windows = Application.Current?.Windows
                .OfType<Window>()
                .Where(window => window.IsLoaded && window.IsVisible)
                .ToList();

            return windows?.FirstOrDefault(window => window.IsActive) ?? windows?.LastOrDefault();
        }

        private static MessageBoxButton ConvertButtons(WinForms.MessageBoxButtons buttons) => buttons switch
        {
            WinForms.MessageBoxButtons.OKCancel => MessageBoxButton.OKCancel,
            WinForms.MessageBoxButtons.YesNo => MessageBoxButton.YesNo,
            WinForms.MessageBoxButtons.YesNoCancel => MessageBoxButton.YesNoCancel,
            _ => MessageBoxButton.OK
        };

        private static MessageBoxImage ConvertIcon(WinForms.MessageBoxIcon icon) => icon switch
        {
            WinForms.MessageBoxIcon.Warning => MessageBoxImage.Warning,
            WinForms.MessageBoxIcon.Error => MessageBoxImage.Error,
            WinForms.MessageBoxIcon.Information => MessageBoxImage.Information,
            WinForms.MessageBoxIcon.Question => MessageBoxImage.Question,
            _ => MessageBoxImage.None
        };
    }
}
