using Sdl.Community.DtSearch4Studio.Provider.ViewModel;

namespace Sdl.Community.DtSearch4Studio.Provider.Ui
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow
	{
		private readonly SettingsViewModel _model;

		public SettingsWindow(SettingsViewModel model)
		{
			InitializeComponent();
			_model = model;
			Loaded += SettingsWindow_Loaded;
		}
		private void SettingsWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			Loaded -= SettingsWindow_Loaded;
			DataContext = _model;
		}
	}
}