using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MTEnhancedMicrosoftProvider.Commands;
using MTEnhancedMicrosoftProvider.Interfaces;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;

namespace MTEnhancedMicrosoftProvider.ViewModel
{
	public class ProviderControlViewModel : BaseModel, IProviderControlViewModel
	{
		private readonly ITranslationOptions _options;
		private ICommand _clearCommand;
		private readonly RegionsProvider _regionsProvider;
		private TranslationOption _selectedTranslationOption;
		private bool _isMicrosoftSelected;
		private bool _useCatId;
		private bool _isV2Checked;
		private bool _persistGoogleKey;
		private bool _persistMicrosoftKey;
		private bool _isTellMeAction;
		private bool _basicCsvGlossary;
		private string _catId;
		private string _apiKey;
		private string _clientId;
		private RegionSubscription _region;
		private ObservableCollection<RegionSubscription> _regions;
		private string _jsonFilePath;
		private string _projectName;
		private string _googleEngineModel;
		private string _projectLocation;
		private string _glossaryId;
		private string _glossaryPath;

		public ProviderControlViewModel(ITranslationOptions options, RegionsProvider regionsProvider)
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
					ProviderType = MTEMicrosoftTranslationOptions.ProviderType.MicrosoftTranslator
				}
			};

			// set the default region
			Region = Regions.FirstOrDefault(a => a.Key == "");
			if (_options != null)
			{
				ClientId = _options.ClientId;
				PersistMicrosoftKey = _options.PersistMicrosoftCreds;
				UseCatId = _options.UseCatID;
				CatId = _options.CatId;

				JsonFilePath = _options.JsonFilePath;
				ProjectName = _options.ProjectName;
				ProjectLocation = _options.ProjectLocation;
				GlossaryPath = _options.GlossaryPath;
				BasicCsvGlossary = _options.BasicCsv;

				Region = Regions.FirstOrDefault(a => a.Key == (_options.Region ?? ""));
			}
			

			SetTranslationOption();
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
				case "GlossaryPath":
					GlossaryPath = string.Empty;
					break;
			}
		}

		public BaseModel ViewModel { get; set; }

		public ICommand ShowSettingsCommand { get; set; }

		public List<TranslationOption> TranslationOptions { get; set; }

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

		public TranslationOption SelectedTranslationOption
		{
			get => _selectedTranslationOption;
			set
			{
				_selectedTranslationOption = value;
				IsMicrosoftSelected = value.ProviderType == MTEMicrosoftTranslationOptions.ProviderType.MicrosoftTranslator;
				OnPropertyChanged(nameof(SelectedTranslationOption));
				ClearMessageRaised?.Invoke();
			}
		}

		public bool IsMicrosoftSelected
		{
			get => _isMicrosoftSelected;
			set
			{
				if (_isMicrosoftSelected == value)
				{
					return;
				}

				OnPropertyChanged(nameof(IsMicrosoftSelected));
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

		public RegionSubscription Region
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

		public ObservableCollection<RegionSubscription> Regions
		{
			get
			{
				return _regions ?? (_regions = new ObservableCollection<RegionSubscription>(
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
			/*if (_options?.SelectedProvider != null)
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
						case MTEMicrosoftTranslationOptions.ProviderType.MicrosoftTranslator:
							IsMicrosoftSelected = true;
							break;
						case MTEMicrosoftTranslationOptions.ProviderType.None:
							IsMicrosoftSelected = false;
							break;
					}
					SelectedTranslationOption = selectedProvider;
				}
			}
			else
			{
				//By default we'll select Microsoft translator option
				SelectMicrosoftTranslation();
			}*/
		}

		private void SelectMicrosoftTranslation()
		{
			SelectedTranslationOption = TranslationOptions[0];
			IsMicrosoftSelected = true;
		}
	}
}