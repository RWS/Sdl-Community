using LanguageWeaverProvider.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace LanguageWeaverProvider.View
{
    public partial class ThemedMessageBoxWindow : Window
    {
        public MessageBoxResult Result { get; private set; }

        public ThemedMessageBoxWindow()
        {
            InitializeComponent();
        }

        public ThemedMessageBoxWindow(string text, string caption, MessageBoxButton button, MessageBoxImage icon = MessageBoxImage.None) : this()
        {
            Title = caption;
            TxtMessage.Text = text;
            Result = button switch
            {
                MessageBoxButton.OK => MessageBoxResult.OK,
                MessageBoxButton.YesNo => MessageBoxResult.No,
                _ => MessageBoxResult.Cancel
            };

            ConfigureButtons(button);
            ConfigureIcon(icon);
            this.FollowStudioTheme();
        }

        private void ConfigureIcon(MessageBoxImage icon)
        {
            var systemIcon = icon switch
            {
                MessageBoxImage.Error => SystemIcons.Error,
                MessageBoxImage.Warning => SystemIcons.Warning,
                MessageBoxImage.Information => SystemIcons.Information,
                MessageBoxImage.Question => SystemIcons.Question,
                _ => null
            };

            if (systemIcon is null)
            {
                return;
            }

            ImgIcon.Source = Imaging.CreateBitmapSourceFromHIcon(systemIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ImgIcon.Visibility = Visibility.Visible;
        }

        public static MessageBoxResult Show(Window owner, string text, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            var win = new ThemedMessageBoxWindow(text, caption, button, icon);

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
                    BtnNo.IsCancel = true;
                    break;

                case MessageBoxButton.YesNoCancel:
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;

                default:
                    BtnOk.Visibility = Visibility.Visible;
                    BtnOk.IsCancel = true;
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
