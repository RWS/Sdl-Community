using InterpretBank.GlossaryService.Interface;
using System.Windows;
using System.Windows.Input;

namespace InterpretBank.SettingsService.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsMain : Window
    {
        public SettingsMain(ViewModel.SettingsService settingsService)
        {
            DataContext = settingsService;
            InitializeComponent();
        }

        private ViewModel.SettingsService SettingsService => (ViewModel.SettingsService)DataContext;

        public SettingsService.Settings Settings => SettingsService.Settings;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        public void Setup(SettingsService.Settings settings)
        {
            SettingsService.Settings = settings;
        }
    }
}