using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class ProviderViewModel : BaseModel, IProviderViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private readonly ITranslationOptions _options;

		private LanguageMapping _selectedLanguageMapping;
		private List<LanguageMapping> _languageMappings;
		private List<RegionSubscription> _regions;
		private RegionSubscription _selectedRegion;

		private bool _editProvider;
		private bool _persistMicrosoftKey;
		private string _apiKey;

		private ICommand _learnMoreCommand;

		public ProviderViewModel(ITranslationOptions options, LanguagePair[] languagePairs, bool editProvider)
		{
			_options = options;
			_languagePairs = languagePairs;
			EditProvider = editProvider;
			InitializeComponent();
			CreateMapping();
		}

		public BaseModel ViewModel => this;

		public bool EditProvider
		{
			get => _editProvider;
			set
			{
				if (_editProvider == value) return;
				_editProvider = value;
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

		public RegionSubscription SelectedRegion
		{
			get => _selectedRegion;
			set
			{
				if (_selectedRegion == value) return;
				_selectedRegion = value;
				OnPropertyChanged();
			}
		}

		public List<RegionSubscription> Regions
		{
			get
			{
				return _regions ??= new List<RegionSubscription>(new RegionsProvider().GetSubscriptionRegions());
			}
			set
			{
				if (_regions == value) return;
				_regions = value;
				OnPropertyChanged();
			}
		}

		public bool PersistMicrosoftKey
		{
			get => _persistMicrosoftKey;
			set
			{
				if (_persistMicrosoftKey == value) return;
				_persistMicrosoftKey = value;
				OnPropertyChanged();
			}
		}

		public List<LanguageMapping> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				if (_languageMappings == value) return;
				_languageMappings = value;
				OnPropertyChanged();
			}
		}

		public LanguageMapping SelectedLanguageMapping
		{
			get => _selectedLanguageMapping;
			set
			{
				if (_selectedLanguageMapping == value) return;
				_selectedLanguageMapping = value;
				OnPropertyChanged();
			}
		}

		public ICommand LearnMoreCommand => _learnMoreCommand ??= new RelayCommand(parameter => { Process.Start(parameter as string); });

		private void InitializeComponent()
		{
			ApiKey = _options.ApiKey;
			PersistMicrosoftKey = _options.PersistMicrosoftCredentials;
			SelectedRegion = Regions.FirstOrDefault(a => a.Key == _options.Region) ?? Regions.ElementAt(0);
		}

		private void CreateMapping()
		{
			if (_options.LanguageMappings is not null)
			{
				LoadLanguageMappings();
				return;
			}

			var mapping = new List<LanguageMapping>();
			foreach (var pair in _languagePairs)
			{
				mapping.Add(new()
				{
					DisplayName = $"{pair.SourceCulture.DisplayName} - {pair.TargetCulture.DisplayName}",
					CategoryID = string.Empty,
					LanguagePair = pair
				});
			}

			LanguageMappings = mapping;
		}

		private void LoadLanguageMappings()
		{
			var mapping = new List<LanguageMapping>();
			foreach (var mappedLanguage in _options.LanguageMappings)
			{
				mapping.Add(new LanguageMapping()
				{
					DisplayName = mappedLanguage.DisplayName,
					CategoryID = mappedLanguage.CategoryID,
					LanguagePair = mappedLanguage.LanguagePair
				});
			}

			LanguageMappings = mapping;
		}
	}
}