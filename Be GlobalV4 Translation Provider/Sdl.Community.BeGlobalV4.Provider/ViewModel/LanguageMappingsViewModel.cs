using System.Collections.ObjectModel;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LanguageMappingsViewModel : BaseViewModel
	{
		private TranslationModel _selectedModel;
		private bool _reSendChecked;

		public LanguageMappingsViewModel(BeGlobalTranslationOptions options)
		{
			Options = options;
			TranslationOptions = new ObservableCollection<TranslationModel>();

			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
			}
		}

		public BeGlobalTranslationOptions Options { get; set; }
		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }
		
		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				if (Options?.Model != null)
				{
					SetOptions(value);
				}
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				if (Options?.Model != null)
				{
					Options.ResendDrafts = value;
				}
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}

		public void SetOptions(TranslationModel translationModel)
		{
			Options.Model = translationModel?.Model;
			Options.DisplayName = translationModel?.DisplayName;
			Options.LanguagesSupported = translationModel?.LanguagesSupported;
		}
	}
}
