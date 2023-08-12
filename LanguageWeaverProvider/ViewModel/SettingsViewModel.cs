using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		private bool _resendDrafts;
		private bool _excludeTags;

		public SettingsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
		}

		public ITranslationOptions TranslationOptions { get; private set; }

		public bool ResendDrafts
		{
			get => _resendDrafts;
			set
			{
				if (_resendDrafts == value) return;
				_resendDrafts = value;
				OnPropertyChanged();
			}
		}

		public bool ExcludeTags
		{
			get => _excludeTags;
			set
			{
				if (_excludeTags == value) return;
				_excludeTags = value;
				OnPropertyChanged();
			}
		}
	}
}