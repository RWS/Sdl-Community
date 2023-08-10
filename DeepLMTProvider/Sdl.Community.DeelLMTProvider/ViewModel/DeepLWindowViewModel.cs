using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
	public class DeepLWindowViewModel : ViewModel
	{
		private string _apiKey;
		private string _apiKeyValidationMessage;
		private bool _formalityCompatible = true;
		private int _formalitySelectedIndex;
		private ObservableCollection<LanguagePairOptions> _languagePairSettings = new();

		public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, TranslationProviderCredential credentialStore = null, LanguagePair[] languagePairs = null, DeepLGlossaryClient glossaryClient = null, bool isTellMeAction = false)
		{
			LanguagePairs = languagePairs;
			IsTellMeAction = isTellMeAction;

			FormalitySelectedIndex = (int)deepLTranslationOptions.Formality;
			SendPlainText = deepLTranslationOptions.SendPlainText;
			Options = deepLTranslationOptions;
			GlossaryClient = glossaryClient;

			PasswordChangedTimer.Elapsed += OnPasswordChanged;
			SetSettingsOnWindow(credentialStore, isTellMeAction);

			LoadLanguagePairSettings();
		}

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				SetField(ref _apiKey, value);
				PasswordChangedTimer.Enabled = true;
			}
		}

		public bool ApiKeyBoxEnabled { get; set; }

		public string ApiKeyValidationMessage
		{
			get => _apiKeyValidationMessage;
			set
			{
				SetField(ref _apiKeyValidationMessage, value);
				OnPropertyChanged(nameof(OkCommand));
			}
		}

		public bool FormalityCompatible
		{
			get => _formalityCompatible;
			set => SetField(ref _formalityCompatible, value);
		}

		public int FormalitySelectedIndex
		{
			get => _formalitySelectedIndex;
			set => SetField(ref _formalitySelectedIndex, value);
		}

		public ObservableCollection<LanguagePairOptions> LanguagePairOptions
		{
			get => _languagePairSettings;
			set => SetField(ref _languagePairSettings, value);
		}

		public ICommand OkCommand => new NoParameterCommand(Save, () => ApiKeyValidationMessage == null);

		public DeepLTranslationOptions Options { get; set; }

		public bool SendPlainText { get; set; }

		private DeepLGlossaryClient GlossaryClient { get; }

		private bool IsTellMeAction { get; set; }

		private LanguagePair[] LanguagePairs { get; set; }

		private Timer PasswordChangedTimer { get; } = new()
		{
			Interval = 500,
			AutoReset = false
		};

		private static void AskUserToRestart()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var documentsOpened = editorController.GetDocuments().Any();

			if (documentsOpened)
			{
				MessageBox.Show(PluginResources.SettingsUpdated_ReopenFilesForEditing,
					PluginResources.SettingsUpdated, MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private async void LoadLanguagePairSettings()
		{
			var glossaries = await GlossaryClient.GetGlossaries(ApiKey);
			foreach (var languagePair in LanguagePairs)
			{
				var sourceLangCode = languagePair.SourceCultureName.Split('-')[0];
				var targetLangCode = languagePair.TargetCultureName.Split('-')[0];
				LanguagePairOptions.Add(new LanguagePairOptions
				{
					Formality = Formality.Default,
					Glossaries = glossaries.Where(g => g.SourceLanguage == sourceLangCode && g.TargetLanguage == targetLangCode).ToList(),
					SelectedGlossary = glossaries?.FirstOrDefault(),
					LanguagePair = languagePair
				});
			}
		}

		private void OnPasswordChanged(object sender, EventArgs e)
		{
			Options.ApiKey = ApiKey.Trim();

			DeepLTranslationProviderConnecter.ApiKey = Options.ApiKey;

			SetApiKeyValidityLabel();
			SetFormalityCompatibilityLabel();
		}

		private void Save()
		{
			DeepLTranslationProviderConnecter.ApiKey = Options.ApiKey;
			SetApiKeyValidityLabel();

			Enum.TryParse<Formality>(FormalitySelectedIndex.ToString(), out var formality);

			Options.Formality = formality;
			Options.SendPlainText = SendPlainText;

			if (IsTellMeAction)
			{
				AskUserToRestart();
			}
		}

		private void SetApiKeyValidityLabel()
		{
			if (!string.IsNullOrEmpty(Options.ApiKey))
			{
				ApiKeyValidationMessage = null;

				var isApiKeyValidResponse = DeepLTranslationProviderConnecter.IsApiKeyValidResponse;
				if (isApiKeyValidResponse?.IsSuccessStatusCode ?? false)
					return;

				SetValidationBlockMessage(isApiKeyValidResponse?.StatusCode == HttpStatusCode.Forbidden
					? "Authorization failed. Please supply a valid API Key."
					: $"{isApiKeyValidResponse?.StatusCode}");

				return;
			}

			SetValidationBlockMessage(PluginResources.ApiKeyIsRequired_ValidationBlockMessage);
		}

		private void SetFormalityCompatibilityLabel()
		{
			var currentLanguagePairs = IsTellMeAction
				? Options?.LanguagesSupported?.Keys.Select(key => new CultureInfo(key)).ToList()
				: LanguagePairs?.Select(lp => new CultureInfo(lp.TargetCultureName)).ToList();

			var formalityIncompatibleLanguages =
				DeepLTranslationProviderConnecter.GetFormalityIncompatibleLanguages(currentLanguagePairs);

			FormalityCompatible = formalityIncompatibleLanguages.Count == 0;
		}

		private void SetSettingsOnWindow(TranslationProviderCredential credentialStore, bool isTellMeAction)
		{
			if (isTellMeAction)
			{
				ApiKeyBoxEnabled = false;
			}
			else
			{
				if (credentialStore == null)
					return;

				ApiKeyBoxEnabled = true;
				ApiKey = credentialStore.Credential;
				Options.ApiKey = ApiKey;
			}
		}

		private void SetValidationBlockMessage(string message = null)
		{
			ApiKeyValidationMessage = message;
		}
	}
}