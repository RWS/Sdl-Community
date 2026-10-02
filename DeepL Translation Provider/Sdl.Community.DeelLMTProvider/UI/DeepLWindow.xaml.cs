using Sdl.Community.DeepLMTProvider.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class DeepLWindow
    {
        public DeepLWindow(DeepLWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Clicking a non-focusable spot keeps focus in the custom context box; drop it so the box collapses.
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (CustomContext.IsKeyboardFocusWithin && !CustomContext.IsMouseOver)
                Keyboard.ClearFocus();
            base.OnPreviewMouseDown(e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void ValidationDetailsLink_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (DeepLWindowViewModel)DataContext;
            var detailsWindow = new ValidationDetailsWindow(viewModel.ValidationIssues, viewModel.ValidationNotes)
            {
                Owner = this
            };
            detailsWindow.ShowDialog();
        }
    }
}