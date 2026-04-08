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
        private ObservableCollection<LanguagePairOptions> _languagePairOptions = new();
        private Dictionary<LanguagePair, List<string>> _languagePairValidationErrors = new();

        public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, IDeepLGlossaryClient glossaryClient, IMessageService messageService, ILanguageValidationService languageValidationService)
        {
            IsTellMeAction = true;
            MessageService = messageService;
            GlossaryClient = glossaryClient;
            LanguageValidationService = languageValidationService;

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

        public DeepLWindowViewModel(DeepLTranslationOptions deepLTranslationOptions, IDeepLGlossaryClient glossaryClient, TranslationProviderCredential credentialStore, LanguagePair[] languagePairs, IMessageService messageService, ILanguageValidationService languageValidationService)
        {
            MessageService = messageService;
            LanguagePairs = languagePairs;
            GlossaryClient = glossaryClient;
            LanguageValidationService = languageValidationService;
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
            get;
            set => SetField(ref field, value);
        }

        public bool ResendDraft
        {
            get;
            set => SetField(ref field, value);
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

        public bool HasValidationErrors
        {
            get;
            set => SetField(ref field, value);
        }

        public string ValidationIssues
        {
            get;
            set => SetField(ref field, value);
        }

        public string ValidationMessages
        {
            get;
            set => SetField(ref field, value);
        }

        public string ValidationMessagePreview
        {
            get;
            set => SetField(ref field, value);
        }

        public string ValidationNotes
        {
            get;
            set => SetField(ref field, value);
        }

        public bool IsValidating
        {
            get;
            set => SetField(ref field, value);
        }

        private IDeepLGlossaryClient GlossaryClient { get; set; }
        private bool IsTellMeAction { get; }
        private ILanguageValidationService LanguageValidationService { get; set; }
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
                HasValidationErrors = false;
                ValidationIssues = null;
                ValidationNotes = null;
                ValidationMessagePreview = null;
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

                System.Diagnostics.Debug.WriteLine($"[DeepL] Language pair: {languagePair.SourceCulture} -> {languagePair.TargetCulture}, targetLangCode: '{targetLangCode}'");
                System.Diagnostics.Debug.WriteLine($"[DeepL] All styles count: {allStyles.Count}, filtered styles for this language: {currentLanguageStyles.Count}");
                foreach (var style in currentLanguageStyles)
                {
                    System.Diagnostics.Debug.WriteLine($"[DeepL]   - Style: '{style.Name}', Language: '{style.Language}'");
                }

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

            var languagePair = options.LanguagePair;

            try
            {
                var result = await LanguageValidationService.ValidateAsync(languagePair, ApiKey, Constants.BaseUrlV3);

                var allMessages = result.Messages.ToList();
                if (result.IsSourceLanguageSupported && result.IsTargetLanguageSupported)
                    allMessages.AddRange(options.Apply(result));

                if (allMessages.Any())
                    _languagePairValidationErrors[languagePair] = allMessages;
                else
                    _languagePairValidationErrors.Remove(languagePair);
            }
            catch (Exception ex)
            {
                options.ResetToUnsupported();
                _languagePairValidationErrors[languagePair] = [$"Unable to validate language pair settings: {ex.Message}"];
            }
        }

        private void UpdateValidationMessages()
        {
            if (!_languagePairValidationErrors.Any())
            {
                HasValidationErrors = false;
                ValidationIssues = null;
                ValidationNotes = null;
                ValidationMessages = null;
                ValidationMessagePreview = null;
                return;
            }

            var issuesBuilder = new StringBuilder();
            var notesBuilder = new StringBuilder();

            foreach (var kvp in _languagePairValidationErrors)
            {
                var languagePair = kvp.Key;
                var messages = kvp.Value;

                var infoMessages = messages.Where(msg => msg.StartsWith("ℹ️")).ToList();
                var errorMessages = messages.Where(msg => !msg.StartsWith("ℹ️")).ToList();

                if (errorMessages.Any())
                {
                    issuesBuilder.AppendLine($"• {languagePair.SourceCulture} → {languagePair.TargetCulture}:");
                    foreach (var error in errorMessages)
                        issuesBuilder.AppendLine($"  - {error}");
                    issuesBuilder.AppendLine();
                }

                if (infoMessages.Any())
                {
                    notesBuilder.AppendLine($"• {languagePair.SourceCulture} → {languagePair.TargetCulture}:");
                    foreach (var info in infoMessages)
                        notesBuilder.AppendLine($"  {info}");
                    notesBuilder.AppendLine();
                }
            }

            HasValidationErrors = issuesBuilder.Length > 0;
            ValidationIssues = issuesBuilder.Length > 0 ? issuesBuilder.ToString().Trim() : null;
            ValidationNotes = notesBuilder.Length > 0 ? notesBuilder.ToString().Trim() : null;
            ValidationMessages = ValidationIssues ?? ValidationNotes;

            var errorPairCount = _languagePairValidationErrors.Values.Count(msgs => msgs.Any(m => !m.StartsWith("\u2139\ufe0f")));
            var infoPairCount = _languagePairValidationErrors.Values.Count(msgs => msgs.All(m => m.StartsWith("\u2139\ufe0f")));

            var previewParts = new List<string>();
            if (errorPairCount > 0)
                previewParts.Add($"\u26a0\ufe0f {errorPairCount} pair{(errorPairCount > 1 ? "s" : "")} with issues");
            if (infoPairCount > 0)
                previewParts.Add($"\u2139\ufe0f {infoPairCount} pair{(infoPairCount > 1 ? "s" : "")} with notes");

            ValidationMessagePreview = string.Join(", ", previewParts) + " \u2014 click to view";
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