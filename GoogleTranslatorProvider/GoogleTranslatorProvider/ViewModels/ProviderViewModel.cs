using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Google.Protobuf.WellKnownTypes;
using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.GoogleAPI;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.Core;
using static Google.Rpc.Context.AttributeContext.Types;
using Path = System.IO.Path;

namespace GoogleTranslatorProvider.ViewModels
{
	public class ProviderViewModel : BaseModel, IProviderControlViewModel
	{
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ITranslationOptions _options;

		private GoogleApiVersion _selectedGoogleApiVersion;

		private List<RetrievedGlossary> _availableGlossaries;
		private RetrievedGlossary _selectedGlossary;

		private List<string> _locations;
		private string _projectLocation;

		private string _errorMessage;
		private string _googleEngineModel;
		private string _glossaryPath;
		private string _visibleJsonPath;
		private string _jsonFilePath;
		private string _projectId;
		private string _glossaryId;
		private string _urlPath;
		private string _apiKey;

		private bool _projectIdIsCorrupt;
		private bool _persistGoogleKey;
		private bool _basicCsvGlossary;
		private bool _isTellMeAction;
		private bool _useCustomModel;
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

		public bool UseCustomModel
		{
			get => _useCustomModel;
			set
			{
				if (_useCustomModel == value) return;
				_useCustomModel = value;
				OnPropertyChanged(nameof(UseCustomModel));
				ErrorMessage = string.Empty;
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
				GetProjectGlossaries();
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
				if (ProjectIdIsCorrupt)
				{
					ProjectIdIsCorrupt = false;
				}
				ErrorMessage = string.Empty;
			}
		}

		public bool ProjectIdIsCorrupt
		{
			get => _projectIdIsCorrupt;
			set
			{
				if (_projectIdIsCorrupt == value) return;
				_projectIdIsCorrupt = value;
				OnPropertyChanged(nameof(ProjectIdIsCorrupt));
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

		public string UrlPath
		{
			get => _urlPath;
			set
			{
				if (_urlPath == value) return;
				_urlPath = value;
				OnPropertyChanged(nameof(UrlPath));
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
			else if (UseCustomModel
				  && string.IsNullOrEmpty(GoogleEngineModel?.Trim()))
			{
				ErrorMessage = PluginResources.Validation_CustomModel_EnabledEmpty;
				return false;
			}

			return true;
		}

		private bool GoogleV3CredentialsAreValid(LanguagePair[] languagePairs)
		{
			try
			{
				var customModel = UseCustomModel ? GoogleEngineModel : null;
				var providerOptions = new GTPTranslationOptions
				{
					ProjectId = ProjectId,
					JsonFilePath = JsonFilePath,
					GoogleEngineModel = customModel,
					ProjectLocation = ProjectLocation,
					GlossaryPath = SelectedGlossary?.GlossaryID,
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
			_projectId = _options.ProjectId ?? string.Empty;
			_projectLocation = _options.ProjectLocation ?? string.Empty;
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

			if (_options is not null)
			{
				PersistGoogleKey = _options.PersistGoogleKey;
				ApiKey = PersistGoogleKey ? _options.ApiKey : string.Empty;
				JsonFilePath = _options.JsonFilePath;
				ProjectId = _options.ProjectId;
				GoogleEngineModel = _options.GoogleEngineModel;
				UseCustomModel = !string.IsNullOrEmpty(GoogleEngineModel);
				GlossaryPath = _options.GlossaryPath;
				BasicCsvGlossary = _options.BasicCsv;
			}

			SetTranslationOption();
			SetGoogleApiVersion();
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
			if (parameter is not DragEventArgs eventArgs)
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, true) as string[];
			if (fileDrop?.Length != 1)
			{
				return;
			}

			var fileDropped = fileDrop.First();
			if (!fileDropped.EndsWith(".json"))
			{
				return;
			}

			GetJsonDetails(fileDrop.First());
			GetProjectLocations();
		}

		private void DownloadJsonFile(object parameter)
		{
			ErrorMessage = string.Empty;
			UrlPath ??= string.Empty;
			try
			{
				using var webClient = new WebClient();
				var uri = new Uri(UrlPath.Trim());
				if (!Directory.Exists(Constants.DefaultDownloadableLocation))
				{
					Directory.CreateDirectory(Constants.DefaultDownloadableLocation);
				}

				var filePath = Constants.DefaultDownloadableLocation + Constants.DefaultDownloadedJsonFileName;
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}

				webClient.DownloadFile(uri, filePath);
				BrowseJsonFile(filePath);
				GetProjectLocations();
			}
			catch (Exception e)
			{
				ErrorMessage = e.Message;
				return;
			}
		}

		private void BrowseJsonFile(object o)
		{
			const string Browse_JsonFiles = "JSON File|*.json";
			var selectedFile = o as string ?? _openFileDialogService.ShowDialog(Browse_JsonFiles);
			if (string.IsNullOrEmpty(selectedFile)
			|| !selectedFile.ToLower().EndsWith(".json"))
			{
				ErrorMessage = "The selected file it is not JSON type.";
				return;
			}

			GetJsonDetails(selectedFile);
			GetProjectLocations();
		}

		private void GetJsonDetails(string selectedFile)
		{
			JsonFilePath = selectedFile;
			VisibleJsonPath = string.Format(@"...\{0}\{1}",
				Path.GetFileName(Path.GetDirectoryName(selectedFile)),
				Path.GetFileName(selectedFile));
			var projectJson = new StreamReader(JsonFilePath).ReadToEnd();
			var projectDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(projectJson);
			if (!projectDictionary.TryGetValue("project_id", out var value))
			{
				ProjectIdIsCorrupt = true;
			}

			ProjectId = value;
		}

		private void GetProjectLocations()
		{
			var tempOptions = new GTPTranslationOptions
			{
				ProjectId = _projectId,
				JsonFilePath = _jsonFilePath,
				ProjectLocation = "gctp-test"
			};

			try
			{
				var v3Connector = new V3Connector(tempOptions);
				v3Connector.TryToAuthenticateUser();
			}
			catch (Exception e)
			{
				var availableLocationsStr = e.Message.Substring(e.Message.LastIndexOf("Must be"));
				availableLocationsStr = availableLocationsStr.Substring(availableLocationsStr.IndexOf('\''));
				availableLocationsStr = availableLocationsStr.Substring(0, availableLocationsStr.IndexOf('.')).Replace("\'", "").Replace(" ", "");
				Locations.Clear();
				Locations.AddRange(availableLocationsStr.Split(','));
				ProjectLocation = Locations.First();
			}
		}

		private void GetProjectGlossaries()
		{
			var tempList = new List<RetrievedGlossary>();
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
				var myGlossaries = v3Connector.GetGlossaries(_projectLocation);
				tempList.Add(new(new()));
				tempList.AddRange(myGlossaries.Select(retrievedGlossary => new RetrievedGlossary(retrievedGlossary)));
			}
			catch
			{
				tempList.Clear();
				tempList.Add(new(null));
			}

			AvailableGlossaries = tempList;
			SelectedGlossary = _availableGlossaries.First();
		}

		private void OpenLocalPath(object parameter)
		{
			Process.Start("explorer.exe", Path.GetDirectoryName(JsonFilePath));
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
				case nameof(UrlPath):
					UrlPath = string.Empty;
					break;
			}
		}
	}
}