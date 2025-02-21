using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TellMe.View
{
    /// <summary>
    /// Interaction logic for ProviderSelectorView.xaml
    /// </summary>
    public partial class ProviderSelectorView : Window
    {
        public ProviderSelectorView()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GetWindow(this) is Window window)
            {
                window.DragMove();
            }
        }
    }
}