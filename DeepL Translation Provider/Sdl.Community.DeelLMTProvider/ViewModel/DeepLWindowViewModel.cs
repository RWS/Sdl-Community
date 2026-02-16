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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
    public class DeepLWindowViewModel : ViewModel
    {
        public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, IDeepLGlossaryClient glossaryClient, IMessageService messageService)
        {
            IsTellMeAction = true;
            MessageService = messageService;
            GlossaryClient = glossaryClient;

            var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
            Title = $"{Title} - {currentProject.GetProjectInfo().Name}";

            LanguagePairs = deepLTranslationOptions.LanguagePairOptions.Select(lpo => lpo.LanguagePair).ToArray();
            SendPlainText = deepLTranslationOptions.SendPlainText;
            ResendDraft = deepLTranslationOptions.ResendDraft;
            TagType = deepLTranslationOptions.TagHandling;
            SplitSentencesType = deepLTranslationOptions.SplitSentenceHandling;
            PreserveFormatting = deepLTranslationOptions.PreserveFormatting;
            ApiVersion = deepLTranslationOptions.ApiVersion;
            IgnoreTags = deepLTranslationOptions.IgnoreTagsParameter;
            ModelType = deepLTranslationOptions.ModelType;

            Options = deepLTranslationOptions;

            LoadCredentialSettings(null);
            LoadLanguagePairSettings();
        }

        public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, IDeepLGlossaryClient glossaryClient, TranslationProviderCredential credentialStore, LanguagePair[] languagePairs, IMessageService messageService)
        {
            MessageService = messageService;
            LanguagePairs = languagePairs;
            GlossaryClient = glossaryClient;
            IsTellMeAction = false;

            Options = deepLTranslationOptions;

            SendPlainText = deepLTranslationOptions.SendPlainText;
            ResendDraft = deepLTranslationOptions.ResendDraft;
            PreserveFormatting = deepLTranslationOptions.PreserveFormatting;
            TagType = deepLTranslationOptions.TagHandling;
            SplitSentencesType = deepLTranslationOptions.SplitSentenceHandling;
            ApiVersion = deepLTranslationOptions.ApiVersion;
            IgnoreTags = deepLTranslationOptions.IgnoreTagsParameter;
            ModelType = deepLTranslationOptions.ModelType;

            PasswordChangedTimer.Elapsed += OnPasswordChanged;

            LoadCredentialSettings(credentialStore);
            //DeepLTranslationProviderClient.ApiKeyChanged += Dispatcher_LoadLanguagePairSettings;
        }

        public event Action ManageGlossaries;

        public string ApiKey
        {
            get;
            set
            {
                SetField(ref field, value?.Trim());
                PasswordChangedTimer.Enabled = true;
            }
        }

        public bool ApiKeyBoxEnabled { get; set; }

        public string ApiKeyValidationMessage
        {
            get;
            set
            {
                SetField(ref field, value);
                OnPropertyChanged(nameof(OkCommand));
            }
        }

        public string ApiVersion
        {
            get;
            set
            {
                SetField(ref field, value);
                OnPasswordChanged(null, null);
            }
        }

        public ICommand CancelCommand => new ParameterlessCommand(DetachEvents);

        public List<string> IgnoreTags
        {
            get;
            set => SetField(ref field, value);
        }

        public ObservableCollection<LanguagePairOptions> LanguagePairOptions
        {
            get;
            set => SetField(ref field, value);
        } = new();

        public ICommand ManageGlossariesCommand => new ParameterlessCommand(() => ManageGlossaries?.Invoke(), () => ApiKeyValidationMessage == null);

        public ModelType ModelType
        {
            get;
            set => SetField(ref field, value);
        }

        public ICommand OkCommand => new ParameterlessCommand(Save, () => ApiKeyValidationMessage == null);

        public DeepLTranslationOptions Options { get; set; }

        public bool PreserveFormatting
        {
            get;
            set => SetField(ref field, value);
        }

        public bool ResendDraft
        {
            get;
            set
            {
                SetField(ref field, value);
            }
        }

        public bool SendPlainText
        {
            get;
            set => SetField(ref field, value);
        }

        public SplitSentences SplitSentencesType
        {
            get;
            set => SetField(ref field, value);
        }

        public TagFormat TagType
        {
            get;
            // Add a method here to update the splitsentencestype
            set
            {
                SetField(ref field, value);
                SplitSentencesType = GetDefaultSplitSentences(value);
            }
        }

        public string Title { get; set; } = "DeepL Translation Provider";

        public string ValidationMessages
        {
            get;
            set => SetField(ref field, value);
        }

        private IDeepLGlossaryClient GlossaryClient { get; set; }
        private bool IsTellMeAction { get; }
        private LanguagePair[] LanguagePairs { get; }
        private IMessageService MessageService { get; }

        private Timer PasswordChangedTimer { get; } = new()
        {
            Interval = 500,
            AutoReset = false
        };

        public async Task LoadLanguagePairSettings()
        {
            ValidationMessages = null;
            List<GlossaryInfo> glossaries = [];
            List<DeepLStyle> allStyles = [];

            if (DeepLTranslationProviderClient.IsApiKeyValidResponse.IsSuccessStatusCode)
            {
                glossaries = await GetGlossaries();
                allStyles = await GetStyles();
            }

            foreach (var languagePair in LanguagePairs)
            {
                var sourceLangCode = languagePair.GetSourceLanguageCode();
                var targetLangCode = languagePair.GetTargetLanguageCode();

                var languageSavedOptions =
                    Options.LanguagePairOptions?.FirstOrDefault(lpo => lpo.LanguagePair.Equals(languagePair));

                var selectedGlossary = GetSelectedGlossaryFromSavedSetting(glossaries, languageSavedOptions, sourceLangCode, targetLangCode);
                var selectedStyle = allStyles.FirstOrDefault(s => s.ID == languageSavedOptions?.SelectedStyle?.ID);

                var currentLanguageGlossaries = glossaries?.Where(g =>
                    g.SourceLanguage == sourceLangCode && g.TargetLanguage == targetLangCode ||
                    g.Name == PluginResources.NoGlossary).ToList();

                var targetCulture = new CultureInfo(languagePair.TargetCulture);
                var ietfLanguageTag = targetCulture.IetfLanguageTag.ToLowerInvariant();
                var twoLetterIso = targetCulture.TwoLetterISOLanguageName.ToLowerInvariant();

                var currentLanguageStyles = allStyles.Where(style =>
                    style.Language == ietfLanguageTag || style.Language == twoLetterIso ||
                    style.Name == PluginResources.NoStyle).ToList();

                var newLanguagePairOptions = new LanguagePairOptions
                {
                    Formality = languageSavedOptions?.Formality ?? Formality.Default,
                    Glossaries =
                        currentLanguageGlossaries,
                    SelectedGlossary = selectedGlossary,
                    LanguagePair = languagePair,
                    SelectedStyle = selectedStyle,
                    Styles = currentLanguageStyles
                };

                var oldLanguagePairOption = LanguagePairOptions.FirstOrDefault(lpo => lpo.LanguagePair.Equals(languagePair));

                LanguagePairOptions.Remove(oldLanguagePairOption);
                LanguagePairOptions.Add(newLanguagePairOptions);
            }
        }

        private static GlossaryInfo GetSelectedGlossaryFromSavedSetting(List<GlossaryInfo> glossaries, LanguagePairOptions languageSavedOptions, string sourceLangCode, string targetLangCode)
        {
            var glossaryId = languageSavedOptions == null
                ? GlobalSettings.GetLastUsedGlossaryId((sourceLangCode, targetLangCode))
                : languageSavedOptions.SelectedGlossary?.Id;

            return string.IsNullOrWhiteSpace(glossaryId)
                ? glossaries.FirstOrDefault(g => g.Name == PluginResources.NoGlossary)
                : glossaries.FirstOrDefault(g => g.Id == glossaryId);
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

        private void DetachEvents()
        {
            PasswordChangedTimer.Elapsed -= OnPasswordChanged;
            //DeepLTranslationProviderClient.ApiKeyChanged -= Dispatcher_LoadLanguagePairSettings;
        }

        private void Dispatcher_LoadLanguagePairSettings() =>
                    Application.Current.Dispatcher.Invoke(LoadLanguagePairSettings);

        private SplitSentences GetDefaultSplitSentences(TagFormat tagFormat)
        => tagFormat == TagFormat.None
            ? SplitSentences.Default
            : SplitSentences.NoNewlines;

        private async Task<List<GlossaryInfo>> GetGlossaries()
        {
            var (success, glossaries, message) =
                await GlossaryClient.GetGlossaries(DeepLTranslationProviderClient.ApiKey);
            if (!success)
            {
                HandleError(message);
                glossaries = [];
            }

            glossaries?.Add(GlossaryInfo.NoGlossary);
            return glossaries;
        }

        private async Task<List<DeepLStyle>> GetStyles()
        {
            var styles =  await StyleClient.GetStyles(ApiKey).ConfigureAwait(false) ?? [];
            styles.Add(DeepLStyle.NoStyle);
            return styles;
        }

        private void HandleError(string message, [CallerMemberName] string failingMethod = null)
        {
            ValidationMessages = message;
        }

        private void LoadCredentialSettings(TranslationProviderCredential credentialStore)
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

        private void OnPasswordChanged(object sender, EventArgs e)
        {
            DeepLTranslationProviderClient.ApiVersion = ApiVersion;
            DeepLTranslationProviderClient.ApiKey = ApiKey;
            GlossaryClient.ApiVersion = ApiVersion;

            SetApiKeyValidityLabel();
            Dispatcher_LoadLanguagePairSettings();
        }

        private void Save()
        {
            DeepLTranslationProviderClient.ApiVersion = ApiVersion;
            DeepLTranslationProviderClient.ApiKey = ApiKey;
            SetApiKeyValidityLabel();

            Options.SendPlainText = SendPlainText;
            Options.ResendDraft = ResendDraft;
            Options.ApiKey = ApiKey;
            Options.LanguagePairOptions = [.. LanguagePairOptions];
            Options.PreserveFormatting = PreserveFormatting;
            Options.TagHandling = TagType;
            Options.SplitSentenceHandling = SplitSentencesType;
            Options.ApiVersion = ApiVersion;
            Options.IgnoreTagsParameter = IgnoreTags;
            Options.ModelType = ModelType;

            var glossaryIds = Options.LanguagePairOptions.ToDictionary(
                lpo => (lpo.LanguagePair.SourceCulture.Name, lpo.LanguagePair.TargetCulture.Name),
                lpo => lpo.SelectedGlossary?.Id);
            GlobalSettings.SetLastUsedGlossaryIds(glossaryIds);

            DetachEvents();

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

        private void SetValidationBlockMessage(string message = null)
        {
            ApiKeyValidationMessage = message;
        }
    }
}