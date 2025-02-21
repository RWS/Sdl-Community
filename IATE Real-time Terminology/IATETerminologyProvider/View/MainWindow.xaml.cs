using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.ViewModel;

namespace Sdl.Community.IATETerminologyProvider.View
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow(List<ISettingsViewModel> viewModels, SettingsModel settingsModel)
		{
			InitializeComponent();
			DataContext = new MainWindowViewModel(viewModels, settingsModel);
		}

		public SettingsModel ProviderSettings
		{
			get => (DataContext as MainWindowViewModel).ProviderSettings;
			set => (DataContext as MainWindowViewModel).ProviderSettings = value;
		}

		private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}