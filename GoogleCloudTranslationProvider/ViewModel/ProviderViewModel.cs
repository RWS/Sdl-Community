using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.Service;
using GoogleCloudTranslationProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace GoogleCloudTranslationProvider.ViewModels
{
	public class ProviderViewModel : BaseViewModel, IProviderControlViewModel
	{
		private const string DummyLocation = "gctp-sdl";
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ITranslationOptions _options;
		private readonly IEnumerable<LanguagePair> _languagePairs;

		private GoogleApiVersion _selectedGoogleApiVersion;

		private List<RetrievedGlossary> _availableGlossaries;
		private RetrievedGlossary _selectedGlossary;
		private List<RetrievedCustomModel> _availableCustomModels;
		private RetrievedCustomModel _selectedCustomModel;

		private List<string> _locations;
		private string _projectLocation;

		private string _googleEngineModel;
		private string _glossaryPath;
		private string _visibleJsonPath;
		private string _jsonFilePath;
		private string _projectId;
		private string _glossaryId;
		private string _urlToDownload;
		private string _apiKey;

		private bool _canModifyExistingFields;
		private bool _projectResourcesLoaded;
		private bool _persistGoogleKey;
		private bool _basicCsvGlossary;
		private bool _isTellMeAction;
		private bool _useUrlPath;

		private ICommand _downloadJsonFileCommand;
		private ICommand _dragDropJsonFileCommand;
		private ICommand _browseJsonFileCommand;
		private ICommand _openLocalPathCommand;
		private ICommand _navigateToCommand;
		private ICommand _clearCommand;

		public ProviderViewModel(ITranslationOptions options, List<LanguagePair> languagePairs)
		{
			ViewModel = this;
			_options = options;
			_languagePairs = languagePairs;
			CanChangeProviderResources = string.IsNullOrEmpty(JsonFilePath);
			_openFileDialogService = new OpenFileDialogService();
			InitializeComponent();
		}

		public BaseViewModel ViewModel { get; set; }

		public List<RetrievedGlossary> AvailableGlossaries
		{
			get => _availableGlossaries;
			set
			{
				if (_availableGlossaries == value) return;
				_availableGlossaries = value;
				OnPropertyChanged(nameof(AvailableGlossaries));
			}
		}

		public RetrievedGlossary SelectedGlossary
		{
			get => _selectedGlossary;
			set
			{
				if (_selectedGlossary == value) return;
				_selectedGlossary = value;
				OnPropertyChanged(nameof(SelectedGlossary));
				GlossaryPath = value?.GlossaryID;
			}
		}

		public List<RetrievedCustomModel> AvailableCustomModels
		{
			get => _availableCustomModels;
			set
			{
				if (_availableCustomModels == value) return;
				_availableCustomModels = value;
				OnPropertyChanged(nameof(AvailableCustomModels));
			}
		}

		public RetrievedCustomModel SelectedCustomModel
		{
			get => _selectedCustomModel;
			set
			{
				if (_selectedCustomModel == value) return;
				_selectedCustomModel = value;
				OnPropertyChanged(nameof(SelectedCustomModel));
				GoogleEngineModel = value?.DatasetId;
			}
		}

		public List<TranslationOption> TranslationOptions { get; set; }

		public TranslationOption SelectedTranslationOption => TranslationOptions[0];

		public List<GoogleApiVersion> GoogleApiVersions { get; set; }

		public GoogleApiVersion SelectedGoogleApiVersion
		{
			get => _selectedGoogleApiVersion;
			set
			{
				if (_selectedGoogleApiVersion == value) return;
				_selectedGoogleApiVersion = value;
				OnPropertyChanged(nameof(SelectedGoogleApiVersion));
				OnPropertyChanged(nameof(IsV2Checked));
				OnPropertyChanged(nameof(IsV3Checked));
				ClearMessageRaised?.Invoke();
			}
		}

		public List<string> Locations
		{
			get => _locations ??= new();
			set
			{
				if (_locations == value) return;
				_locations = value;
				OnPropertyChanged(nameof(Locations));
			}
		}

		public string ProjectLocation
		{
			get => _projectLocation;
			set
			{
				if (_projectLocation == value) return;
				_projectLocation = value;
				OnPropertyChanged(nameof(ProjectLocation));
				GetProjectResources();
				ClearMessageRaised?.Invoke();
			}
		}

		public bool BasicCsvGlossary
		{
			get => _basicCsvGlossary;
			set
			{
				if (_basicCsvGlossary == value) return;
				_basicCsvGlossary = value;
				OnPropertyChanged(nameof(BasicCsvGlossary));
			}
		}

		public bool PersistGoogleKey
		{
			get => _persistGoogleKey;
			set
			{
				if (_persistGoogleKey == value) return;
				_persistGoogleKey = value;
				OnPropertyChanged(nameof(PersistGoogleKey));
				_options.PersistGoogleKey = value;
			}
		}

		public bool IsTellMeAction
		{
			get => _isTellMeAction;
			set
			{
				if (_isTellMeAction == value) return;
				_isTellMeAction = value;
				OnPropertyChanged(nameof(IsTellMeAction));
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
				OnPropertyChanged(nameof(CanChangeProviderResources));
			}
		}

		public bool ProjectResourcesLoaded
		{
			get => _projectResourcesLoaded;
			set
			{
				if (_projectResourcesLoaded == value) return;
				_projectResourcesLoaded = value;
				OnPropertyChanged(nameof(ProjectResourcesLoaded));
			}
		}

		public string GoogleEngineModel
		{
			get => _googleEngineModel;
			set
			{
				if (_googleEngineModel == value) return;
				_googleEngineModel = value;
				OnPropertyChanged(nameof(GoogleEngineModel));
				ClearMessageRaised?.Invoke();
			}
		}

		public string VisibleJsonPath
		{
			get => _visibleJsonPath ?? PluginResources.ProviderViewModel_PathNotSpecified;
			set
			{
				if (_visibleJsonPath == value) return;
				_visibleJsonPath = value;
				OnPropertyChanged(nameof(VisibleJsonPath));
			}
		}

		public string JsonFilePath
		{
			get => _jsonFilePath;
			set
			{
				if (_jsonFilePath == value) return;
				_jsonFilePath = value;
				OnPropertyChanged(nameof(JsonFilePath));
				ClearMessageRaised?.Invoke();
			}
		}

		public string ProjectId
		{
			get => _projectId ?? PluginResources.ProviderViewModel_PathNotSpecified;
			set
			{
				if (_projectId == value) return;
				_projectId = value;
				OnPropertyChanged(nameof(ProjectId));
				ClearMessageRaised?.Invoke();
			}
		}

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				if (_apiKey == value) return;
				_apiKey = value.Trim();
				OnPropertyChanged(nameof(ApiKey));
				ClearMessageRaised?.Invoke();
			}
		}

		public string GlossaryPath
		{
			get => _glossaryPath;
			set
			{
				if (_glossaryPath == value) return;
				_glossaryPath = value;
				OnPropertyChanged(nameof(GlossaryPath));
				ClearMessageRaised?.Invoke();
			}
		}

		public string GlossaryId
		{
			get => _glossaryId;
			set
			{
				if (_glossaryId == value) return;
				_glossaryId = value;
				OnPropertyChanged(nameof(GlossaryId));
				ClearMessageRaised?.Invoke();
			}
		}

		public bool UseUrlPath
		{
			get => _useUrlPath;
			set
			{
				if (_useUrlPath == value) return;
				_useUrlPath = value;
				OnPropertyChanged(nameof(UseUrlPath));
			}
		}

		public string UrlToDownload
		{
			get => _urlToDownload;
			set
			{
				if (_urlToDownload == value) return;
				_urlToDownload = value;
				OnPropertyChanged(nameof(UrlToDownload));
			}
		}

		private void ResetFields()
		{
			AvailableCustomModels = null;
			AvailableGlossaries = null;
			BasicCsvGlossary = false;
			CanChangeProviderResources = false;
			GlossaryId = null;
			GlossaryPath = null;
			GoogleEngineModel = null;
			JsonFilePath = null;
			Locations = new();
			ProjectId = null;
			ProjectLocation = null;
			ProjectResourcesLoaded = false;
			SelectedCustomModel = null;
			SelectedGlossary = null;
			UrlToDownload = null;
			VisibleJsonPath = null;
		}

		public ICommand DragDropJsonFileCommand => _dragDropJsonFileCommand ??= new RelayCommand(DragAndDropJsonFile);
		public ICommand DownloadJsonFileCommand => _downloadJsonFileCommand ??= new RelayCommand(DownloadJsonFile);
		public ICommand BrowseJsonFileCommand => _browseJsonFileCommand ??= new RelayCommand(BrowseJsonFile);

		public ICommand OpenLocalPathCommand => _openLocalPathCommand ??= new RelayCommand(OpenLocalPath);
		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);
		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public event ClearMessageEventRaiser ClearMessageRaised;

		public bool CanConnectToGoogleV2(HtmlUtil htmlUtil)
		{
			if (string.IsNullOrEmpty(ApiKey))
			{
				ErrorHandler.HandleError(PluginResources.Validation_ApiKey, "Invalid key");
				return false;
			}

			try
			{
				var v2Connector = new V2Connector(ApiKey, htmlUtil);
				return v2Connector.CredentialsAreValid();
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

		public bool CanConnectToGoogleV3(LanguagePair[] languagePairs)
		{
			return GoogleV3OptionsAreSet() && GoogleV3CredentialsAreValid(languagePairs);
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

		private bool GoogleV3CredentialsAreValid(LanguagePair[] languagePairs)
		{
			try
			{
				var providerOptions = new GCTPTranslationOptions
				{
					ProjectId = ProjectId,
					JsonFilePath = JsonFilePath,
					GoogleEngineModel = GoogleEngineModel,
					ProjectLocation = ProjectLocation,
					GlossaryPath = GlossaryPath,
					SelectedGoogleVersion = SelectedGoogleApiVersion.Version,
					PairMappings = Mappings
				};

				var googleV3 = new V3Connector(providerOptions);
				googleV3.TryToAuthenticateUser(languagePairs);
				return true;
			}
			catch (Exception e)
			{
				if (e.Message.Contains("Resource type: models") || e.Message.Contains("The model"))
				{
					ErrorHandler.HandleError(PluginResources.Validation_ModelName_Invalid, "Custom Models");
				}
				else if (e.Message.Contains("Invalid resource name") || e.Message.Contains("project number"))
				{
					ErrorHandler.HandleError(PluginResources.Validation_ProjectID_Failed, nameof(ProjectId));
				}
				else if (e.Message.Contains("Glossary not found"))
				{
					ErrorHandler.HandleError(PluginResources.Validation_Glossary_Invalid, nameof(GlossaryPath));
				}
				else if (e.Message.Contains("PermissionDenied"))
				{
					ErrorHandler.HandleError(PluginResources.Validation_PermissionDenied, "Permission Denied");
				}
				else
				{
					ErrorHandler.HandleError(e);
				}

				return false;
			}
		}

		private void InitializeComponent()
		{
			TranslationOptions = new List<TranslationOption>
			{
				new TranslationOption
				{
					Name = PluginResources.Google,
					ProviderType = ProviderType.GoogleTranslate
				}
			};

			GoogleApiVersions = new List<GoogleApiVersion>
			{
				new GoogleApiVersion
				{
					Name = PluginResources.GoogleApiVersionV2Description,
					Version = ApiVersion.V2
				},
				new GoogleApiVersion
				{
					Name = PluginResources.GoogleApiVersionV3Description,
					Version = ApiVersion.V3
				},
			};

			ProjectId = string.Empty;
			ProjectLocation = string.Empty;
			if (_options is not null)
			{
				PersistGoogleKey = _options.PersistGoogleKey;
				ApiKey = PersistGoogleKey ? _options.ApiKey : string.Empty;
				JsonFilePath = _options.JsonFilePath;
				VisibleJsonPath = JsonFilePath.ShortenFilePath();
				ProjectId = _options.ProjectId;
				ProjectLocation = _options.ProjectLocation;
				GoogleEngineModel = _options.GoogleEngineModel;
				GlossaryPath = _options.GlossaryPath;
				Mappings = _options.PairMappings;
			}

			SelectedGoogleApiVersion = GoogleApiVersions.FirstOrDefault(v => v.Version.Equals(_options.SelectedGoogleVersion))
									?? GoogleApiVersions.First(x => x.Version == ApiVersion.V3);
			if (!string.IsNullOrEmpty(_projectId)
			 && !string.IsNullOrEmpty(_projectLocation))
			{
				Locations.Clear();
				Locations.Add(_projectLocation);
				SelectedGlossary = _availableGlossaries.FirstOrDefault(x => x.GlossaryID == _options.GlossaryPath) ?? _availableGlossaries.First();
				SelectedCustomModel = _availableCustomModels.FirstOrDefault(x => x.DatasetId == _options.GoogleEngineModel) ?? _availableCustomModels.First();
			}
		}

		private void DragAndDropJsonFile(object parameter)
		{
			if (parameter is not DragEventArgs eventArgs
			 || eventArgs.Data.GetData(DataFormats.FileDrop, true) is not string[] fileDrop
			 || fileDrop?.Length < 1)
			{
				return;
			}

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
			var operation = UrlToDownload.Trim().VerifyAndDownloadJsonFile(_options.DownloadPath, Constants.DefaultDownloadedJsonFileName);
			if (!operation.Success)
			{
				ErrorHandler.HandleError(operation.ErrorMessage, "Download failed");
				return;
			}

			ReadJsonFile($"{_options.DownloadPath}\\{Constants.DefaultDownloadedJsonFileName}");
		}

		private void BrowseJsonFile(object parameter)
		{
			const string Browse_JsonFiles = "JSON File|*.json";
			var selectedFile = _openFileDialogService.ShowDialog(Browse_JsonFiles);
			if (!string.IsNullOrEmpty(selectedFile))
			{
				ReadJsonFile(selectedFile);
			}
		}

		private void ReadJsonFile(string filePath)
		{
			GetJsonDetails(filePath);
			var tempOptions = new GCTPTranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = DummyLocation
			};

			Locations = ProjectConnector.GetLocations(tempOptions);
			ProjectLocation = Locations.First();
		}

		private void GetJsonDetails(string selectedFile)
		{
			var (success, operationResult) = selectedFile.VerifyPathAndReadJsonFile();
			if (!success)
			{
				ErrorHandler.HandleError(operationResult as string, "Reading failed");
				return;
			}

			JsonFilePath = selectedFile;
			VisibleJsonPath = selectedFile.ShortenFilePath();
			ProjectId = (operationResult as Dictionary<string, string>)["project_id"];
		}

		private void CreateMapping()
		{
			var pairMapping = new List<PairMapping>();
			for (var i = 0; i < _languagePairs.Count(); i++)
			{
				var currentPair = _languagePairs.ElementAt(i);
				var mapping = new PairMapping()
				{
					DisplayName = $"{currentPair.SourceCulture.DisplayName} - {currentPair.TargetCulture.DisplayName}",
					LanguagePair = currentPair,
					AvailableGlossaries = ProjectConnector.GetPairGlossaries(currentPair, _availableGlossaries),
					AvailableModels = ProjectConnector.GetPairModels(currentPair, _availableCustomModels)
				};

				pairMapping.Add(mapping);
			}

			Mappings = pairMapping;
		}

		public void GetProjectResources()
		{
			if (string.IsNullOrEmpty(_projectLocation) || _projectLocation.Equals(DummyLocation))
			{
				return;
			}

			var tempOptions = new GCTPTranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = _projectLocation
			};

			AvailableGlossaries = ProjectConnector.GetGlossaries(tempOptions);
			SelectedGlossary = _availableGlossaries.First();
			AvailableCustomModels = ProjectConnector.GetCustomModels(tempOptions);
			SelectedCustomModel = _availableCustomModels.First();

			if (_availableGlossaries.Any() && _availableCustomModels.Any())
			{
				ProjectResourcesLoaded = true;
			}


			CreateMapping();
		}

		private void OpenLocalPath(object parameter)
		{
			var operation = JsonFilePath.OpenFolderAndSelectFile();
			if (!operation.Success)
			{
				ErrorHandler.HandleError(operation.ErrorMessage, "Authentication failed");
			}
		}

		private void NavigateTo(object parameter)
		{
			if (parameter is not string target)
			{
				return;
			}

			var query = IsV2Checked || target.Contains("cloud-resource-manager") ? string.Empty : $"?project={_projectId}";
			if (!string.IsNullOrEmpty(target))
			{
				Process.Start($"{target}{query}");
			}
		}

		private void Clear(object parameter)
		{
			switch (parameter as string)
			{
				case nameof(GoogleEngineModel):
					GoogleEngineModel = string.Empty;
					break;
				case nameof(UrlToDownload):
					UrlToDownload = string.Empty;
					ResetFields();
					break;
			}
		}

		private List<PairMapping> _mappings;
		public List<PairMapping> Mappings
		{
			get => _mappings;
			set
			{
				if (_mappings == value) return;
				_mappings = value;
				OnPropertyChanged();
			}
		}

		private PairMapping _selectedMapping;
		public PairMapping SelectedMapping
		{
			get => _selectedMapping;
			set
			{
				if (_selectedMapping == value) return;
				_selectedMapping = value;
				OnPropertyChanged();
			}
		}
	}
}