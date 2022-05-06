using System.Windows;
using System.Windows.Input;
using CustomViewExample.Commands;
using CustomViewExample.Services;

namespace CustomViewExample.ViewModel
{
	public class SettingsViewModel: BaseViewModel
	{
		private readonly SettingsService _settingsService;
		private bool _option1;
		private bool _option2;

		private ICommand _saveCommand;

		public SettingsViewModel(SettingsService settingsService)
		{
			Name = "Settings";

			_settingsService = settingsService;
			var settings = _settingsService.GetSettings();

			Option1 = settings.Option1;
			Option2 = settings.Option2;
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(Save));

		public bool Option1
		{
			get => _option1;
			set
			{
				if (_option1 == value)
				{
					return;
				}

				_option1 = value;
				OnPropertyChanged(nameof(Option1));
			}
		}

		public bool Option2
		{
			get => _option2;
			set
			{
				if (_option2 == value)
				{
					return;
				}

				_option2 = value;
				OnPropertyChanged(nameof(Option2));
			}
		}

		private void Save(object obj)
		{
			var settings = _settingsService.GetSettings();
			settings.Option1 = _option1; 
			settings.Option2 = _option2;
			
			_settingsService.SaveSettings(settings);
			
			if (obj is Window window)
			{
				window.DialogResult = true;
			}
		}
	}
}
