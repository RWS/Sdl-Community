using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		bool _resendDrafts;
		bool _includeTags;
		bool _useCustomName;

		string _customName;

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

		public bool UseCustomName
		{
			get => _useCustomName;
			set
			{
				_useCustomName = value;
				OnPropertyChanged();
			}
		}

		public string CustomName
		{
			get => _customName;
			set
			{
				_customName = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public event EventHandler BackCommandExecuted;

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
			ClearCommand = new RelayCommand(Clear);
		}

		private void SetSettings()
		{
			TranslationOptions.ProviderSettings ??= new();
			ResendDrafts = TranslationOptions.ProviderSettings.ResendDrafts;
			IncludeTags = TranslationOptions.ProviderSettings.IncludeTags;
			UseCustomName = TranslationOptions.ProviderSettings.UseCustomName;
			CustomName = TranslationOptions.ProviderSettings.CustomName;
		}

		private void Back(object parameter)
		{
			BackCommandExecuted?.Invoke(this, EventArgs.Empty);
		}

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case nameof(CustomName):
					CustomName = string.Empty;
					break;
			}
		}
	}
}