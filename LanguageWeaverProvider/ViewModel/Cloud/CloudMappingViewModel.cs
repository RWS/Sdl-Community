using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Database.Interface;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.NewFolder;
using LanguageWeaverProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudMappingViewModel : BaseViewModel, IPairMappingViewModel
	{
		private readonly ILanguageMappingDatabase _languageMappingDatabase;
		private readonly ITranslationOptions _translationOptions;
		private readonly LanguagePair[] _langaugePairs;

		private ObservableCollection<PairMapping> _pairMappings;

		public CloudMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_langaugePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = new LanguageMappingDatabase("cloud", DatabaseControl.GetCloudLanguageCodes());
			_ = LoadPairMappingAsync();
		}

		public ObservableCollection<PairMapping> PairMappings
		{
			get => _pairMappings;
			set
			{
				if (_pairMappings == value) return;
				_pairMappings = value;
				OnPropertyChanged();
			}
		}

		private async Task LoadPairMappingAsync()
		{
			if (_translationOptions.PairMappings is null
			 || !_translationOptions.PairMappings.Any())
			{
				await CreatePairMappingAsync();
				return;
			}

			var pairMappings = new ObservableCollection<PairMapping>();
			foreach (var pairMapping in _translationOptions.PairMappings)
			{
				var selectedModelName = pairMapping.SelectedModel.Name;
				var selectedModel = pairMapping.Models.FirstOrDefault(x => x.Name == selectedModelName);

				var selectedDictionaryId = pairMapping.SelectedDictionary.DictionaryId;
				var selectedDictionary = pairMapping.Dictionaries.FirstOrDefault(x => x?.DictionaryId == selectedDictionaryId);

				var newPairMapping = new PairMapping()
				{
					DisplayName = pairMapping.DisplayName,
					SourceCode = pairMapping.SourceCode,
					TargetCode = pairMapping.TargetCode,
					LanguagePair = pairMapping.LanguagePair,
					Models = pairMapping.Models,
					SelectedModel = selectedModel,
					Dictionaries = pairMapping.Dictionaries,
					SelectedDictionary = selectedDictionary,
				};

				pairMappings.Add(newPairMapping);
			}

			PairMappings = pairMappings;
		}

		private async Task CreatePairMappingAsync()
		{
			PairMappings = new();
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			var accountModels = await CloudService.GetSupportedLanguages(_translationOptions.CloudCredentials);
			var accountDictionaries = await CloudService.GetDictionaries(_translationOptions.CloudCredentials);
			foreach (var languagePair in _langaugePairs)
			{
				var lps = mappedLanguages.Where(x => x.TradosCode.Equals(languagePair.SourceCultureName) || x.TradosCode.Equals(languagePair.TargetCultureName));

				var source = lps.FirstOrDefault(x => x.TradosCode.Equals(languagePair.SourceCultureName));
				var target = lps.FirstOrDefault(x => x.TradosCode.Equals(languagePair.TargetCultureName));

				var models = accountModels.Where(model => model.SourceLanguageId.Equals(source.LanguageCode) && model.TargetLanguageId.Equals(target.LanguageCode)).ToList();
				if (!models.Any())
				{
					models.Add(new()
					{
						Name = "No model available",
						DisplayName = "No model available",
						SourceLanguageId = source.LanguageCode,
						TargetLanguageId = target.LanguageCode
					});
				}

				var dictionaries = accountDictionaries.Where(dictionary => dictionary.Source.Equals(source.LanguageCode) && dictionary.Target.Equals(target.LanguageCode)).ToList();
				dictionaries.Insert(0, new()
				{
					Name = dictionaries.Any() ? "No dictionary selected" : "No dictionary available",
					DictionaryId = string.Empty,
					Source = source.LanguageCode,
					Target = target.LanguageCode,
				});

				var newPairMapping = new PairMapping
				{
					DisplayName = $"{source.Name} ({source.Region}) - {target.Name} ({target.Region})",
					LanguagePair = languagePair,
					SourceCode = source.LanguageCode,
					TargetCode = target.LanguageCode,
					Models = models,
					SelectedModel = models.FirstOrDefault(),
					Dictionaries = dictionaries,
					SelectedDictionary = dictionaries.FirstOrDefault()
				};

				PairMappings.Add(newPairMapping);
			}
		}
	}
}