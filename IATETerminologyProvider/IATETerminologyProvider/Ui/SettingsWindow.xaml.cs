using IATETerminologyProvider.ViewModel;

namespace IATETerminologyProvider.Ui
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow
    {
        public SettingsWindow(SettingsViewModel settingsViewModel)
        {
			InitializeComponent();
			DataContext = settingsViewModel;
		}
    }
}