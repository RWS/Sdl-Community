using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
    public class DeepLWindowViewModel : ViewModel
    {
        private string _apiKey;
        private string _apiKeyValidationMessage;
        private ObservableCollection<LanguagePairOptions> _languagePairSettings = new();
        private bool _removeLockedContent;
        private bool _sendPlainText;

        public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, IDeepLGlossaryClient glossaryClient, IMessageService messageService)
        {
            IsTellMeAction = true;
            MessageService = messageService;
            GlossaryClient = glossaryClient;

            var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
            Title = $"{Title} - {currentProject.GetProjectInfo().Name}";

            LanguagePairs = deepLTranslationOptions.LanguagePairOptions.Select(lpo => lpo.LanguagePair).ToArray();
            SendPlainText = deepLTranslationOptions.SendPlainText;
            RemoveLockedContent = deepLTranslationOptions.RemoveLockedContent;
            DecodeHtmlOrUrl = deepLTranslationOptions.DecodeFromHtmlOrUrl;
            Options = deepLTranslationOptions;

            SetSettingsOnWindow(null);
            LoadLanguagePairSettings();
        }

        public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, IDeepLGlossaryClient glossaryClient, TranslationProviderCredential credentialStore, LanguagePair[] languagePairs, IMessageService messageService)
        {
            MessageService = messageService;
            LanguagePairs = languagePairs;
            GlossaryClient = glossaryClient;
            IsTellMeAction = false;

            SendPlainText = deepLTranslationOptions.SendPlainText;
            RemoveLockedContent = deepLTranslationOptions.RemoveLockedContent;
            DecodeHtmlOrUrl = deepLTranslationOptions.DecodeFromHtmlOrUrl;
            Options = deepLTranslationOptions;

            PasswordChangedTimer.Elapsed += OnPasswordChanged;

            SetSettingsOnWindow(credentialStore);
            DeepLTranslationProviderClient.ApiKeyChanged += Dispatcher_LoadLanguagePairSettings;
        }

        public event Action ManageGlossaries;

        public string ApiKey
        {
            get => _apiKey;
            set
            {
                SetField(ref _apiKey, value?.Trim());
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

        public bool DecodeHtmlOrUrl { get; set; }

        public ObservableCollection<LanguagePairOptions> LanguagePairOptions
        {
            get => _languagePairSettings;
            set => SetField(ref _languagePairSettings, value);
        }

        public ICommand ManageGlossariesCommand => new ParameterlessCommand(() => ManageGlossaries?.Invoke());

        public ICommand OkCommand => new ParameterlessCommand(Save, () => ApiKeyValidationMessage == null);

        public DeepLTranslationOptions Options { get; set; }

        public bool RemoveLockedContent
        {
            get => _removeLockedContent;
            set => SetField(ref _removeLockedContent, value);
        }

        public bool SendPlainText
        {
            get => _sendPlainText;
            set
            {
                SetField(ref _sendPlainText, value);
                if (!value) RemoveLockedContent = false;
            }
        }

        public string Title { get; set; } = "DeepL Translation Provider";

        private IDeepLGlossaryClient GlossaryClient { get; set; }

        private bool IsTellMeAction { get; }

        private LanguagePair[] LanguagePairs { get; }

        private IMessageService MessageService { get; }

        private Timer PasswordChangedTimer { get; } = new()
        {
            Interval = 500,
            AutoReset = false
        };

        public async void LoadLanguagePairSettings()
        {
            var (success, glossaries, message) = await GlossaryClient.GetGlossaries(DeepLTranslationProviderClient.ApiKey);
            if (!success)
            {
                HandleError(message);
                glossaries = new List<GlossaryInfo>();
            }

            glossaries?.Add(GlossaryInfo.NoGlossary);

            foreach (var languagePair in LanguagePairs)
            {
                var sourceLangCode = languagePair.GetSourceLanguageCode();
                var targetLangCode = languagePair.GetTargetLanguageCode();

                var languageSavedOptions =
                    Options.LanguagePairOptions?.FirstOrDefault(lpo => lpo.LanguagePair.Equals(languagePair));

                var selectedGlossary = GetSelectedGlossaryFromSavedSetting(glossaries, languageSavedOptions, sourceLangCode, targetLangCode);

                var newLanguagePairOptions = new LanguagePairOptions
                {
                    Formality = languageSavedOptions?.Formality ?? Formality.Default,
                    Glossaries = glossaries?.Where(g => g.SourceLanguage == sourceLangCode && g.TargetLanguage == targetLangCode || g.Name == PluginResources.NoGlossary).ToList(),
                    SelectedGlossary = selectedGlossary,
                    LanguagePair = languagePair
                };

                var oldLanguagePairOption = LanguagePairOptions.FirstOrDefault(lpo => lpo.LanguagePair.Equals(languagePair));

                RestorePreviouslySelectedGlossaryIfThereIsOne(oldLanguagePairOption, newLanguagePairOptions);

                LanguagePairOptions.Remove(oldLanguagePairOption);
                LanguagePairOptions.Add(newLanguagePairOptions);
            }
        }

        private static GlossaryInfo GetSelectedGlossaryFromSavedSetting(List<GlossaryInfo> glossaries, LanguagePairOptions languageSavedOptions, string sourceLangCode, string targetLangCode)
        {
            if (languageSavedOptions == null)
                return GlossaryInfo.NoGlossary;

            if (languageSavedOptions.SelectedGlossary.Name == PluginResources.NoGlossary)
                return languageSavedOptions.SelectedGlossary;

            if ((glossaries?.Contains(languageSavedOptions.SelectedGlossary) ?? false)
                    && languageSavedOptions.SelectedGlossary.SourceLanguage == sourceLangCode
                    && languageSavedOptions.SelectedGlossary.TargetLanguage == targetLangCode)
                return languageSavedOptions.SelectedGlossary;

            return GlossaryInfo.NoGlossary;
        }

        private static void RestorePreviouslySelectedGlossaryIfThereIsOne(LanguagePairOptions oldLanguagePairOption,
                    LanguagePairOptions newLanguagePairOptions)
        {
            if (oldLanguagePairOption != null && (newLanguagePairOptions.Glossaries?.Select(g => g.Name)
                    .Contains(oldLanguagePairOption.SelectedGlossary.Name) ?? false))
                newLanguagePairOptions.SelectedGlossary =
                    newLanguagePairOptions.Glossaries.FirstOrDefault(g =>
                        g.Name == oldLanguagePairOption.SelectedGlossary.Name);
        }

        private void AskUserToRestart()
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            var documentsOpened = editorController.GetDocuments().Any();

            if (documentsOpened)
            {
                MessageService.ShowWarning(PluginResources.SettingsUpdated_ReopenFilesForEditing,
                    PluginResources.SettingsUpdated);
            }
        }

        private void Dispatcher_LoadLanguagePairSettings() =>
            Application.Current.Dispatcher.Invoke(LoadLanguagePairSettings);

        private void HandleError(string message, [CallerMemberName] string failingMethod = null)
        {
            MessageService.ShowWarning(message, failingMethod);
        }

        private void OnPasswordChanged(object sender, EventArgs e)
        {
            DeepLTranslationProviderClient.ApiKey = ApiKey;
            SetApiKeyValidityLabel();
        }

        private void Save()
        {
            DeepLTranslationProviderClient.ApiKey = ApiKey;
            SetApiKeyValidityLabel();

            Options.SendPlainText = SendPlainText;
            Options.RemoveLockedContent = RemoveLockedContent;
            Options.DecodeFromHtmlOrUrl = DecodeHtmlOrUrl;
            Options.ApiKey = ApiKey;
            Options.LanguagePairOptions = new List<LanguagePairOptions>(LanguagePairOptions);

            if (IsTellMeAction)
            {
                AskUserToRestart();
            }
        }

        private void SetApiKeyValidityLabel()
        {
            if (!string.IsNullOrEmpty(ApiKey))
            {
                ApiKeyValidationMessage = null;

                var isApiKeyValidResponse = DeepLTranslationProviderClient.IsApiKeyValidResponse;
                if (isApiKeyValidResponse?.IsSuccessStatusCode ?? false)
                    return;

                SetValidationBlockMessage(isApiKeyValidResponse?.StatusCode == HttpStatusCode.Forbidden
                    ? "Authorization failed. Please supply a valid API Key."
                    : $"{isApiKeyValidResponse?.StatusCode}");

                return;
            }

            SetValidationBlockMessage(PluginResources.ApiKeyIsRequired_ValidationBlockMessage);
        }

        private void SetSettingsOnWindow(TranslationProviderCredential credentialStore)
        {
            if (IsTellMeAction)
            {
                ApiKeyBoxEnabled = false;
            }
            else
            {
                ApiKeyBoxEnabled = true;
                ApiKey = credentialStore?.Credential;
                Options.ApiKey = ApiKey;
            }
        }

        private void SetValidationBlockMessage(string message = null)
        {
            ApiKeyValidationMessage = message;
        }
    }
}