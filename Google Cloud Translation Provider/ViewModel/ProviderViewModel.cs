using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using NLog;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace GoogleCloudTranslationProvider.ViewModel;

public class ProviderViewModel : BaseViewModel, IProviderControlViewModel
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private const string DummyLocation = "gctp-sdl";
    private readonly IOpenFileDialogService _openFileDialogService;
    private readonly ITranslationOptions _options;
    private readonly IEnumerable<LanguagePair> _languagePairs;
    private readonly bool _editProvider;

    private GoogleApiVersion _selectedGoogleApiVersion;

    private List<LanguagePairResources> _languageMappingPairs;
    private LanguagePairResources _selectedMappedPair;

    private List<string> _locations;
    private string _projectLocation;

    private string _visibleJsonPath;
    private string _jsonFilePath;
    private string _projectId;
    private string _urlToDownload;
    private string _apiKey;

    private bool _canModifyExistingFields;
    private bool _projectResourcesLoaded;
    private bool _persistGoogleKey;
    private bool _userLocalPath;

    private ICommand _switchJsonLoadingPathCommand;
    private ICommand _downloadJsonFileCommand;
    private ICommand _dragDropJsonFileCommand;
    private ICommand _browseJsonFileCommand;
    private ICommand _openLocalPathCommand;
    private ICommand _navigateToCommand;
    private ICommand _clearCommand;

    public ProviderViewModel(ITranslationOptions options, List<LanguagePair> languagePairs, bool editProvider)
    {
        ViewModel = this;
        UseLocalPath = true;
        _editProvider = editProvider;
        _options = options;
        _languagePairs = languagePairs;
        CanChangeProviderResources = string.IsNullOrEmpty(_options.ProjectId);
        _openFileDialogService = new OpenFileDialogService();
        InitializeComponent();
    }

    public BaseViewModel ViewModel { get; set; }

    public List<GoogleApiVersion> GoogleApiVersions { get; set; }

    public GoogleApiVersion SelectedGoogleApiVersion
    {
        get => _selectedGoogleApiVersion;
        set
        {
            if (_selectedGoogleApiVersion == value) return;
            _selectedGoogleApiVersion = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsV2Checked));
            OnPropertyChanged(nameof(IsV3Checked));
            SwitchViewExternal?.Execute(nameof(ProviderViewModel));
        }
    }

    public List<LanguagePairResources> LanguageMappingPairs
    {
        get => _languageMappingPairs;
        set
        {
            if (_languageMappingPairs == value) return;
            _languageMappingPairs = value;
            OnPropertyChanged();
        }
    }

    public LanguagePairResources SelectedMappedPair
    {
        get => _selectedMappedPair;
        set
        {
            if (_selectedMappedPair == value) return;
            _selectedMappedPair = value;
            OnPropertyChanged();
        }
    }

    public List<string> Locations
    {
        get => _locations ??= new();
        set
        {
            if (_locations == value) return;
            _locations = value;
            OnPropertyChanged();
        }
    }

    public string ProjectLocation
    {
        get => _projectLocation;
        set
        {
            if (_projectLocation == value) return;
            _projectLocation = value;
            OnPropertyChanged();
            GetProjectResources();
        }
    }

    public bool PersistGoogleKey
    {
        get => _persistGoogleKey;
        set
        {
            if (_persistGoogleKey == value) return;
            _persistGoogleKey = value;
            OnPropertyChanged();
            _options.PersistGoogleKey = value;
        }
    }

    public bool IsV2Checked => SelectedGoogleApiVersion.Version == ApiVersion.V2;

    public bool IsV3Checked => SelectedGoogleApiVersion.Version == ApiVersion.V3;

    public bool CanChangeProviderResources
    {
        get => _canModifyExistingFields;
        set
        {
            if (_canModifyExistingFields == value) return;
            _canModifyExistingFields = value;
            OnPropertyChanged();
        }
    }

    public bool ProjectResourcesLoaded
    {
        get => _projectResourcesLoaded;
        set
        {
            if (_projectResourcesLoaded == value) return;
            _projectResourcesLoaded = value;
            OnPropertyChanged();
        }
    }

    public string VisibleJsonPath
    {
        get => _visibleJsonPath ?? PluginResources.ProviderViewModel_PathNotSpecified;
        set
        {
            if (_visibleJsonPath == value) return;
            _visibleJsonPath = value;
            OnPropertyChanged();
        }
    }

    public string JsonFilePath
    {
        get => _jsonFilePath;
        set
        {
            if (_jsonFilePath == value) return;
            _jsonFilePath = value;
            OnPropertyChanged();
        }
    }

    public string ProjectId
    {
        get => _projectId ?? PluginResources.ProviderViewModel_PathNotSpecified;
        set
        {
            if (_projectId == value) return;
            _projectId = value;
            OnPropertyChanged();
        }
    }

    public string ApiKey
    {
        get => _apiKey;
        set
        {
            if (_apiKey == value) return;
            _apiKey = value.Trim();
            OnPropertyChanged();
        }
    }

    public bool UseLocalPath
    {
        get => _userLocalPath;
        set
        {
            if (_userLocalPath == value) return;
            _userLocalPath = value;
            OnPropertyChanged();
        }
    }

    public string UrlToDownload
    {
        get => _urlToDownload;
        set
        {
            if (_urlToDownload == value) return;
            _urlToDownload = value;
            OnPropertyChanged();
        }
    }

    private void ResetFields()
    {
        JsonFilePath = null;
        Locations = new();
        ProjectId = null;
        ProjectLocation = null;
        ProjectResourcesLoaded = false;
        UrlToDownload = null;
        VisibleJsonPath = null;
    }

    public ICommand SwitchJsonLoadingPathCommand => _switchJsonLoadingPathCommand ??= new RelayCommand(action => { UseLocalPath = !UseLocalPath; });

    public ICommand DragDropJsonFileCommand => _dragDropJsonFileCommand ??= new RelayCommand(DragAndDropJsonFile);
    public ICommand DownloadJsonFileCommand => _downloadJsonFileCommand ??= new RelayCommand(DownloadJsonFile);
    public ICommand BrowseJsonFileCommand => _browseJsonFileCommand ??= new RelayCommand(BrowseJsonFile);

    public ICommand OpenLocalPathCommand => _openLocalPathCommand ??= new RelayCommand(OpenLocalPath);
    public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);
    public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

    public ICommand SwitchViewExternal { get; set; }

    public event EventHandler LanguageMappingLoaded;

    public async Task<bool> CanConnectToGoogleV2(HtmlUtil htmlUtil)
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            ErrorHandler.HandleError(PluginResources.Validation_ApiKey, "Invalid key");
            return false;
        }

        try
        {
            var v2Connector = new V2Connector(ApiKey, htmlUtil);
            var canConnect = v2Connector.CredentialsAreValid();

            if (!canConnect)
                return canConnect;

            _options.V2SupportedLanguages = await v2Connector.GetLanguages();
            _options.ApiKey ??= ApiKey;
            var database = DatabaseExtensions.CreateDatabase(_options);
            foreach (var languagePair in _languagePairs)
            {
                var sourceLm = database.GetLanguage(languagePair.SourceCulture);
                var targetLm = database.GetLanguage(languagePair.TargetCulture);
                if (string.IsNullOrEmpty(sourceLm.LanguageCode)
                 || string.IsNullOrEmpty(targetLm.LanguageCode))
                {
                    var dialogResult = MessageBox.Show(
                        "Warning: One or more language pairs might not be fully supported or the language code was not set. Please set a valid language code using the Language Mapping button before performing language-specific operations.",
                        "Invalid language pair",
                        MessageBoxButtons.RetryCancel);
                    LanguageMappingLoaded?.Invoke(this, EventArgs.Empty);
                    return dialogResult != DialogResult.Retry;
                }
            }

            return canConnect;
        }
        catch (Exception e)
        {
            if (e.Message.Contains("(400) Bad Request"))
            {
                ErrorHandler.HandleError(PluginResources.Validation_ApiKey_Invalid, "API Key");
                return false;
            }

            ErrorHandler.HandleError(e);
            return false;
        }
    }

    public bool CanConnectToGoogleV3(IEnumerable<LanguagePair> languagePairs)
    {
        var optionsAreSet = GoogleV3OptionsAreSet();
        if (!optionsAreSet) return false;

        if (!LanguageMappingPairs.Any()) return true;
        foreach (var languagePair in LanguageMappingPairs)
            if (string.IsNullOrEmpty(languagePair.SourceLanguageCode)
             || string.IsNullOrEmpty(languagePair.TargetLanguageCode))
            {
                var dialogResult = MessageBox.Show(
                    "Warning: One or more language pairs might not be fully supported or the language code was not set. Please set a valid language code using the Language Mapping button before performing language-specific operations.",
                    "Invalid language pair",
                    MessageBoxButtons.RetryCancel);
                LanguageMappingLoaded?.Invoke(this, EventArgs.Empty);
                return dialogResult != DialogResult.Retry;
            }

        return optionsAreSet;
    }

    private bool GoogleV3OptionsAreSet()
    {
        if (string.IsNullOrEmpty(JsonFilePath))
        {
            ErrorHandler.HandleError(PluginResources.Validation_EmptyJsonFilePath, nameof(JsonFilePath));
            return false;
        }
        else if (!File.Exists(JsonFilePath))
        {
            ErrorHandler.HandleError(PluginResources.Validation_MissingJsonFile, nameof(JsonFilePath));
            return false;
        }
        else if (string.IsNullOrEmpty(ProjectId))
        {
            ErrorHandler.HandleError(PluginResources.Validation_ProjectID_Empty, nameof(ProjectId));
            return false;
        }
        else if (string.IsNullOrEmpty(ProjectLocation))
        {
            ErrorHandler.HandleError(PluginResources.Validation_Location_Empty, nameof(ProjectLocation));
            return false;
        }

        return true;
    }

    private void InitializeComponent()
    {
        GoogleApiVersions = new List<GoogleApiVersion>
    {
        new GoogleApiVersion
        {
            Name    = PluginResources.GoogleApiVersionV2Description,
            Version = ApiVersion.V2
        },
        new GoogleApiVersion
        {
            Name    = PluginResources.GoogleApiVersionV3Description,
            Version = ApiVersion.V3
        },
    };

        PersistGoogleKey = _options.PersistGoogleKey;
        ApiKey = PersistGoogleKey || _editProvider ? _options.ApiKey : string.Empty;
        JsonFilePath = _options.JsonFilePath;
        VisibleJsonPath = JsonFilePath.ShortenFilePath();
        ProjectId = _options.ProjectId;

        // ✅ Step 1: Set version first — needed by CreateV3Database
        SelectedGoogleApiVersion = GoogleApiVersions
            .FirstOrDefault(v => v.Version.Equals(_options.SelectedGoogleVersion))
            ?? GoogleApiVersions.First(x => x.Version == ApiVersion.V2);

        // ✅ Step 2: Populate Locations before setting ProjectLocation
        // so the ComboBox has items ready when ProjectLocation triggers
        // GetProjectResources() via the property setter
        if (!string.IsNullOrEmpty(_options.ProjectLocation))
        {
            var locations = V3ResourceManager.GetLocations(new TranslationOptions
            {
                ProjectId = _options.ProjectId,
                JsonFilePath = _options.JsonFilePath,
                ProjectLocation = DummyLocation
            });

            // GetLocations returns null on success (no unsupported location error)
            // In that case seed the list with the saved location so ComboBox isn't empty
            Locations = locations ?? new List<string> { _options.ProjectLocation };
        }

        // ✅ Step 3: NOW set ProjectLocation — triggers GetProjectResources()
        // at this point: SelectedGoogleApiVersion ✓, Locations ✓
        ProjectLocation = _options.ProjectLocation;

        if (LanguageMappingPairs is not null && LanguageMappingPairs.Any())
            ProjectResourcesLoaded = true;
    }

    private void DragAndDropJsonFile(object parameter)
    {
        if (parameter is not DragEventArgs eventArgs
         || eventArgs.Data.GetData(DataFormats.FileDrop, true) is not string[] fileDrop
         || fileDrop?.Length < 1)
            return;

        if (fileDrop.Length != 1)
        {
            ErrorHandler.HandleError(PluginResources.Validation_MultipleFiles, "Multiple files");
            return;
        }

        ReadJsonFile(fileDrop.FirstOrDefault());
    }

    private void DownloadJsonFile(object parameter)
    {
        UrlToDownload ??= string.Empty;
        var (Success, ErrorMessage) = UrlToDownload.Trim().VerifyAndDownloadJsonFile(_options.DownloadPath, Constants.DefaultDownloadedJsonFileName);
        if (!Success)
        {
            ErrorHandler.HandleError(ErrorMessage, "Download failed");
            return;
        }

        ReadJsonFile($"{_options.DownloadPath}\\{Constants.DefaultDownloadedJsonFileName}");
    }

    private void BrowseJsonFile(object parameter)
    {
        const string Browse_JsonFiles = "JSON File|*.json";
        var selectedFile = _openFileDialogService.ShowDialog(Browse_JsonFiles);
        if (!string.IsNullOrEmpty(selectedFile))
            ReadJsonFile(selectedFile);
    }

    private void ReadJsonFile(string filePath)
    {
        if (!GetJsonDetails(filePath).Success)
            return;

        var tempOptions = new TranslationOptions
        {
            ProjectId = _projectId,
            JsonFilePath = _jsonFilePath,
            ProjectLocation = DummyLocation
        };

        Locations = V3ResourceManager.GetLocations(tempOptions);
        ProjectLocation = Locations.First();
    }

    private (bool Success, object OperationResult) GetJsonDetails(string selectedFile)
    {
        var (success, operationResult) = selectedFile.VerifyPathAndReadJsonFile();
        if (!success)
        {
            ErrorHandler.HandleError(operationResult as string, "Reading failed");
            return (success, operationResult);
        }

        JsonFilePath = selectedFile;
        VisibleJsonPath = selectedFile.ShortenFilePath();
        ProjectId = (operationResult as Dictionary<string, string>)["project_id"];
        return (true, null);
    }

    private async void GetProjectResources()
    {
        if (string.IsNullOrEmpty(_projectLocation)
            || _projectLocation.Equals(DummyLocation))
            return;

        var tempOptions = new TranslationOptions
        {
            ProjectId = _projectId,
            JsonFilePath = _jsonFilePath,
            ProjectLocation = _projectLocation,
            SelectedGoogleVersion = ApiVersion.V3
        };

        // ✅ Step 1: Create DB FIRST — must exist before GetLanguageCode is called
        CreateV3Database(tempOptions);

        // ✅ Step 2: Verify DB was actually created before proceeding
        var dbPath = string.Format(Constants.DatabaseFilePath, Constants.Database_PluginName_V3);
        if (!File.Exists(dbPath))
        {
            _logger.Error($"GetProjectResources: Database was not created at '{dbPath}'. " +
                          $"Cannot build language mapping pairs.");
            return;
        }

        // Step 3: Now safe to fetch resources and resolve language codes
        var pairMapping = new List<LanguagePairResources>();
        var availableGlossaries = V3ResourceManager.GetGlossaries(tempOptions);
        var availableCustomModels = await V3ResourceManager.GetCustomModelsAsync(tempOptions);

        _logger.Info($"GetProjectResources: Processing {_languagePairs.Count()} pairs");

        for (var i = 0; i < _languagePairs.Count(); i++)
        {
            var currentPair = _languagePairs.ElementAt(i);
            var sourceCode = currentPair.SourceCulture.GetLanguageCode(ApiVersion.V3);
            var targetCode = currentPair.TargetCulture.GetLanguageCode(ApiVersion.V3);

            _logger.Info($"Pair [{i}]: {currentPair.SourceCultureName} → " +
                         $"{currentPair.TargetCultureName} | " +
                         $"Codes: '{sourceCode}' → '{targetCode}'");

            

            // ✅ Log so you can see exactly what's resolving on his machine
            _logger.Info($"Pair [{i}]: {currentPair.SourceCultureName} → {currentPair.TargetCultureName} " +
                         $"| Codes: '{sourceCode}' → '{targetCode}'");

            if (string.IsNullOrEmpty(sourceCode) || string.IsNullOrEmpty(targetCode))
            {
                _logger.Warn($"GetProjectResources: Empty language code for pair " +
                             $"{currentPair.SourceCultureName} → {currentPair.TargetCultureName}. " +
                             $"DB lookup failed — database may not exist or language not found.");
            }

            var sourceCultureName = GetCultureDisplayName(currentPair.SourceCultureName);
            var targetCultureName = GetCultureDisplayName(currentPair.TargetCultureName);

            var mapping = new LanguagePairResources()
            {
                DisplayName = $"{sourceCultureName} - {targetCultureName}",
                LanguagePair = currentPair,
                AvailableGlossaries = V3ResourceManager.GetPairGlossaries(currentPair, availableGlossaries),
                AvailableModels = V3ResourceManager.GetPairModels(currentPair, availableCustomModels),
                SourceLanguageCode = sourceCode,
                TargetLanguageCode = targetCode
            };

            if (_options.LanguageMappingPairs is null)
            {
                mapping.SelectedModel = mapping.AvailableModels.FirstOrDefault();
                mapping.SelectedGlossary = mapping.AvailableGlossaries.FirstOrDefault();
            }
            else
            {
                var model = _options.LanguageMappingPairs[i].AvailableModels.FirstOrDefault(x => x.DisplayName == _options.LanguageMappingPairs[i].SelectedModel.DisplayName);
                var modelIndex = _options.LanguageMappingPairs[i].AvailableModels.IndexOf(model);
                mapping.SelectedModel = mapping.AvailableModels.ElementAtOrDefault(modelIndex) ?? mapping.AvailableModels.FirstOrDefault();

                var glossary = _options.LanguageMappingPairs[i].AvailableGlossaries.FirstOrDefault(x => x.DisplayName == _options.LanguageMappingPairs[i].SelectedGlossary.DisplayName);
                var glossaryIndex = _options.LanguageMappingPairs[i].AvailableGlossaries.IndexOf(glossary);
                mapping.SelectedGlossary = mapping.AvailableGlossaries.ElementAtOrDefault(glossaryIndex) ?? mapping.AvailableGlossaries.FirstOrDefault();
            }

            pairMapping.Add(mapping);
        }

        LanguageMappingPairs = pairMapping;
        ProjectResourcesLoaded = true;

        _logger.Info($"GetProjectResources: Completed with {pairMapping.Count} mapped pairs");
    }

    private static string GetCultureDisplayName(string cultureName)
    {
        try
        {
            return new CultureInfo(cultureName).EnglishName;
        }
        catch (CultureNotFoundException)
        {
            _logger.Warn($"GetCultureDisplayName: Culture '{cultureName}' " +
                      $"not recognized by OS — falling back to raw name. " +
                      $"This may occur on Windows Server with limited culture support.");
            return cultureName; // graceful fallback to raw string
        }
    }

    private void OpenLocalPath(object parameter)
    {
        var (Success, ErrorMessage) = JsonFilePath.OpenFolderAndSelectFile();
        if (!Success)
            ErrorHandler.HandleError(ErrorMessage, "Authentication failed");
    }

    private void NavigateTo(object parameter)
    {
        if (parameter is not string target)
            return;

        var query = IsV2Checked || target.Contains("cloud-resource-manager") ? string.Empty : $"?project={_projectId}";
        if (!string.IsNullOrEmpty(target))
            Process.Start($"{target}{query}");
    }

    private void Clear(object parameter)
    {
        switch (parameter as string)
        {
            case nameof(UrlToDownload):
                UrlToDownload = string.Empty;
                ResetFields();
                break;
        }
    }

    private void CreateV3Database(TranslationOptions translationOptions)
    {
        var dbFilePath = string.Format(
            Constants.DatabaseFilePath,
            Constants.Database_PluginName_V3);

        try
        {
            // ✅ Use what was passed in — don't rely on VM properties
            // that may not be initialized yet during startup
            translationOptions.SelectedGoogleVersion = ApiVersion.V3;
            translationOptions.ProjectLocation = translationOptions.ProjectLocation
                                                 ?? _projectLocation;
            translationOptions.LanguageMappingPairs = translationOptions.LanguageMappingPairs
                                                      ?? LanguageMappingPairs
                                                      ?? new List<LanguagePairResources>();

            var db = DatabaseExtensions.CreateDatabase(translationOptions);
            if (db is null)
            {
                _logger.Warn("CreateV3Database: Database creation returned null");
                return;
            }

            _logger.Info($"CreateV3Database: Database created/updated successfully");
            LanguageMappingLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.Error($"CreateV3Database failed: {ex}");
        }
    }

    public void UpdateLanguageMapping()
    {
        if (IsV2Checked
         || LanguageMappingPairs is null
         || !LanguageMappingPairs.Any())
            return;


        foreach (var languagePair in LanguageMappingPairs)
        {
            var sourceLanguageCode = languagePair.LanguagePair.SourceCulture.GetLanguageCode(ApiVersion.V3);
            var targetLanguageCode = languagePair.LanguagePair.TargetCulture.GetLanguageCode(ApiVersion.V3);

            languagePair.SourceLanguageCode = sourceLanguageCode;
            languagePair.TargetLanguageCode = targetLanguageCode;
        }
    }
}