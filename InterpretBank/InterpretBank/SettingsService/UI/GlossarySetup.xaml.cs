using InterpretBank.SettingsService.ViewModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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
            DialogResult = true;
        }

       

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
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

        private bool IsMouseOverPopup(MouseButtonEventArgs e)
        {
            var point = e.GetPosition(this);
            var hitTestResult = VisualTreeHelper.HitTest(this, point);
            return hitTestResult.VisualHit is Popup;
        }

       
    }
}