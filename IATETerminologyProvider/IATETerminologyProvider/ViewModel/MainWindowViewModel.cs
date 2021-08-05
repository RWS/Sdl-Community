using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.ViewModel
{
	public class MainWindowViewModel
	{
		private SettingsModel _providerSettings;

		public MainWindowViewModel(List<SettingsViewModelBase> viewModels, SettingsModel settingsModel)
		{
			ViewModels = viewModels;
			ProviderSettings = settingsModel;
		}

		public SettingsModel ProviderSettings
		{
			get => _providerSettings;
			set
			{
				_providerSettings = value;
				ViewModels.ForEach(vm => vm.Settings = _providerSettings);
			}
		}

		public List<SettingsViewModelBase> ViewModels { get; set; }
	}
}