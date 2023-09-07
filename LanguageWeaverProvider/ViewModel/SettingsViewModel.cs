using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		private bool _resendDrafts;
		private bool _includeTags;

		public SettingsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
			SetSettings();
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

		public bool IncludeTags
		{
			get => _includeTags;
			set
			{
				if (_includeTags == value) return;
				_includeTags = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public event EventHandler BackCommandExecuted;

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
		}

		private void SetSettings()
		{
			TranslationOptions.ProviderSettings ??= new();
			ResendDrafts = TranslationOptions.ProviderSettings.ResendDrafts;
			IncludeTags = TranslationOptions.ProviderSettings.IncludeTags;
		}

		private void Back(object parameter)
		{
			BackCommandExecuted?.Invoke(this, EventArgs.Empty);
		}
	}
}