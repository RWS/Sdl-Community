using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class ProviderControlViewModel : BaseModel, IProviderControlViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private readonly ITranslationOptions _options;
		private readonly RegionsProvider _regionsProvider;
		private readonly bool _showSettingsView;

		private ObservableCollection<RegionSubscription> _regions;
		private TranslationOption _selectedTranslationOption;
		private List<LanguageMapping> _languageMappings;
		private RegionSubscription _region;

		private bool _isMicrosoftSelected;
		private bool _useCategoryID;
		private bool _persistMicrosoftKey;
		private bool _isTellMeAction;

		private string _catId;
		private string _clientId;

		private ICommand _clearCommand;
		private ICommand _learnMoreCommand;

		public ProviderControlViewModel(ITranslationOptions options, RegionsProvider regionsProvider, LanguagePair[] languagePairs)
		{
			ViewModel = this;
			_options = options;
			_languagePairs = languagePairs;
			_regionsProvider = regionsProvider;
			InitializeComponent();
			CreateMapping();
		}

		public BaseModel ViewModel { get; set; }

		public List<TranslationOption> TranslationOptions { get; set; }

		public TranslationOption SelectedTranslationOption
		{
			get => _selectedTranslationOption;
			set
			{
				if (_selectedTranslationOption == value) return;
				_selectedTranslationOption = value;
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

		public List<LanguageMapping> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				if (_languageMappings == value) return;
				_languageMappings = value;
				OnPropertyChanged(nameof(LanguageMappings));
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
					Name = PluginResources.Microsoft
				}
			};

			Region = Regions.FirstOrDefault(a => a.Key == "");
			if (_options == null)
			{
				SetTranslationOption();
				return;
			}

			CategoryID = _options.CategoryID;
			ClientID = _options.ClientID;
			PersistMicrosoftKey = _options.PersistMicrosoftCredentials;
			UseCategoryID = _options.UseCategoryID;
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
			}
		}

		private void NavigateTo(object parameter)
		{
			Process.Start(parameter as string);
		}

		private void CreateMapping()
		{
			var mapping = new List<LanguageMapping>();
			foreach (var pair in _languagePairs)
			{
				mapping.Add(new()
				{
					DisplayName = $"{pair.SourceCulture.DisplayName} - {pair.TargetCulture.DisplayName}",
					CategoryID = string.Empty,
					LanguagePair = pair,
					Regions = _regionsProvider.GetSubscriptionRegions(),
					Region = Regions.First(),
					RegioKey = Region.Key
			});
			}

			LanguageMappings = mapping;
		}
	}
}