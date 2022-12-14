using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Google.Protobuf.WellKnownTypes;
using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.GoogleAPI;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace GoogleTranslatorProvider.ViewModels
{
	public class ProviderViewModel : BaseModel, IProviderControlViewModel
	{
		private readonly IOpenFileDialogService _openFileDialogService;
		private readonly ITranslationOptions _options;

		private GoogleApiVersion _selectedGoogleApiVersion;
		private List<string> _locations;
		private string _projectLocation;

		private string _googleEngineModel;
		private string _glossaryPath;
		private string _visibleJsonPath;
		private string _jsonFilePath;
		private string _projectId;
		private string _glossaryId;
		private string _apiKey;

		private bool _projectIdIsCorrupt;
		private bool _persistGoogleKey;
		private bool _basicCsvGlossary;
		private bool _isTellMeAction;
		private bool _useCustomModel;

		private ICommand _browseJsonFileCommand;
		private ICommand _navigateToCommand;
		private ICommand _dragDropCommand;
		private ICommand _clearCommand;

		public ProviderViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_options = options;
			_openFileDialogService = new OpenFileDialogService();
			_projectId = _options.ProjectId ?? string.Empty;
			_projectLocation = _options.ProjectLocation ?? string.Empty;
			InitializeComponent();
		}

		public BaseModel ViewModel { get; set; }

		public bool UseCustomModel
		{
			get => _useCustomModel;
			set
			{
				if (_useCustomModel == value) return;
				_useCustomModel = value;
				OnPropertyChanged(nameof(UseCustomModel));
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
				GetProjectGlossaries();
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
			get => _visibleJsonPath ?? "Not specified";
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

		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);

		public ICommand DragDropCommand => _dragDropCommand ??= new RelayCommand(DragAndDrop);

		public ICommand BrowseJsonFileCommand => _browseJsonFileCommand ??= new RelayCommand(BrowseJsonFile);

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public event ClearMessageEventRaiser ClearMessageRaised;

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

		private void DragAndDrop(object parameter)
		{
			if (parameter is not DragEventArgs eventArgs)
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, true) as string[];
			if (fileDrop.Length != 1)
			{
				return;
			}

			BrowseJsonFile(fileDrop.First());
		}

		private void BrowseJsonFile(object o)
		{
			const string Browse_JsonFiles = "JSON File|*.json";

			var selectedFile = o as string ?? _openFileDialogService.ShowDialog(Browse_JsonFiles);
			if (string.IsNullOrEmpty(selectedFile)
			|| !selectedFile.ToLower().EndsWith(".json"))
			{
				return;
			}

			GetJsonDetails(selectedFile);
			GetProjectLocations();
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

		private List<RetrievedGlossary> _availableGlossaries;
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

		private RetrievedGlossary _selectedGlossary;
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

		private void Clear(object parameter)
		{
			switch (parameter as string)
			{
				case nameof(ProjectId):
					ProjectId = string.Empty;
					break;
				case nameof(ProjectLocation):
					ProjectLocation = string.Empty;
					break;
				case nameof(GoogleEngineModel):
					GoogleEngineModel = string.Empty;
					break;
				case nameof(GlossaryPath):
					GlossaryPath = string.Empty;
					break;
			}
		}

		private void NavigateTo(object o)
		{
			if (o is string target)
			{
				Process.Start(target);
			}
		}
	}
}