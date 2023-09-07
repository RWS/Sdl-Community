using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LanguageMappingProvider.Database.Interface;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
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

		public CloudMappingViewModel(ITranslationOptions translationOptions, ILanguageMappingDatabase languageMappingDatabse, LanguagePair[] languagePairs)
		{
			_langaugePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = languageMappingDatabse;
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
			var originalPairMappings = PairMappings;
			PairMappings = new();
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			var accountModels = await CloudService.GetSupportedLanguages(_translationOptions.CloudCredentials);
			var accountDictionaries = await CloudService.GetDictionaries(_translationOptions.CloudCredentials);
			foreach (var languagePair in _langaugePairs)
			{
				var mappedLanguagePairs = mappedLanguages.Where(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName) || mappedLang.TradosCode.Equals(languagePair.TargetCultureName));

				var mappedSource = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName));
				var mappedTarget = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var displayName = $"{mappedSource.Name} ({mappedSource.Region}) - {mappedTarget.Name} ({mappedTarget.Region})";

				var currentModel = originalPairMappings?.FirstOrDefault(pair => pair.DisplayName.Equals(displayName));
				if (currentModel is not null
				 && mappedSource.LanguageCode.Equals(currentModel.SourceCode)
				 && mappedTarget.LanguageCode.Equals(currentModel.TargetCode))
				{
					PairMappings.Add(currentModel);
					continue;
				}

				var models = accountModels.Where(model => model.SourceLanguageId.Equals(mappedSource.LanguageCode) && model.TargetLanguageId.Equals(mappedTarget.LanguageCode)).ToList();
				if (!models.Any())
				{
					models.Add(new PairModel()
					{
						Name = PluginResources.PairModel_Model_Unavailable,
						DisplayName = PluginResources.PairModel_Model_Unavailable,
						SourceLanguageId = mappedSource.LanguageCode,
						TargetLanguageId = mappedTarget.LanguageCode
					});
				}

				var dictionaries = accountDictionaries.Where(dictionary => dictionary.Source.Equals(mappedSource.LanguageCode) && dictionary.Target.Equals(mappedTarget.LanguageCode)).ToList();
				dictionaries.Insert(0, new PairDictionary()
				{
					Name = dictionaries.Any() ? PluginResources.PairModel_Dictionary_NotSelected : PluginResources.PairModel_Dictionary_Unavailable,
					DictionaryId = string.Empty,
					Source = mappedSource.LanguageCode,
					Target = mappedTarget.LanguageCode,
				});

				var newPairMapping = new PairMapping
				{
					DisplayName = displayName,
					LanguagePair = languagePair,
					SourceCode = mappedSource.LanguageCode,
					TargetCode = mappedTarget.LanguageCode,
					Models = models,
					SelectedModel = models.FirstOrDefault(),
					Dictionaries = dictionaries,
					SelectedDictionary = dictionaries.FirstOrDefault()
				};

				PairMappings.Add(newPairMapping);
			}
		}

		public void UpdateLanguageMapping()
			=> _ = CreatePairMappingAsync();
	}
}