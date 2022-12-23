using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.Extensions;
using GoogleTranslatorProvider.GoogleAPI;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Sdl.LanguagePlatform.Core;

namespace GoogleTranslatorProvider.ViewModels
{
	public class ProviderViewModel : BaseModel, IProviderControlViewModel
	{
		private const string DummyLocation = "gctp-sdl";
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ITranslationOptions _options;

		private GoogleApiVersion _selectedGoogleApiVersion;

		private List<RetrievedGlossary> _availableGlossaries;
		private RetrievedGlossary _selectedGlossary;
		private List<RetrievedCustomModel> _availableCustomModels;
		private RetrievedCustomModel _selectedCustomModel;

		private List<string> _locations;
		private string _projectLocation;

		private string _errorMessage;
		private string _googleEngineModel;
		private string _glossaryPath;
		private string _visibleJsonPath;
		private string _jsonFilePath;
		private string _projectId;
		private string _glossaryId;
		private string _urlToDownload;
		private string _apiKey;

		private bool _canModifyExistingFields;
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

		public ProviderViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_options = options;
			_openFileDialogService = new OpenFileDialogService();
			InitializeComponent();
		}

		public BaseModel ViewModel { get; set; }

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
			}
		}

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
				GlossaryPath = value.GlossaryID;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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

		public string GoogleEngineModel
		{
			get => _googleEngineModel;
			set
			{
				if (_googleEngineModel == value) return;
				_googleEngineModel = value;
				OnPropertyChanged(nameof(GoogleEngineModel));
				ClearMessageRaised?.Invoke();
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
			}
		}

		public string ProjectId
		{
			get => _projectId;
			set
			{
				if (_projectId == value) return;
				_projectId = value;
				OnPropertyChanged(nameof(ProjectId));
				ClearMessageRaised?.Invoke();
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = PluginResources.Validation_ApiKey;
				return false;
			}

			try
			{
				var v2Connector = new V2Connector(ApiKey, htmlUtil);
				v2Connector.ValidateCredentials();
				return true;
			}
			catch (Exception e)
			{
				ErrorMessage = e.Message;
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
				ErrorMessage = PluginResources.Validation_EmptyJsonFilePath;
				return false;
			}
			else if (!File.Exists(JsonFilePath))
			{
				ErrorMessage = PluginResources.Validation_MissingJsonFile;
				return false;
			}
			else if (string.IsNullOrEmpty(ProjectId))
			{
				ErrorMessage = PluginResources.Validation_ProjectID_Empty;
				return false;
			}
			else if (string.IsNullOrEmpty(ProjectLocation))
			{
				ErrorMessage = PluginResources.Validation_Location_Empty;
				return false;
			}

			return true;
		}

		private bool GoogleV3CredentialsAreValid(LanguagePair[] languagePairs)
		{
			try
			{
				var providerOptions = new GTPTranslationOptions
				{
					ProjectId = ProjectId,
					JsonFilePath = JsonFilePath,
					GoogleEngineModel = GoogleEngineModel,
					ProjectLocation = ProjectLocation,
					GlossaryPath = GlossaryPath,
					BasicCsv = BasicCsvGlossary,
					SelectedProvider = SelectedTranslationOption.ProviderType,
					SelectedGoogleVersion = SelectedGoogleApiVersion.Version
				};

				var googleV3 = new V3Connector(providerOptions);
				googleV3.TryToAuthenticateUser(languagePairs);
				return true;
			}
			catch (Exception e)
			{
				if (e.Message.Contains("Resource type: models") || e.Message.Contains("The model"))
				{
					ErrorMessage = PluginResources.Validation_ModelName_Invalid;
				}
				else if (e.Message.Contains("Invalid resource name"))
				{
					ErrorMessage = PluginResources.Validation_ProjectID_Failed;
				}
				else if (e.Message.Contains("Glossary not found"))
				{
					ErrorMessage = "Wrong glossary";
				}
				else if (e.Message.Contains("PermissionDenied"))
				{
					ErrorMessage = PluginResources.Validation_PermissionDenied;
				}
				else
				{
					ErrorMessage = e.Message;
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
				}
			};

			ProjectId = string.Empty;
			ProjectLocation = string.Empty;
			if (_options is not null)
			{
				PersistGoogleKey = _options.PersistGoogleKey;
				ApiKey = PersistGoogleKey ? _options.ApiKey : string.Empty;
				JsonFilePath = _options.JsonFilePath;
				ProjectId = _options.ProjectId;
				ProjectLocation = _options.ProjectLocation;
				GoogleEngineModel = _options.GoogleEngineModel;
				GlossaryPath = _options.GlossaryPath;
				BasicCsvGlossary = _options.BasicCsv;
			}


			SetTranslationOption();
			SetGoogleApiVersion();
			if (!string.IsNullOrEmpty(_projectId)
			 && !string.IsNullOrEmpty(_projectLocation))
			{
				CanChangeProviderResources = true;
				Locations.Clear();
				Locations.Add(_projectLocation);
				SelectedGlossary = _availableGlossaries.FirstOrDefault(x => x.GlossaryID == _options.GlossaryPath) ?? _availableGlossaries.First();
				SelectedCustomModel = _availableCustomModels.FirstOrDefault(x => x.DatasetId == _options.GoogleEngineModel) ?? _availableCustomModels.First();
			}
		}

		private void SetTranslationOption()
		{
			if (_options?.SelectedProvider is null)
			{
				SelectGoogleV2();
				return;
			}

			var selectedProvider = TranslationOptions.FirstOrDefault(t => t.ProviderType.Equals(_options.SelectedProvider));
			if (selectedProvider is null)
			{
				SelectGoogleV2();
				return;
			}
		}

		private void SetGoogleApiVersion()
		{
			if (_options?.SelectedGoogleVersion is null)
			{
				SelectGoogleV2();
				return;
			}

			var selectedVersion = GoogleApiVersions.FirstOrDefault(v => v.Version.Equals(_options.SelectedGoogleVersion));
			if (selectedVersion is null)
			{
				SelectGoogleV2();
				return;
			}

			SelectedGoogleApiVersion = selectedVersion;
		}

		private void SelectGoogleV2()
		{
			SelectedGoogleApiVersion = GoogleApiVersions.First(x => x.Version == ApiVersion.V2);
		}

		private void DragAndDropJsonFile(object parameter)
		{
			ErrorMessage = string.Empty;
			if (parameter is not DragEventArgs eventArgs)
			{
				return;
			}

			if (eventArgs.Data.GetData(DataFormats.FileDrop, true) is not string[] fileDrop
			 || fileDrop?.Length < 1)
			{
				return;
			}

			if (fileDrop.Length > 1)
			{
				ErrorMessage = "Only one file can be dropped to be used on the authentication process.";
				return;
			}

			ReadJsonFile(fileDrop.First());
		}

		private void DownloadJsonFile(object parameter)
		{
			ErrorMessage = string.Empty;
			UrlToDownload ??= string.Empty;
			var operation = UrlToDownload.VerifyAndDownloadJsonFile(Constants.DefaultDownloadableLocation + Constants.DefaultDownloadedJsonFileName);
			if (operation.Success)
			{
				ReadJsonFile(Constants.DefaultDownloadableLocation + Constants.DefaultDownloadedJsonFileName);
				return;
			}

			ErrorMessage = operation.ErrorMessage;
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
			GetProjectLocations();
		}

		private void GetJsonDetails(string selectedFile)
		{
			ErrorMessage = string.Empty;
			var (success, operationResult) = selectedFile.VerifyPathAndReadJsonFile();
			if (!success)
			{
				ErrorMessage = operationResult as string;
				return;
			}

			JsonFilePath = selectedFile;
			VisibleJsonPath = selectedFile.ShortenFilePath();
			ProjectId = (operationResult as Dictionary<string, string>)["project_id"];
		}

		private void GetProjectLocations()
		{
			var tempOptions = new GTPTranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = DummyLocation
			};

			string errorMessage;
			try
			{
				var v3Connector = new V3Connector(tempOptions);
				v3Connector.TryToAuthenticateUser();
				errorMessage = string.Empty;
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				return;
			}

			if (!errorMessage.Contains("Unsupported location"))
			{
				ErrorMessage = "Could not authenticate the user with the ProjectID provided in the JSON file.";
				return;
			}

			var matches = new Regex(@"(['])(?:(?=(\\?))\2.)*?\1").Matches(errorMessage);
			Locations.Clear();
			Locations.AddRange(from object match in matches
							   let locationValue = match.ToString().Replace("\'", "")
							   where !Locations.Contains(locationValue) && !locationValue.Equals(DummyLocation)
							   select locationValue);
			ProjectLocation = Locations.First();
		}

		public void GetProjectResources()
		{
			if (string.IsNullOrEmpty(_projectLocation) || _projectLocation.Equals(DummyLocation))
			{
				return;
			}

			GetProjectGlossaries();
			GetProjectCustomModels();
		}

		private void GetProjectGlossaries()
		{
			var tempGlossariesList = new List<RetrievedGlossary>();
			var tempOptions = new GTPTranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = _projectLocation
			};

			try
			{
				var v3Connector = new V3Connector(tempOptions);
				v3Connector.TryToAuthenticateUser();

				tempGlossariesList.Add(new(new()));
				tempGlossariesList.AddRange(v3Connector.GetGlossaries(_projectLocation).Select(retrievedGlossary => new RetrievedGlossary(retrievedGlossary)));
			}
			catch
			{
				tempGlossariesList.Clear();
				tempGlossariesList.Add(new(null));
			}

			AvailableGlossaries = tempGlossariesList;
			SelectedGlossary = _availableGlossaries.First();
		}

		private void GetProjectCustomModels()
		{
			var tempCustomModelsList = new List<RetrievedCustomModel>();
			var tempOptions = new GTPTranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = _projectLocation
			};

			try
			{
				var v3Connector = new V3Connector(tempOptions);
				v3Connector.TryToAuthenticateUser();

				tempCustomModelsList.Add(new(new()));
				tempCustomModelsList.AddRange(v3Connector.GetCustomModels().Select(retrievedCustomModel => new RetrievedCustomModel(retrievedCustomModel)));
			}
			catch
			{
				tempCustomModelsList.Clear();
				tempCustomModelsList.Add(new(null));
			}

			AvailableCustomModels = tempCustomModelsList;
			SelectedCustomModel = _availableCustomModels.First();
		}

		private void OpenLocalPath(object parameter)
		{
			var operation = JsonFilePath.OpenFolderAndSelectFile();
			if (!operation.Success)
			{
				ErrorMessage = operation.ErrorMessage;
			}
		}

		private void NavigateTo(object parameter)
		{
			if (parameter is string target)
			{
				Process.Start(target);
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
					break;
			}
		}
	}
}