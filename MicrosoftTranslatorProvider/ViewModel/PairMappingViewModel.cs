using System;
using System.Collections.Generic;
using System.Linq;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Database.Interface;
using MicrosoftTranslatorProvider.ApiService;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class PairMappingViewModel : LanguageMappingProvider.Model.BaseModel
    {
		readonly ITranslationOptions _translationOptions;
		readonly ILanguageMappingDatabase _languageMappingDatabase;
		readonly LanguagePair[] _languagePairs;

		public PairMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_languagePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = new LanguageMappingDatabase("microsoft", null);
			LoadPairMapping();
		}

		public List<PairModel> PairModels { get; set; }

		private async void LoadPairMapping()
		{
			CreatePairMappings();
		}

		private void CreatePairMappings()
		{
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			foreach (var languagePair in _languagePairs)
			{
				var mappedLanguagePairs = mappedLanguages.Where(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName) || mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var mappedSource = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName));
				var mappedTarget = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var displayName = $"{mappedSource.Name} ({mappedSource.Region}) - {mappedTarget?.Name} ({mappedTarget?.Region})";

				var pairModel = new PairModel()
				{
					DisplayName = displayName,
					SourceLanguageCode = mappedSource.LanguageCode,
					TargetLanguageCode = mappedTarget.LanguageCode,
					TradosLanguagePair = languagePair
				};

				PairModels ??= [];
				PairModels.Add(pairModel);
			}
		}
	}
}