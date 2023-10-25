using InterpretBank.SettingsService.ViewModel;
using System.Windows;
using System.Windows.Controls;
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

        private void GlossarySetup_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ChooseFilepathControl.AutoCompletePopup.IsOpen && !IsMouseOverPopup(e))
                ChooseFilepathControl.AutoCompletePopup.IsOpen = false;
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

        //This is used to hold the value as we clear the textbox
        //We need this value to send as a parameter for adding a new db value
        //Otherwise we'd have to create ChangeNotification properties for each textbox, which is less clean
        private void MoveTextToValueHolder(object sender)
        {
            ValueHolder.Text = (sender as TextBox).Text;
            (sender as TextBox).Clear();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            MoveTextToValueHolder(sender);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}