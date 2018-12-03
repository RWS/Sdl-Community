using System.Collections.ObjectModel;
using System.Linq;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		private bool _reSendChecked;
		private TranslationModel _selectedModel;
		private string _messageVisibility;
		public SettingsViewModel(BeGlobalTranslationOptions options)
		{
			TranslationOptions = new ObservableCollection<TranslationModel>();
			MessageVisibility = "Collapsed";
			if (options != null)
			{
				ReSendChecked = options.ResendDrafts;
				if (options.Model != null)
				{
					var model = TranslationOptions.FirstOrDefault(m => m.Model.Equals(options.Model));
					if (model != null)
					{
						var selectedModelIndex = TranslationOptions.IndexOf(model);
						SelectedModelOption = TranslationOptions[selectedModelIndex];
					} 
				}
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
		public string MessageVisibility
		{
			get => _messageVisibility;
			set
			{
				_messageVisibility = value;
				OnPropertyChanged();
			}
		}

		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }
	}
}
