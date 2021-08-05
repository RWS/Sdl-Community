using Sdl.Community.IATETerminologyProvider.Interface;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SettingsViewModelBase : ViewModelBase, ISettingsViewModel
	{
		private SettingsModel _settings;

		public SettingsModel Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				OnPropertyChanged(nameof(Settings));
			}
		}
	}
}