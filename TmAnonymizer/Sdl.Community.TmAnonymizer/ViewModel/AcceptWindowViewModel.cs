using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Services;
using AcceptWindow = Sdl.Community.SdlTmAnonymizer.View.AcceptWindow;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class AcceptWindowViewModel : ViewModelBase
	{
		private string _description;
		private bool _accepted;
		private ICommand _okCommand;
		private readonly SettingsService _settingsService;

		public AcceptWindowViewModel(SettingsService settingsService)
		{
			_settingsService = settingsService;
			Description = StringResources.AcceptsNoLiability_Description;

		}
		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		private void Ok(object window)
		{
			var settings = _settingsService.GetSettings();
			settings.Accepted = Accepted;

			_settingsService.SaveSettings(settings);

			var accept = (AcceptWindow)window;
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
