using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;

namespace Sdl.Community.TmAnonymizer.ViewModel
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
