using Sdl.Community.DeepLMTProvider.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class GlossariesWindow
    {
        public GlossariesWindow()
        {
            InitializeComponent();
            SelectedLP_ComboBox.Loaded += SelectedLP_ComboBox_Loaded;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var contextMenu = button.ContextMenu;

            if (contextMenu == null) return;

            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ((GlossariesWindowViewModel)DataContext).CancelCommand.Execute(null);
            Close();
        }

        private void SelectedLP_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedLP_ComboBox.SelectedIndex == -1)
                SelectedLP_ComboBox.SelectedIndex = 0;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            if (ProgressBar.Visibility == Visibility.Visible) ((GlossariesWindowViewModel)DataContext).CancelCommand.Execute(null);

            e.Handled = true;
            Close();
        }
    }
}