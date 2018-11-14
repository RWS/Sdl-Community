using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		private bool _reSendChecked;

		public SettingsViewModel(BeGlobalTranslationOptions options)
		{
			if (options != null)
			{
				ReSendChecked = options.ResendDrafts;
			}
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				OnPropertyChanged();
			}
		}
	}
}
