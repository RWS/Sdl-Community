using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Activation;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class ProviderControlViewModel : ModelBase, IProviderControlViewModel
	{
		private readonly IMtTranslationOptions _options;
		private ICommand _clearCommand;
		private readonly RegionsProvider _regionsProvider;
		private TranslationOption _selectedTranslationOption;
		private GoogleApiVersion _selectedGoogleApiVersion;
		private bool _isMicrosoftSelected;
		private bool _isMicrosoftWithPeSelected;
		private bool _useCatId;
		private bool _isV2Checked;
		private bool _persistGoogleKey;
		private bool _persistMicrosoftKey;
		private bool _isTellMeAction;
		private bool _basicCsvGlossary;
		private string _catId;
		private string _peUrl;
		private string _apiKey;
		private string _clientId;
		private SubscriptionRegion _region;
		private ObservableCollection<SubscriptionRegion> _regions;
		private string _jsonFilePath;
		private string _projectName;
		private string _googleEngineModel;
		private string _projectLocation;
		private string _glossaryId;
		private string _glossaryPath;

		public ProviderControlViewModel(IMtTranslationOptions options, RegionsProvider regionsProvider)
		{
			ViewModel = this;
			_options = options;
			_regionsProvider = regionsProvider;
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
					Name = PluginResources.Microsoft,
					ProviderType = MtTranslationOptions.ProviderType.MicrosoftTranslator
				},
				new TranslationOption
				{
					Name = PluginResources.MicrosoftWithPe,
					ProviderType = MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe
				},
				new TranslationOption
				{
					Name = PluginResources.Google,
					ProviderType = MtTranslationOptions.ProviderType.GoogleTranslate
				}
			};

			GoogleApiVersions = new List<GoogleApiVersion>
			{
				new GoogleApiVersion
				{
					Name = PluginResources.GoogleApiVersionV2Description,
					Version = Enums.GoogleApiVersion.V2
				},
				new GoogleApiVersion
				{
					Name = PluginResources.GoogleApiVersionV3Description,
					Version = Enums.GoogleApiVersion.V3
				}
			};

			// set the default region
			Region = Regions.FirstOrDefault(a => a.Key == "");

			if (_options != null)
			{
				PeUrl = _options.PeUrl;
				ClientId = _options.ClientId;
				PersistMicrosoftKey = _options.PersistMicrosoftCreds;
				UseCatId = _options.UseCatID;
				CatId = _options.CatId;

				ApiKey = _options.ApiKey;
				PersistGoogleKey = _options.PersistGoogleKey;
				JsonFilePath = _options.JsonFilePath;
				ProjectName = _options.ProjectName;
				GoogleEngineModel = _options.GoogleEngineModel;
				ProjectLocation = _options.ProjectLocation;
				GlossaryPath = _options.GlossaryPath;
				BasicCsvGlossary = _options.BasicCsv;

				Region = Regions.FirstOrDefault(a => a.Key == (_options.Region ?? ""));
			}
			

			SetTranslationOption();

			SetGoogleApiVersion();
		}
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(Clear));

		private void Clear(object obj)
		{
			if (!(obj is string objectName)) return;

			switch (objectName)
			{
				case "CategoryId":
					CatId = string.Empty;
					break;
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
				case "PeUrl":
					PeUrl = string.Empty;
					break;
			}
		}

		public ModelBase ViewModel { get; set; }

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
				IsV2Checked = _selectedGoogleApiVersion.Version == Enums.GoogleApiVersion.V2;
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
				IsMicrosoftSelected = value.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslator 
										|| value.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe;
				IsMicrosoftWithPeSelected = value.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe;
				OnPropertyChanged(nameof(SelectedTranslationOption));
				ClearMessageRaised?.Invoke();
			}
		}

		public bool IsMicrosoftSelected
		{
			get => _isMicrosoftSelected;
			set
			{
				if (_isMicrosoftSelected == value) return;
				_isMicrosoftSelected = value;
				if (_isMicrosoftSelected || SelectedGoogleApiVersion.Version == Enums.GoogleApiVersion.V3)
				{
					IsV2Checked = false;
				}
				else
				{
					IsV2Checked = true;
				}
				OnPropertyChanged(nameof(IsMicrosoftSelected));
				ClearMessageRaised?.Invoke();
			}
		}

		public bool IsMicrosoftWithPeSelected
		{
			get => _isMicrosoftWithPeSelected;
			set
			{
				if (_isMicrosoftWithPeSelected == value) return;
				_isMicrosoftWithPeSelected = value;
				OnPropertyChanged(nameof(IsMicrosoftWithPeSelected));
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

		public string PeUrl
		{
			get => _peUrl;
			set
			{
				if (_peUrl == value) return;
				_peUrl = value.Trim();
				OnPropertyChanged(nameof(PeUrl));
				ClearMessageRaised?.Invoke();
			}
		}

		public string ClientId
		{
			get => _clientId;
			set
			{
				if (_clientId == value) return;
				_clientId = value.Trim();
				OnPropertyChanged(nameof(ClientId));
				ClearMessageRaised?.Invoke();
			}
		}

		public SubscriptionRegion Region
		{
			get => _region;
			set
			{
				if (_region == value)
				{
					return;
				}

				_region = value;
				OnPropertyChanged(nameof(Region));
				ClearMessageRaised?.Invoke();
			}
		}

		public ObservableCollection<SubscriptionRegion> Regions
		{
			get
			{
				return _regions ?? (_regions = new ObservableCollection<SubscriptionRegion>(
			  _regionsProvider.GetSubscriptionRegions()));
			}
			set
			{
				_regions = value;
				OnPropertyChanged(nameof(Region));
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

		public bool UseCatId
		{
			get => _useCatId;
			set
			{
				if (_useCatId == value) return;
				_useCatId = value;
				if (!value)
				{
					CatId = string.Empty;
				}
				OnPropertyChanged(nameof(UseCatId));
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

		public bool PersistMicrosoftKey
		{
			get => _persistMicrosoftKey;
			set
			{
				if (_persistMicrosoftKey == value) return;
				_persistMicrosoftKey = value;
				OnPropertyChanged(nameof(PersistMicrosoftKey));
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

		public string CatId
		{
			get => _catId;
			set
			{
				if (_catId == value) return;
				_catId = value;
				OnPropertyChanged(nameof(CatId));
			}
		}

		private void SetTranslationOption()
		{
			if (_options?.SelectedProvider != null)
			{
				var selectedProvider = TranslationOptions.FirstOrDefault(t => t.ProviderType.Equals(_options.SelectedProvider));

				if (selectedProvider == null)
				{
					SelectMicrosoftTranslation();
				}
				else
				{
					switch (selectedProvider.ProviderType)
					{
						case MtTranslationOptions.ProviderType.GoogleTranslate:
							IsMicrosoftSelected = false;
							IsMicrosoftWithPeSelected = false;
							break;
						case MtTranslationOptions.ProviderType.MicrosoftTranslator:
							IsMicrosoftSelected = true;
							IsMicrosoftWithPeSelected = false;
							break;
						case MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe:
							IsMicrosoftSelected = true;
							IsMicrosoftWithPeSelected = true;
							break;
						case MtTranslationOptions.ProviderType.None:
							IsMicrosoftSelected = false;
							IsMicrosoftWithPeSelected = false;
							break;
					}
					SelectedTranslationOption = selectedProvider;
				}
			}
			else
			{
				//By default we'll select Microsoft translator option
				SelectMicrosoftTranslation();
			}
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

		private void SelectMicrosoftTranslation()
		{
			SelectedTranslationOption = TranslationOptions[0];
			IsMicrosoftSelected = true;
		}

		private void SelectGoogleV2()
		{
			SelectedGoogleApiVersion = GoogleApiVersions[0];
			IsV2Checked = true;
		}
	}
}
