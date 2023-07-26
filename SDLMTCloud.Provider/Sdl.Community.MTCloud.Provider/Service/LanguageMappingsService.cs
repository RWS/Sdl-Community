using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Core.Globalization;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class LanguageMappingsService : ILanguageMappingsService
	{
		private readonly ITranslationService _translationService;
		private List<MTCloudDictionary> _dictionaries;
		private SubscriptionInfo _subscriptionInfo;

		public LanguageMappingsService(ITranslationService translationService)
		{
			_translationService = translationService;
		}

		public List<MTCloudDictionary> Dictionaries
		{
			get
			{
				if (_dictionaries == null)
				{
					var result = Task.Run(async () => await _translationService.GetDictionaries()).Result;
					_dictionaries = result?.Dictionaries;
				}

				return _dictionaries;
			}
		}

		public SubscriptionInfo SubscriptionInfo
		{
			get
			{
				return _subscriptionInfo ??= Task.Run(async () =>
					await _translationService.GetLanguagePairs()).Result;
			}
		}

		/// <summary>
		/// Gets a list of available dictionaries for the MT Cloud langauge pair
		/// </summary>
		/// <param name="mtCloudSource">The source MT Cloud language code</param>
		/// <param name="mtCloudTarget">The target MT Cloud language code</param>
		/// <returns>Returns a list of MTCloudDictionary that correspond to the MT Cloud language pair</returns>
		public List<MTCloudDictionary> GetDictionaries(MTCloudLanguage mtCloudSource, MTCloudLanguage mtCloudTarget)
		{
			var cloudDictionaries = new List<MTCloudDictionary>();

			if (Dictionaries != null)
			{
				var dictionaries = Dictionaries.Where(a =>
					a.Source.Equals(mtCloudSource.CodeName, StringComparison.InvariantCultureIgnoreCase) &&
						a.Target.Equals(mtCloudTarget.CodeName, StringComparison.InvariantCultureIgnoreCase)).ToList();

				if (dictionaries.Any())
				{
					cloudDictionaries.AddRange(dictionaries);
				}

				if (cloudDictionaries.Count == 0)
				{
					cloudDictionaries.Add(new MTCloudDictionary { Name = PluginResources.Message_No_dictionary_available, DictionaryId = string.Empty });
				}
				else if (!cloudDictionaries.Exists(a => a.Name == PluginResources.Message_No_dictionary))
				{
					cloudDictionaries.Insert(0, new MTCloudDictionary { Name = PluginResources.Message_No_dictionary, DictionaryId = string.Empty });
				}
			}

			return cloudDictionaries;
		}

		public List<MTCloudLanguage> GetMTCloudLanguages(MappedLanguage mappedLanguage, CultureInfo cultureInfo)
		{
			var languageMappings = new List<MTCloudLanguage>();

			if (mappedLanguage != null)
			{
				languageMappings.Add(new MTCloudLanguage
				{
					CodeName = mappedLanguage.MTCode,
					IsLocale = false,
					Flag = SetLanguageFlag(cultureInfo)
				});

				if (!string.IsNullOrEmpty(mappedLanguage.MTCodeLocale))
				{
					languageMappings.Add(new MTCloudLanguage
					{
						CodeName = mappedLanguage.MTCodeLocale,
						IsLocale = true,
						Flag = SetLanguageFlag(cultureInfo)
					});
				}
			}

			return languageMappings;
		}

		/// <summary>
		/// Gets a list of the available translation models for the MT Cloud language pair
		/// </summary>
		/// <param name="mtCloudSource">The source MT Cloud language code</param>
		/// <param name="mtCloudTarget">The target MT Cloud language code</param>
		/// <param name="source">The source langauge name used by Studio (e.g. en-US, it-IT...)</param>
		/// <param name="target">The target langauge name used by Studio (e.g. en-US, it-IT...)</param>
		/// <returns>Returns a list of TranslationModel that correspond to the MT Cloud language pair</returns>
		public List<TranslationModel> GetTranslationModels(MTCloudLanguage mtCloudSource, MTCloudLanguage mtCloudTarget, string source, string target)
		{
			var translationModels = new List<TranslationModel>();

			if (SubscriptionInfo?.LanguagePairs == null)
			{
				return null;
			}

			var models = SubscriptionInfo?.LanguagePairs.Where(a => mtCloudSource.CodeName.Equals(a.SourceLanguageId, StringComparison.InvariantCultureIgnoreCase)
														 && mtCloudTarget.CodeName.Equals(a.TargetLanguageId, StringComparison.InvariantCultureIgnoreCase));

			foreach (var model in models)
			{
				translationModels.Add(new TranslationModel
				{
					Model = model.Model,
					MTCloudLanguagePair = model,
					DisplayName = $"{model.SourceLanguageId}-{model.TargetLanguageId} {model.DisplayName}",
					Source = source,
					Target = target,
					LinguisticOptions = SetLinguisticOptions(model.Name)
				});
			}

			if (translationModels.Count == 0)
			{
				translationModels.Add(new TranslationModel
				{
					Model = null,
					MTCloudLanguagePair = null,
					DisplayName = PluginResources.Message_No_model_available,
					Source = source,
					Target = target

				});
			}

			return translationModels;
		}

		private List<LinguisticOption> SetLinguisticOptions(string modelName)
		{
			var result = Task.Run(async () => await _translationService.GetLinguisticOptions(modelName)).Result;
			var availableLinguisticOptions = result?.AvailableLinguisticOptions;
			if (availableLinguisticOptions is null || !availableLinguisticOptions.Any())
			{
				return null;
			}

			if (!availableLinguisticOptions.Any())
			{
				return null;
			}

			foreach (var linguisticOption in availableLinguisticOptions)
			{
				if (linguisticOption.Name == "QualityEstimation")
				{
					linguisticOption.SystemDefault = linguisticOption.Values.FirstOrDefault(x => x == "Enabled");
					linguisticOption.SelectedValue = linguisticOption.Values.FirstOrDefault(x => x == "Enabled");
				}
			}

			return availableLinguisticOptions;
		}

		private static Image SetLanguageFlag(CultureInfo cultureInfo)
		{
			return new Language(cultureInfo).GetFlagImage();
		}
	}
}