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
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
    public class DeepLWindowViewModel : ViewModel
    {
        private string _apiKey;
        private string _apiKeyValidationMessage;
        private string _apiVersion;
        private List<string> _ignoreTags;
        private ObservableCollection<LanguagePairOptions> _languagePairOptions = new();
        private bool _preserveFormatting;
        private bool _resendDraft;
        private bool _sendPlainText;
        private SplitSentences _splitSentencesType;
        private TagFormat _tagType;
        private string _validationMessages;
        private Dictionary<LanguagePair, List<string>> _languagePairValidationErrors = new();
        private bool _isValidating;

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

            PasswordChangedTimer.Elapsed += OnPasswordChanged;

            LoadCredentialSettings(credentialStore);
            //DeepLTranslationProviderClient.ApiKeyChanged += Dispatcher_LoadLanguagePairSettings;
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

        public string ApiVersion
        {
            get => _apiVersion;
            set
            {
                SetField(ref _apiVersion, value);
                OnPasswordChanged(null, null);
            }
        }

        public ICommand CancelCommand => new ParameterlessCommand(DetachEvents);

        public List<string> IgnoreTags
        {
            get => _ignoreTags;
            set => SetField(ref _ignoreTags, value);
        }

        public ObservableCollection<LanguagePairOptions> LanguagePairOptions
        {
            get => _languagePairOptions;
            set 
            {
                SetField(ref _languagePairOptions, value);

                // Subscribe to changes in language pair options
                if (_languagePairOptions != null)
                {
                    foreach (var option in _languagePairOptions)
                    {
                        option.PropertyChanged += OnLanguagePairOptionChanged;
                    }
                    _languagePairOptions.CollectionChanged += (sender, e) =>
                    {
                        if (e.NewItems != null)
                        {
                            foreach (LanguagePairOptions newOption in e.NewItems)
                            {
                                newOption.PropertyChanged += OnLanguagePairOptionChanged;
                            }
                        }
                        if (e.OldItems != null)
                        {
                            foreach (LanguagePairOptions oldOption in e.OldItems)
                            {
                                oldOption.PropertyChanged -= OnLanguagePairOptionChanged;
                            }
                        }
                    };
                }
            }
        }

        public ICommand ManageGlossariesCommand => new ParameterlessCommand(() => ManageGlossaries?.Invoke(), () => ApiKeyValidationMessage == null);

        
        public ICommand OkCommand => new ParameterlessCommand(Save, () => ApiKeyValidationMessage == null);

        public DeepLTranslationOptions Options { get; set; }

        public bool PreserveFormatting
        {
            get => _preserveFormatting;
            set => SetField(ref _preserveFormatting, value);
        }

        public bool ResendDraft
        {
            get => _resendDraft;
            set => SetField(ref _resendDraft, value);
        }

        public bool SendPlainText
        {
            get => _sendPlainText;
            set => SetField(ref _sendPlainText, value);
        }

        public SplitSentences SplitSentencesType
        {
            get => _splitSentencesType;
            set => SetField(ref _splitSentencesType, value);
        }

        public TagFormat TagType
        {
            get => _tagType;
            // Add a method here to update the splitsentencestype
            set
            {
                SetField(ref _tagType, value);
                SplitSentencesType = GetDefaultSplitSentences(value);
            }
        }

        public string Title { get; set; } = "DeepL Translation Provider";

        public string ValidationMessages
        {
            get => _validationMessages;
            set => SetField(ref _validationMessages, value);
        }

        public bool IsValidating
        {
            get => _isValidating;
            set => SetField(ref _isValidating, value);
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
            IsValidating = true;
            _languagePairValidationErrors.Clear();

            List<GlossaryInfo> glossaries = [];
            List<DeepLStyle> allStyles = [];

            if (DeepLTranslationProviderClient.IsApiKeyValidResponse.IsSuccessStatusCode)
            {
                glossaries = await GetGlossaries();
                allStyles = await GetStyles();
            }

            if (ApiKey is null) 
            {
                IsValidating = false;
                return;
            }

            foreach (var languagePair in LanguagePairs)
            {
                var sourceLangCode = languagePair.SourceCulture.RegionNeutralName.ToLowerInvariant();
                var targetLangCode = languagePair.TargetCulture.RegionNeutralName.ToLowerInvariant();

                var languageSavedOptions =
                    Options.LanguagePairOptions?.FirstOrDefault(lpo => lpo.LanguagePair.Equals(languagePair));

                var currentLanguageGlossaries = glossaries?.Where(g =>
                    g.SourceLanguage == sourceLangCode && g.TargetLanguage == targetLangCode ||
                    g.Name == PluginResources.NoGlossary).ToList();

                var selectedGlossary = currentLanguageGlossaries is not null
                    ? GetSelectedGlossaryFromSavedSetting(currentLanguageGlossaries, languageSavedOptions,
                        sourceLangCode,
                        targetLangCode)
                    : GlossaryInfo.NotSupported;

                var currentLanguageStyles = allStyles.Where(style =>
                        targetLangCode == style.Language?.ToLowerInvariant() || style.Name == PluginResources.NoStyle)
                    .ToList();

                var selectedStyle = currentLanguageStyles.FirstOrDefault(s => s.ID == languageSavedOptions?.SelectedStyle?.ID);

                var formality = DeepLTranslationProviderClient.SupportsFormality(languagePair.TargetCulture)
                    ? languageSavedOptions?.Formality ?? Formality.Default
                    : Formality.Not_Supported;

                var modelType = DeepLTranslationProviderClient.SupportsAllModelTypes(languagePair)
                    ? languageSavedOptions?.ModelType ?? ModelType.Prefer_Quality_Optimized
                    : ModelType.Not_Supported;

                var newLanguagePairOptions = new LanguagePairOptions
                {
                    Formality = formality,
                    Glossaries = currentLanguageGlossaries,
                    SelectedGlossary = selectedGlossary,
                    LanguagePair = languagePair,
                    SelectedStyle = selectedStyle,
                    Styles = currentLanguageStyles,
                    ModelType = modelType
                };

                // Validate language pair settings using V3 API
                await ValidateLanguagePairSettings(newLanguagePairOptions);

                var oldLanguagePairOption = LanguagePairOptions.FirstOrDefault(lpo => lpo.LanguagePair.Equals(languagePair));

                LanguagePairOptions.Remove(oldLanguagePairOption);
                LanguagePairOptions.Add(newLanguagePairOptions);
            }

            // Update overall validation message
            UpdateValidationMessages();
            IsValidating = false;
        }

        private async Task ValidateLanguagePairSettings(LanguagePairOptions options)
        {
            if (string.IsNullOrEmpty(ApiKey) || options?.LanguagePair == null)
                return;

            var validationErrors = new List<string>();
            var languagePair = options.LanguagePair;

            try
            {
                // Get DeepL language codes (uppercase format)
                var sourceLanguageCode = GetDeepLLanguageCode(languagePair.SourceCulture);
                var targetLanguageCode = GetDeepLLanguageCode(languagePair.TargetCulture);

                // Validate source language support
                var isSourceSupported = await LanguageClientV3.IsLanguageSupportedAsync(
                    sourceLanguageCode, "source", ApiKey, Constants.BaseUrlV3);

                if (!isSourceSupported)
                {
                    validationErrors.Add($"Source language '{languagePair.SourceCulture}' ({sourceLanguageCode}) is not supported by DeepL V3 API.");
                }

                // Validate target language support  
                var isTargetSupported = await LanguageClientV3.IsLanguageSupportedAsync(
                    targetLanguageCode, "target", ApiKey, Constants.BaseUrlV3);

                if (!isTargetSupported)
                {
                    validationErrors.Add($"Target language '{languagePair.TargetCulture}' ({targetLanguageCode}) is not supported by DeepL V3 API.");
                }

                // If both languages are supported, validate specific settings
                if (isSourceSupported && isTargetSupported)
                {
                    await ValidateLanguageSpecificSettings(options, sourceLanguageCode, targetLanguageCode, validationErrors);
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add($"Unable to validate language pair settings: {ex.Message}");
            }

            // Store validation results
            if (validationErrors.Any())
            {
                _languagePairValidationErrors[languagePair] = validationErrors;
            }
            else
            {
                _languagePairValidationErrors.Remove(languagePair);
            }
        }

        private async Task ValidateLanguageSpecificSettings(LanguagePairOptions options, string sourceLanguageCode, string targetLanguageCode, List<string> validationErrors)
        {
            try
            {
                // Get target language metadata to check formality support
                var targetLanguageMetadata = await LanguageClientV3.GetLanguageMetadataAsync(
                    targetLanguageCode, "target", ApiKey, Constants.BaseUrlV3);

                // Validate formality setting
                if (options.Formality != Formality.Not_Supported && options.Formality != Formality.Default)
                {
                    if (targetLanguageMetadata?.SupportsOptions != true)
                    {
                        validationErrors.Add($"Formality settings are not supported for target language '{options.LanguagePair.TargetCulture}' in DeepL V3 API.");
                    }
                }

                // Validate model type setting
                if (options.ModelType != ModelType.Not_Supported && options.ModelType != ModelType.Prefer_Quality_Optimized)
                {
                    var sourceLanguageMetadata = await LanguageClientV3.GetLanguageMetadataAsync(
                        sourceLanguageCode, "source", ApiKey, Constants.BaseUrlV3);

                    if (sourceLanguageMetadata?.SupportsOptions != true || targetLanguageMetadata?.SupportsOptions != true)
                    {
                        validationErrors.Add($"Advanced model types are not supported for this language pair in DeepL V3 API. Only quality-optimized models are available.");
                    }
                }

                // Validate glossary compatibility
                if (options.SelectedGlossary != null && 
                    options.SelectedGlossary.Name != PluginResources.NoGlossary && 
                    options.SelectedGlossary != GlossaryInfo.NotSupported)
                {
                    // Check if glossaries are supported for this language pair
                    if (targetLanguageMetadata?.SupportsOptions != true)
                    {
                        validationErrors.Add($"Glossaries are not supported for target language '{options.LanguagePair.TargetCulture}' in DeepL V3 API.");
                    }
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add($"Unable to validate language-specific settings: {ex.Message}");
            }
        }

        private string GetDeepLLanguageCode(Sdl.Core.Globalization.CultureCode cultureCode)
        {
            // Convert culture code to DeepL language code format  
            var cultureName = cultureCode.Name.ToUpperInvariant();
            var regionNeutralName = cultureCode.RegionNeutralName.ToUpperInvariant();

            // Handle specific language variants that DeepL recognizes
            return cultureName switch
            {
                "EN-US" => "EN-US",
                "EN-GB" => "EN-GB", 
                "PT-BR" => "PT-BR",
                "PT-PT" => "PT-PT",
                "ES-419" => "ES-419",
                "ZH-CN" or "ZH-HANS" => "ZH-HANS",
                "ZH-TW" or "ZH-HANT" => "ZH-HANT",
                _ => regionNeutralName
            };
        }

        private void UpdateValidationMessages()
        {
            if (!_languagePairValidationErrors.Any())
            {
                ValidationMessages = null;
                return;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("⚠️ Language Pair Compatibility Issues:");
            messageBuilder.AppendLine();

            foreach (var kvp in _languagePairValidationErrors)
            {
                var languagePair = kvp.Key;
                var errors = kvp.Value;

                messageBuilder.AppendLine($"• {languagePair.SourceCulture} → {languagePair.TargetCulture}:");
                foreach (var error in errors)
                {
                    messageBuilder.AppendLine($"  - {error}");
                }
                messageBuilder.AppendLine();
            }

            ValidationMessages = messageBuilder.ToString().Trim();
        }

        private async void OnLanguagePairOptionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is LanguagePairOptions option && !string.IsNullOrEmpty(ApiKey))
            {
                // Only validate specific properties that affect compatibility
                if (e.PropertyName == "Formality" ||
                    e.PropertyName == "ModelType" ||
                    e.PropertyName == "SelectedGlossary")
                {
                    await ValidateLanguagePairSettings(option);
                    UpdateValidationMessages();
                }
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

            // Unsubscribe from language pair options changes
            if (_languagePairOptions != null)
            {
                foreach (var option in _languagePairOptions)
                {
                    option.PropertyChanged -= OnLanguagePairOptionChanged;
                }
            }

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