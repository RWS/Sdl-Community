using LanguageWeaverProvider.SubscriptionJourney.ViewModel;
using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Interop;

namespace LanguageWeaverProvider.SubscriptionJourney.View
{
    /// <summary>
    /// Interaction logic for CohereSubscriptionWindow.xaml
    /// </summary>
    public partial class CohereSubscriptionWindow : Window
    {
        public CohereSubscriptionWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            SourceInitialized += OnSourceInitialized;

            // Optional: center relative to owner if one is set by caller
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SubscriptionViewModel vm)
            {
                vm.RequestClose += CloseFromViewModel;
                Closed += (_, __) => vm.RequestClose -= CloseFromViewModel;
            }
        }

        private void CloseFromViewModel()
        {
            DialogResult = true;
            Close();
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var handle = new WindowInteropHelper(this).Handle;
            WindowChromeHelper.DisableMinimizeAndMaximize(handle);
        }
    }

    internal static class WindowChromeHelper
    {
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void DisableMinimizeAndMaximize(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return;

            int style = GetWindowLong(hWnd, GWL_STYLE);
            style &= ~WS_MAXIMIZEBOX;
            style &= ~WS_MINIMIZEBOX;

            SetWindowLong(hWnd, GWL_STYLE, style);
        }
    }
}
