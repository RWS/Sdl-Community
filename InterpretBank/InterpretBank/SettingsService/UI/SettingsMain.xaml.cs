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

        public Settings Settings => SettingsService.Settings;
        private ViewModel.SettingsService SettingsService => (ViewModel.SettingsService)DataContext;

        public void Setup(Settings settings)
        {
            SettingsService.Settings = settings;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}