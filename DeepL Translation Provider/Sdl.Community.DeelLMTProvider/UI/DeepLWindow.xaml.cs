using Sdl.Community.DeepLMTProvider.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class DeepLWindow
    {
        public DeepLWindow(DeepLWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += DeepLWindow_Loaded;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeepLWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiVersionCombobox.SelectedIndex == -1) ApiVersionCombobox.SelectedIndex = 0;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("https://www.deepl.com/api-contact.html");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}