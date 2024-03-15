using System.Windows;

namespace InterpretBank.SettingsService.UI
{
    /// <summary>
    /// Interaction logic for CreateGlossaryWindow.xaml
    /// </summary>
    public partial class CreateGlossaryWindow : Window
    {
        public CreateGlossaryWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) => Close();
    }
}