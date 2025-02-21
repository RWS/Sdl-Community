using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.ViewModel;
using System.Windows;
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

        public ICommand ExportCommand => new ParameterlessCommand(() => ExportButton_Click(null, null));

        

        private void CloseWindow()
        {
            if (ProgressBar.Visibility == Visibility.Visible) ((GlossariesWindowViewModel)DataContext).CancelCommand.Execute(null);
            else Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((GlossariesWindowViewModel)DataContext).DeleteGlossariesCommand.Execute(null);
            Keyboard.Focus(Ok_Button);
        }

        private void EscapePressed()
        {
            if (Filter_TextBox.Filter_TextBox.IsFocused)
            {
                Keyboard.Focus(Ok_Button);
                return;
            }
            CloseWindow();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = Export_Button.ContextMenu;
            if (contextMenu == null) return;

            contextMenu.PlacementTarget = Export_Button;
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
    }
}