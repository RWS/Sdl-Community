using LanguageWeaverProvider.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace LanguageWeaverProvider.View
{
    public partial class ThemedMessageBoxWindow : Window
    {
        public MessageBoxResult Result { get; private set; }

        public ThemedMessageBoxWindow()
        {
            InitializeComponent();
        }

        public ThemedMessageBoxWindow(string text, string caption, MessageBoxButton button) : this()
        {
            Title = caption;
            TxtMessage.Text = text;
            Result = button == MessageBoxButton.YesNo ? MessageBoxResult.No : MessageBoxResult.Cancel;

            ConfigureButtons(button);
            this.FollowStudioTheme();
        }

        public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton button)
        {
            var win = new ThemedMessageBoxWindow(text, caption, button);

            if (owner != null)
            {
                win.Owner = owner;
            }
            else
            {
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ShowInTaskbar = true;

                var mainHandle = Process.GetCurrentProcess().MainWindowHandle;
                if (mainHandle != IntPtr.Zero)
                {
                    new WindowInteropHelper(win).Owner = mainHandle;
                }
            }

            win.ShowDialog();
            return win.Result;
        }

        private void ConfigureButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    BtnOk.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNo:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNoCancel:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;

                default:
                    BtnOk.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.OK);
        private void BtnYes_Click(object sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.Yes);
        private void BtnNo_Click(object sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.No);
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => CloseWithResult(MessageBoxResult.Cancel);

        private void CloseWithResult(MessageBoxResult result)
        {
            Result = result;
            DialogResult = true;
        }
    }
}
