using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
    /// <summary>
    /// Interaction logic for SettingsWarningView.xaml
    /// </summary>
	public partial class SettingsWarningView : Window
    {
        readonly string _url;

        public SettingsWarningView(string url)
        {
            InitializeComponent();
            _url = url;
        }

        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void OpenUrl_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter
             || e.Key == Key.Space)
            {
                Process.Start(_url);
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenUrl_ButtonClicked(object sender, MouseButtonEventArgs e)
        {
            Process.Start(_url);
        }
    }
}