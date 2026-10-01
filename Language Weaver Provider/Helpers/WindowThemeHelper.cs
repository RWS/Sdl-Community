using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace LanguageWeaverProvider.Helpers
{
    /// <summary>
    /// Utility class to detect the active application theme and apply dark mode styling to a window's native title bar via DWM.
    /// </summary>
    internal static class WindowThemeHelper
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_CAPTION_COLOR = 35;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int pvAttribute, int cbAttribute);

        public static void FollowStudioTheme(this Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            window.SourceInitialized += (s, e) => Apply((Window)s);
        }

        public static bool IsDarkTheme()
        {
            var background = ResolveWindowBackground();

            return background.HasValue && IsDark(background.Value) && !SystemParameters.HighContrast;
        }

        private static void Apply(Window window)
        {
            var background = ResolveWindowBackground();

            if (background is not { } color || !IsDark(color) || SystemParameters.HighContrast)
            {
                return;
            }

            var handle = new WindowInteropHelper(window).Handle;

            if (handle == IntPtr.Zero)
            {
                return;
            }

            int useDarkMode = 1;

            if (DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int)) != 0)
            {
                _ = DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDarkMode, sizeof(int));
            }

            int captionColor = color.R | (color.G << 8) | (color.B << 16);

            _ = DwmSetWindowAttribute(handle, DWMWA_CAPTION_COLOR, ref captionColor, sizeof(int));
        }

        private static Color? ResolveWindowBackground()
        {
            var resources = Application.Current?.Resources;

            if (resources == null)
            {
                return null;
            }

            if (resources["Rws.Bg.Main"] is SolidColorBrush themed)
            {
                return themed.Color;
            }

            if (resources[SystemColors.WindowBrushKey] is SolidColorBrush system)
            {
                return system.Color;
            }

            return null;
        }

        private static bool IsDark(Color color)
        {
            double luminance = ((0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B)) / 255d;

            return luminance < 0.5d;
        }
    }
}
