using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Google.Cloud.Translate.V3;
using GoogleTranslatorProvider.Commands;
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
		private List<ProjectLocation> _locations;
		private ProjectLocation _selectedLocation;
		private string _projectLocation;

		private string _googleEngineModel;
		private string _glossaryPath;
		private string _jsonFilePath;
		private string _visibleJsonPath;
		private string _projectName;
		private string _glossaryId;
		private string _apiKey;

		private bool _persistGoogleKey;
		private bool _basicCsvGlossary;
		private bool _isTellMeAction;

		private ICommand _navigateToCommand;
		private ICommand _browseJsonFileCommand;
		private ICommand _clearCommand;

		public ProviderViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_options = options;
			_openFileDialogService = new OpenFileDialogService();
			InitializeComponent();
		}

		public BaseModel ViewModel { get; set; }

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
				ClearMessageRaised?.Invoke();
			}
		}

		public List<ProjectLocation> Locations
		{
			get => _locations;
			set
			{
				if (_locations == value) return;
				_locations = value;
				OnPropertyChanged(nameof(Locations));
				SelectedLocation = value.FirstOrDefault();
			}
		}

		public ProjectLocation SelectedLocation
		{
			get => _selectedLocation;
			set
			{
				_selectedLocation = value;
				OnPropertyChanged(nameof(SelectedLocation));
				ProjectLocation = value.Key;
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
			get => _visibleJsonPath;
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

		public string ProjectName
		{
			get => _projectName;
			set
			{
				if (_projectName == value) return;
				_projectName = value;
				OnPropertyChanged(nameof(ProjectName));
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

		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);

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

			Locations = new List<ProjectLocation>
			{
				new ProjectLocation()
				{
					DisplayName = "Global",
					Key = "global"
				},

				new ProjectLocation()
				{
					DisplayName = "Europe",
					Key = "europe-west1"
				},

				new ProjectLocation()
				{
					DisplayName = "US",
					Key = "us-central1"
				}
			};

			if (_options is not null)
			{
				PersistGoogleKey = _options.PersistGoogleKey;
				ApiKey = PersistGoogleKey ? _options.ApiKey : string.Empty;
				JsonFilePath = _options.JsonFilePath;
				ProjectName = _options.ProjectName;
				GoogleEngineModel = _options.GoogleEngineModel;
				ProjectLocation = _options.ProjectLocation ?? ProjectLocation ?? SelectedLocation.Key;
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
			SelectedGoogleApiVersion = GoogleApiVersions[0];
		}

		private void BrowseJsonFile(object o)
		{
			var selectedFile = _openFileDialogService.ShowDialog("JSON File|*.json");
			if (string.IsNullOrEmpty(selectedFile)
			|| !selectedFile.ToLower().EndsWith(".json"))
			{
				return;
			}

			SetProjectDetails(selectedFile);
		}

		private void SetProjectDetails(string selectedFile)
		{
			JsonFilePath = selectedFile;
			VisibleJsonPath = string.Format(@"...\{0}\{1}",
				Path.GetFileName(Path.GetDirectoryName(selectedFile)),
				Path.GetFileName(selectedFile));

			var json = new StreamReader(JsonFilePath).ReadToEnd();
			dynamic temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			ProjectName = temp["project_id"];
		}

		private void Clear(object o)
		{
			switch (o as string)
			{
				case "GoogleEngineModel":
					GoogleEngineModel = string.Empty;
					break;
				case "GlossaryPath":
					GlossaryPath = string.Empty;
					break;
			}
		}

		private void NavigateTo(object o)
		{
			var value = o.ToString().Trim();
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			Process.Start(value);
		}
	}
}