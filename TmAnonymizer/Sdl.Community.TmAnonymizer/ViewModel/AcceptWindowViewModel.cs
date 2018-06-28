using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Ui;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class AcceptWindowViewModel:ViewModelBase
	{
		private string _description;
		private bool _accepted;
		private ICommand _okCommand;

		public AcceptWindowViewModel()
		{
			Description = Constants.AcceptDescription();
			
		}
		public ICommand OkCommand => _okCommand ??
		                                        (_okCommand = new RelayCommand(Ok));

		private void Ok(object window)
		{
			var settings = SettingsMethods.GetSettings();
			settings.Accepted = Accepted;
			File.WriteAllText(Constants.SettingsFilePath, JsonConvert.SerializeObject(settings));
			var accept = (AcceptWindow) window;
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
