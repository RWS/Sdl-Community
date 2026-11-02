using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.ViewModel;
using System.Collections.Generic;
using TradosStudio.API.ProjectManagement;

namespace Sdl.Community.IATETerminologyProvider.View
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class MainWindow
    {
		public MainWindow(List<ISettingsViewModel> viewModels, 
            SettingsModel settingsModel, 
            ICacheProvider cacheProvider,
            IMessageBoxService messageBoxService,
            IProjectsRegistry projectsRegistry)
		{
                InitializeComponent();
                DataContext = new MainWindowViewModel(
                    viewModels, 
                    settingsModel, 
                    cacheProvider, 
                    messageBoxService, 
                    projectsRegistry);
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