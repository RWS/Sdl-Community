using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.View;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class AcceptViewModel : ViewModelBase
	{
		private string _description;
		private bool _accepted;
		private ICommand _okCommand;
		private readonly SettingsService _settingsService;

		public AcceptViewModel(SettingsService settingsService)
		{
			_settingsService = settingsService;
			Description = StringResources.AcceptsNoLiability_Description_Line01 +
						  "\r\n\r\n" + 
			              StringResources.AcceptsNoLiability_Description_Line02;
		}
		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		private void Ok(object window)
		{
			var settings = _settingsService.GetSettings();
			settings.Accepted = Accepted;

			_settingsService.SaveSettings(settings);

			var accept = (AcceptView)window;
			accept.Close();
		}

		public string Description
		{
			get => _description;
			set
			{
				if (Equals(value, _description))
				{
					return;
				}
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		public bool Accepted
		{
			get => _accepted;
			set
			{
				if (Equals(value, _accepted))
				{
					return;
				}
				_accepted = value;
				OnPropertyChanged(nameof(Accepted));
			}
		}
	}
}
