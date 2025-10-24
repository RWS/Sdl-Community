using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Trados_AI_Essentials.UI.View
{
    public class WindowBase : Window
    {
        private const int GWL_STYLE = -16;

        private const int WS_MAXIMIZEBOX = 0x10000;

        //maximize button
        private const int WS_MINIMIZEBOX = 0x20000;

        private IntPtr _windowHandle;

        public WindowBase()
        {
            SourceInitialized += MainWindow_SourceInitialized;
        }

        protected void DisableWindowButtons()
        {
            if (_windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("The window has not yet been completely initialized");
            }

            SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        //minimize button
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _windowHandle = new WindowInteropHelper(this).Handle;

            DisableWindowButtons();
        }
    }
}