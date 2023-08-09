using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
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
		private bool _useLocalPath;

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
			get => _useLocalPath;
			set
			{
				if (_useLocalPath == value) return;
				_useLocalPath = value;
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

		public ICommand DragDropJsonFileCommand => _dragDropJsonFileCommand ??= new RelayCommand(DragAndDropJsonFile);
		public ICommand DownloadJsonFileCommand => _downloadJsonFileCommand ??= new RelayCommand(DownloadJsonFile);
		public ICommand BrowseJsonFileCommand => _browseJsonFileCommand ??= new RelayCommand(BrowseJsonFile);

		public ICommand OpenLocalPathCommand => _openLocalPathCommand ??= new RelayCommand(OpenLocalPath);
		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);
		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public ICommand SwitchJsonLoadingPathCommand => _switchJsonLoadingPathCommand ??= new RelayCommand(action => { UseLocalPath = !UseLocalPath; });

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
			return GoogleV3OptionsAreSet();
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
					Name = PluginResources.GoogleApiVersionV2Description,
					Version = ApiVersion.V2
				},
				new GoogleApiVersion
				{
					Name = PluginResources.GoogleApiVersionV3Description,
					Version = ApiVersion.V3
				},
			};

			PersistGoogleKey = _options.PersistGoogleKey;
			ApiKey = PersistGoogleKey || _editProvider ? _options.ApiKey : string.Empty;
			JsonFilePath = _options.JsonFilePath;
			VisibleJsonPath = JsonFilePath.ShortenFilePath();
			ProjectId = _options.ProjectId;
			ProjectLocation = _options.ProjectLocation;

			SelectedGoogleApiVersion = GoogleApiVersions.FirstOrDefault(v => v.Version.Equals(_options.SelectedGoogleVersion))
									?? GoogleApiVersions.First(x => x.Version == ApiVersion.V2);
			if (!string.IsNullOrEmpty(_projectLocation))
			{
				Locations = V3ResourceManager.GetLocations(new TranslationOptions
				{
					ProjectId = _projectId,
					JsonFilePath = _jsonFilePath,
					ProjectLocation = DummyLocation
				});
			}

			if (LanguageMappingPairs is not null && LanguageMappingPairs.Any())
			{
				ProjectResourcesLoaded = true;
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
			{
				ReadJsonFile(selectedFile);
			}
		}

		private void ReadJsonFile(string filePath)
		{
			if (!GetJsonDetails(filePath).Success)
			{
				return;
			}

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

		private void GetProjectResources()
		{
			if (string.IsNullOrEmpty(_projectLocation) || _projectLocation.Equals(DummyLocation))
			{
				return;
			}

			var pairMapping = new List<LanguagePairResources>();
			var tempOptions = new TranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = _projectLocation
			};

			var availableGlossaries = V3ResourceManager.GetGlossaries(tempOptions);
			var availableCustomModels = V3ResourceManager.GetCustomModels(tempOptions);
			for (var i = 0; i < _languagePairs.Count(); i++)
			{
				var currentPair = _languagePairs.ElementAt(i);
				var sourceDisplayName = new CultureInfo(currentPair.SourceCultureName).DisplayName;
				var targetDisplayName = new CultureInfo(currentPair.TargetCultureName).DisplayName;
				var mapping = new LanguagePairResources()
				{
					DisplayName = $"{sourceDisplayName} - {targetDisplayName}",
					LanguagePair = currentPair,
					AvailableGlossaries = V3ResourceManager.GetPairGlossaries(currentPair, availableGlossaries),
					AvailableModels = V3ResourceManager.GetPairModels(currentPair, availableCustomModels),
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
		}

		private void OpenLocalPath(object parameter)
		{
			var (Success, ErrorMessage) = JsonFilePath.OpenFolderAndSelectFile();
			if (!Success)
			{
				ErrorHandler.HandleError(ErrorMessage, "Authentication failed");
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
				case nameof(UrlToDownload):
					UrlToDownload = string.Empty;
					ResetFields();
					break;
			}
		}
	}
}