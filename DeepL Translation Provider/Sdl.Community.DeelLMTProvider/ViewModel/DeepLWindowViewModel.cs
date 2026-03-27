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
            var validationInfos = new List<string>(); // For informational messages about language fallbacks
            var languagePair = options.LanguagePair;

            try
            {
                // Get DeepL language codes with fallback information - pass source/target info
                var (sourceLanguageCode, isSourceFallback, sourceFallbackMessage) = GetDeepLLanguageCodeWithFallbackInfo(languagePair.SourceCulture, isSourceLanguage: true);
                var (targetLanguageCode, isTargetFallback, targetFallbackMessage) = GetDeepLLanguageCodeWithFallbackInfo(languagePair.TargetCulture, isSourceLanguage: false);

                // Add fallback information messages
                if (isSourceFallback && !string.IsNullOrEmpty(sourceFallbackMessage))
                {
                    validationInfos.Add($"ℹ️ {sourceFallbackMessage}");
                }
                if (isTargetFallback && !string.IsNullOrEmpty(targetFallbackMessage))
                {
                    validationInfos.Add($"ℹ️ {targetFallbackMessage}");
                }

                // Validate source language support for translate_text product
                var isSourceSupported = await LanguageClientV3.IsLanguageSupportedAsync(
                    sourceLanguageCode, "source", ApiKey, Constants.BaseUrlV3, "translate_text");

                if (!isSourceSupported)
                {
                    validationErrors.Add($"Source language '{languagePair.SourceCulture}' ({sourceLanguageCode}) is not supported by DeepL V3 API for text translation.");
                }

                // Validate target language support for translate_text product
                var isTargetSupported = await LanguageClientV3.IsLanguageSupportedAsync(
                    targetLanguageCode, "target", ApiKey, Constants.BaseUrlV3, "translate_text");

                if (!isTargetSupported)
                {
                    validationErrors.Add($"Target language '{languagePair.TargetCulture}' ({targetLanguageCode}) is not supported by DeepL V3 API for text translation.");
                }

                // If both languages are supported, validate specific settings
                if (isSourceSupported && isTargetSupported)
                {
                    await ValidateLanguageSpecificSettings(options, sourceLanguageCode, targetLanguageCode, validationErrors);
                }

                // Combine validation errors and info messages
                var allMessages = new List<string>();
                allMessages.AddRange(validationInfos); // Add info messages first
                allMessages.AddRange(validationErrors); // Then add errors

                // Store validation results
                if (allMessages.Any())
                {
                    _languagePairValidationErrors[languagePair] = allMessages;
                }
                else
                {
                    _languagePairValidationErrors.Remove(languagePair);
                }
            }
            catch (Exception ex)
            {
                validationErrors.Add($"Unable to validate language pair settings: {ex.Message}");
                _languagePairValidationErrors[languagePair] = validationErrors;
            }
        }

        private async Task ValidateLanguageSpecificSettings(LanguagePairOptions options, string sourceLanguageCode, string targetLanguageCode, List<string> validationErrors)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DeepL Validation] Validating source: '{sourceLanguageCode}', target: '{targetLanguageCode}'");

                // Get target language metadata to check formality support for translate_text
                var targetLanguageInfo = await LanguageClientV3.GetLanguageV3InfoAsync(
                    targetLanguageCode, "translate_text", ApiKey, Constants.BaseUrlV3);

                // Validate formality setting
                if (options.Formality != Formality.Not_Supported && options.Formality != Formality.Default)
                {
                    if (targetLanguageInfo?.Features?.Contains("formality") != true)
                    {
                        validationErrors.Add($"Formality settings are not supported for target language '{options.LanguagePair.TargetCulture}' in DeepL V3 API.");
                    }
                }

                // Validate model type setting (check if advanced features are supported)
                if (options.ModelType != ModelType.Not_Supported && options.ModelType != ModelType.Prefer_Quality_Optimized)
                {
                    var sourceLanguageInfo = await LanguageClientV3.GetLanguageV3InfoAsync(
                        sourceLanguageCode, "translate_text", ApiKey, Constants.BaseUrlV3);

                    // Check if either source or target doesn't support advanced features like tag_handling
                    if (sourceLanguageInfo?.Features?.Contains("tag_handling") != true || 
                        targetLanguageInfo?.Features?.Contains("tag_handling") != true)
                    {
                        validationErrors.Add($"Advanced model types are not supported for this language pair in DeepL V3 API. Only quality-optimized models are available.");
                    }
                }

                // Validate glossary compatibility
                if (options.SelectedGlossary != null && 
                    options.SelectedGlossary.Name != PluginResources.NoGlossary && 
                    options.SelectedGlossary != GlossaryInfo.NotSupported)
                {
                    // Check if glossaries are supported for this language pair using the glossary product
                    var sourceGlossaryInfo = await LanguageClientV3.GetLanguageV3InfoAsync(
                        sourceLanguageCode, "glossary", ApiKey, Constants.BaseUrlV3);
                    var targetGlossaryInfo = await LanguageClientV3.GetLanguageV3InfoAsync(
                        targetLanguageCode, "glossary", ApiKey, Constants.BaseUrlV3);

                    if (!sourceGlossaryInfo?.UsableAsSource == true || !targetGlossaryInfo?.UsableAsTarget == true)
                    {
                        validationErrors.Add($"Glossaries are not supported for this language pair '{options.LanguagePair.SourceCulture}' → '{options.LanguagePair.TargetCulture}' in DeepL V3 API.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DeepL Validation] Error validating language codes '{sourceLanguageCode}' / '{targetLanguageCode}': {ex.Message}");
                validationErrors.Add($"Unable to validate language-specific settings: {ex.Message}");
            }
        }

        private (string languageCode, bool isFallback, string fallbackMessage) GetDeepLLanguageCodeWithFallbackInfo(Sdl.Core.Globalization.CultureCode cultureCode, bool isSourceLanguage = false)
        {
            // Convert culture code to DeepL language code format  
            var cultureName = cultureCode.Name.ToUpperInvariant();
            var regionNeutralName = cultureCode.RegionNeutralName.ToLowerInvariant(); // DeepL typically uses lowercase

            // For source languages, DeepL V3 API often only supports generic language codes
            // For target languages, it supports specific variants
            var (result, isFallback, fallbackMessage) = cultureName switch
            {
                // English variants - only supported as target languages
                "EN-US" when !isSourceLanguage => ("en-US", false, null),
                "EN-GB" when !isSourceLanguage => ("en-GB", false, null),
                "EN-US" or "EN-GB" when isSourceLanguage => 
                    ("en", true, $"English variant '{cultureName}' will use generic 'en' for source language (variants only supported as target)"),

                // Portuguese variants - only supported as target languages
                "PT-BR" when !isSourceLanguage => ("pt-BR", false, null),
                "PT-PT" when !isSourceLanguage => ("pt-PT", false, null),
                "PT-BR" or "PT-PT" when isSourceLanguage => 
                    ("pt", true, $"Portuguese variant '{cultureName}' will use generic 'pt' for source language (variants only supported as target)"),

                // Spanish variants
                "ES-419" when !isSourceLanguage => ("es-419", false, null),
                "ES-419" when isSourceLanguage => 
                    ("es", true, "Latin American Spanish (es-419) will use generic Spanish (es) for source language"),

                // Chinese variants - only supported as target languages
                "ZH-CN" or "ZH-HANS" when !isSourceLanguage => ("zh-Hans", false, null),
                "ZH-TW" or "ZH-HANT" when !isSourceLanguage => ("zh-Hant", false, null),
                "ZH-CN" or "ZH-HANS" or "ZH-TW" or "ZH-HANT" when isSourceLanguage => 
                    ("zh", true, $"Chinese variant '{cultureName}' will use generic 'zh' for source language (variants only supported as target)"),

                // For regional variants that don't have specific DeepL support, fall back to generic
                _ when cultureName != regionNeutralName.ToUpperInvariant() => 
                    (regionNeutralName, true, $"Regional variant '{cultureCode.Name}' will use generic '{regionNeutralName}' language code"),

                // Generic languages use as-is
                _ => (regionNeutralName, false, null)
            };

            // Debug logging to help identify conversion issues
            System.Diagnostics.Debug.WriteLine($"[DeepL] Converting culture '{cultureCode.Name}' (regionNeutral: '{cultureCode.RegionNeutralName}') to DeepL code: '{result}' (fallback: {isFallback}, isSource: {isSourceLanguage})");

            return (result, isFallback, fallbackMessage);
        }

        private string GetDeepLLanguageCode(Sdl.Core.Globalization.CultureCode cultureCode, bool isSourceLanguage = false)
        {
            return GetDeepLLanguageCodeWithFallbackInfo(cultureCode, isSourceLanguage).languageCode;
        }

        private void UpdateValidationMessages()
        {
            if (!_languagePairValidationErrors.Any())
            {
                ValidationMessages = null;
                return;
            }

            var messageBuilder = new StringBuilder();

            // Check if we have any actual errors (not just informational messages)
            bool hasActualErrors = _languagePairValidationErrors.Values
                .Any(messages => messages.Any(msg => !msg.StartsWith("ℹ️")));

            if (hasActualErrors)
            {
                messageBuilder.AppendLine("⚠️ Language Pair Compatibility Issues:");
            }
            else
            {
                messageBuilder.AppendLine("ℹ️ Language Configuration Information:");
            }

            messageBuilder.AppendLine();

            foreach (var kvp in _languagePairValidationErrors)
            {
                var languagePair = kvp.Key;
                var messages = kvp.Value;

                messageBuilder.AppendLine($"• {languagePair.SourceCulture} → {languagePair.TargetCulture}:");

                // Separate info messages from errors
                var infoMessages = messages.Where(msg => msg.StartsWith("ℹ️")).ToList();
                var errorMessages = messages.Where(msg => !msg.StartsWith("ℹ️")).ToList();

                // Display info messages first
                foreach (var info in infoMessages)
                {
                    messageBuilder.AppendLine($"  {info}");
                }

                // Then display errors
                foreach (var error in errorMessages)
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