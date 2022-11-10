using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Model;
using GoogleTranslatorProvider.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace GoogleTranslatorProvider.ViewModels
{
	public class ProviderControlViewModel : BaseModel, IProviderControlViewModel
	{
		private readonly ITranslationOptions _options;
		private ICommand _clearCommand;
		private TranslationOption _selectedTranslationOption;
		private GoogleApiVersion _selectedGoogleApiVersion;
		private bool _isV2Checked;
		private bool _persistGoogleKey;
		private bool _isTellMeAction;
		private bool _basicCsvGlossary;
		private string _apiKey;
		private string _jsonFilePath;
		private string _projectName;
		private string _googleEngineModel;
		private string _projectLocation;
		private string _glossaryId;
		private string _glossaryPath;

		public ProviderControlViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_options = options;
			InitializeComponent();
		}

		////TODO: If is tell me action hide back button(first page) we need to show only the settings page
		//public ProviderControlViewModel(IMtTranslationOptions options,bool isTellMeAction)
		//{
		//	_options = options;
		//	_isTellMeAction = isTellMeAction;
		//	InitializeComponent();

		//}

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

			if (_options != null)
			{
				ApiKey = _options.ApiKey;
				PersistGoogleKey = _options.PersistGoogleKey;
				JsonFilePath = _options.JsonFilePath;
				ProjectName = _options.ProjectName;
				GoogleEngineModel = _options.GoogleEngineModel;
				ProjectLocation = _options.ProjectLocation;
				GlossaryPath = _options.GlossaryPath;
				BasicCsvGlossary = _options.BasicCsv;
			}


			SetTranslationOption();
			SetGoogleApiVersion();
		}
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(Clear));

		private void Clear(object obj)
		{
			if (obj is not string objectName) return;

			switch (objectName)
			{
				case "JsonFilePath":
					JsonFilePath = string.Empty;
					break;
				case "ProjectName":
					ProjectName = string.Empty;
					break;
				case "ProjectLocation":
					ProjectLocation = string.Empty;
					break;
				case "GoogleEngineModel":
					GoogleEngineModel = string.Empty;
					break;
				case "GlossaryPath":
					GlossaryPath = string.Empty;
					break;
			}
		}

		public BaseModel ViewModel { get; set; }

		public ICommand ShowSettingsCommand { get; set; }

		public List<TranslationOption> TranslationOptions { get; set; }

		public List<GoogleApiVersion> GoogleApiVersions { get; set; }

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
		public event ClearMessageEventRaiser ClearMessageRaised;

		public GoogleApiVersion SelectedGoogleApiVersion
		{
			get => _selectedGoogleApiVersion;
			set
			{
				_selectedGoogleApiVersion = value;
				IsV2Checked = _selectedGoogleApiVersion.Version == ApiVersion.V2;
				OnPropertyChanged(nameof(SelectedGoogleApiVersion));
				ClearMessageRaised?.Invoke();
			}
		}

		public TranslationOption SelectedTranslationOption
		{
			get => _selectedTranslationOption;
			set
			{
				_selectedTranslationOption = value;
				OnPropertyChanged(nameof(SelectedTranslationOption));
				ClearMessageRaised?.Invoke();
			}
		}

		public bool IsV2Checked
		{
			get => _isV2Checked;
			set
			{
				if (_isV2Checked == value) return;
				_isV2Checked = value;
				OnPropertyChanged(nameof(IsV2Checked));
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

		public bool PersistGoogleKey
		{
			get => _persistGoogleKey;
			set
			{
				if (_persistGoogleKey == value) return;
				_persistGoogleKey = value;
				OnPropertyChanged(nameof(PersistGoogleKey));
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

		private void SetTranslationOption()
		{
			if ((_options?.SelectedProvider) == null)
			{
				SelectGoogleV2();
				return;
			}

			var selectedProvider = TranslationOptions.FirstOrDefault(t => t.ProviderType.Equals(_options.SelectedProvider));
			if (selectedProvider == null)
			{
				SelectGoogleV2();
			}

			SelectedTranslationOption ??= selectedProvider;
		}

		private void SetGoogleApiVersion()
		{
			if (_options?.SelectedGoogleVersion != null)
			{
				var selectedVersion = GoogleApiVersions.FirstOrDefault(v => v.Version.Equals(_options.SelectedGoogleVersion));
				if (selectedVersion != null)
				{
					SelectedGoogleApiVersion = selectedVersion;
				}
				else
				{
					SelectGoogleV2();
				}
			}
			else
			{
				//Bydefault we'll select Google V2 version - which is the basic one
				SelectGoogleV2();
			}
		}

		private void SelectGoogleV2()
		{
			SelectedTranslationOption = TranslationOptions[0];
			SelectedGoogleApiVersion = GoogleApiVersions[0];
			IsV2Checked = true;
		}
	}
}