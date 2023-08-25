using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.UI.Controls;
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

        public ICommand EscapeCommand => new ParameterlessCommand(EscapePressed);

        private void EscapePressed()
        {
            if (Filter_TextBox.Filter_TextBox.IsFocused)
            {
                Keyboard.Focus(Ok_Button);
                return;
            }
            CloseWindow();
        }


        private void CloseWindow()
        {
            if (ProgressBar.Visibility == Visibility.Visible) ((GlossariesWindowViewModel)DataContext).CancelCommand.Execute(null);
            else Close();
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((GlossariesWindowViewModel)DataContext).DeleteGlossariesCommand.Execute(null);
            Keyboard.Focus(Ok_Button);
        }
    }
}