using InterpretBank.SettingsService.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace InterpretBank.SettingsService.UI
{
    /// <summary>
    /// Interaction logic for SetupGlossaries.xaml
    /// </summary>
    public partial class GlossarySetup : Window
    {
        public GlossarySetup(GlossarySetupViewModel glossarySetupViewModel)
        {
            DataContext = glossarySetupViewModel;
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = Import_Button.ContextMenu;
            if (contextMenu == null) return;

            contextMenu.PlacementTarget = Import_Button;
            contextMenu.IsOpen = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://appstore.rws.com/Plugin/243?tab=documentation");
        }
    }
}