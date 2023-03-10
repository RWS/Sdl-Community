using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class ProviderControlViewModel : BaseModel, IProviderControlViewModel
	{
		private readonly ITranslationOptions _options;
		private readonly RegionsProvider _regionsProvider;

		private ObservableCollection<RegionSubscription> _regions;
		private TranslationOption _selectedTranslationOption;
		private RegionSubscription _region;

		private bool _isMicrosoftSelected;
		private bool _useCategoryID;
		private bool _persistMicrosoftKey;
		private bool _isTellMeAction;
		private bool _basicCsvGlossary;

		private string _catId;
		private string _clientId;
		private string _jsonFilePath;
		private string _projectName;
		private string _projectLocation;
		private string _glossaryId;
		private string _glossaryPath;

		private ICommand _clearCommand;
		private ICommand _learnMoreCommand;

		public ProviderControlViewModel(ITranslationOptions options, RegionsProvider regionsProvider)
		{
			ViewModel = this;
			_options = options;
			_regionsProvider = regionsProvider;
			InitializeComponent();
		}

		public BaseModel ViewModel { get; set; }

		public List<TranslationOption> TranslationOptions { get; set; }

		public string ProjectLocation
		{
			get => _projectLocation;
			set
			{
				if (_projectLocation == value) return;
				_projectLocation = value;
				OnPropertyChanged(nameof(ProjectLocation));
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
			}
		}

		public TranslationOption SelectedTranslationOption
		{
			get => _selectedTranslationOption;
			set
			{
				if (_selectedTranslationOption == value) return;
				_selectedTranslationOption = value;
				IsMicrosoftSelected = value.ProviderType == MTETranslationOptions.ProviderType.MicrosoftTranslator;
				OnPropertyChanged(nameof(SelectedTranslationOption));
			}
		}

		public bool IsMicrosoftSelected
		{
			get => _isMicrosoftSelected;
			set
			{
				if (_isMicrosoftSelected == value) return;
				_isMicrosoftSelected = value;
				OnPropertyChanged(nameof(IsMicrosoftSelected));
			}
		}
		public string ClientID
		{
			get => _clientId;
			set
			{
				if (_clientId == value) return;
				_clientId = value.Trim();
				OnPropertyChanged(nameof(ClientID));
			}
		}

		public RegionSubscription Region
		{
			get => _region;
			set
			{
				if (_region == value) return;
				_region = value;
				OnPropertyChanged(nameof(Region));
			}
		}

		public ObservableCollection<RegionSubscription> Regions
		{
			get
			{
				return _regions ??= new ObservableCollection<RegionSubscription>(_regionsProvider.GetSubscriptionRegions());
			}
			set
			{
				if (_regions == value) return;
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
			}
		}

		public bool UseCategoryID
		{
			get => _useCategoryID;
			set
			{
				if (_useCategoryID == value) return;
				_useCategoryID = value;
				if (!value)
				{
					CategoryID = string.Empty;
				}

				OnPropertyChanged(nameof(UseCategoryID));
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

		public string CategoryID
		{
			get => _catId;
			set
			{
				if (_catId == value) return;
				_catId = value;
				OnPropertyChanged(nameof(CategoryID));
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public ICommand LearnMoreCommand => _learnMoreCommand ??= new RelayCommand(NavigateTo);

		private void InitializeComponent()
		{
			TranslationOptions = new List<TranslationOption>
			{
				new TranslationOption
				{
					Name = PluginResources.Microsoft,
					ProviderType = MTETranslationOptions.ProviderType.MicrosoftTranslator
				}
			};

			Region = Regions.FirstOrDefault(a => a.Key == "");
			if (_options == null)
			{
				SetTranslationOption();
				return;
			}

			BasicCsvGlossary = _options.BasicCsv;
			CategoryID = _options.CategoryID;
			ClientID = _options.ClientID;
			GlossaryPath = _options.GlossaryPath;
			JsonFilePath = _options.JsonFilePath;
			PersistMicrosoftKey = _options.PersistMicrosoftCredentials;
			ProjectLocation = _options.ProjectLocation;
			ProjectName = _options.ProjectName;
			UseCategoryID = _options.UseCategoryID;
			PrivateEndpoint = _options.PrivateEndpoint;
			PersistPrivateEndpoint = _options.PersistPrivateEndpoint;
			UsePrivateEndpoint = !string.IsNullOrEmpty(PrivateEndpoint);
			Region = Regions.FirstOrDefault(a => a.Key == (_options.Region ?? ""));
			SetTranslationOption();
		}

		private void SetTranslationOption()
		{
			IsMicrosoftSelected = true;
			SelectMicrosoftTranslation();
		}

		private void SelectMicrosoftTranslation()
		{
			SelectedTranslationOption = TranslationOptions[0];
			IsMicrosoftSelected = true;
		}

		private void Clear(object parameter)
		{
			switch (parameter as string)
			{
				case "CategoryId":
					CategoryID = string.Empty;
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
				case "PrivateEndpoint":
					PrivateEndpoint = string.Empty;
					break;
			}
		}

		private void NavigateTo(object parameter)
		{
			Process.Start(parameter as string);
		}


		private bool _usePrivateEndpoint;
		public bool UsePrivateEndpoint
		{
			get => _usePrivateEndpoint;
			set
			{
				if (_usePrivateEndpoint == value) return;
				_usePrivateEndpoint = value;
				OnPropertyChanged(nameof(UsePrivateEndpoint));
			}
		}

		private string _privateEndpoint;
		public string PrivateEndpoint
		{
			get => _privateEndpoint;
			set
			{
				if (_privateEndpoint ==	value) return;
				_privateEndpoint = value;
				OnPropertyChanged(nameof(PrivateEndpoint));
			}
		}

		private bool _persistPrivateEndpoint;
		public bool PersistPrivateEndpoint
		{
			get => _persistPrivateEndpoint;
			set
			{
				if (PersistPrivateEndpoint == value) return;
				_persistPrivateEndpoint = value;
				OnPropertyChanged(nameof(PersistPrivateEndpoint));
			}
		}
	}
}