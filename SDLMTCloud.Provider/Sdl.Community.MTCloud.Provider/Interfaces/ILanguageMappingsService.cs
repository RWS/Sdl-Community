using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ILanguageMappingsService
	{
		SubscriptionInfo SubscriptionInfo { get; }

		List<MTCloudDictionary> Dictionaries { get; }

		/// <summary>
		/// Gets a list of the available translation models for the MT Cloud language pair
		/// </summary>
		/// <param name="mtCloudSource">The source MT Cloud language code</param>
		/// <param name="mtCloudTarget">The target MT Cloud language code</param>
		/// <param name="source">The source langauge name used by Studio (e.g. en-US, it-IT...)</param>
		/// <param name="target">The target langauge name used by Studio (e.g. en-US, it-IT...)</param>
		/// <returns>Returns a list of TranslationModel that correspond to the MT Cloud language pair</returns>
		List<TranslationModel> GetTranslationModels(MTCloudLanguage mtCloudSource, MTCloudLanguage mtCloudTarget, string source, string target);

		/// <summary>
		/// Gets a list of available dictionaries for the MT Cloud langauge pair
		/// </summary>
		/// <param name="mtCloudSource">The source MT Cloud language code</param>
		/// <param name="mtCloudTarget">The target MT Cloud language code</param>
		/// <returns>Returns a list of MTCloudDictionary that correspond to the MT Cloud language pair</returns>
		List<MTCloudDictionary> GetDictionaries(MTCloudLanguage mtCloudSource, MTCloudLanguage mtCloudTarget);


		List<MTCloudLanguage> GetMTCloudLanguages(MappedLanguage mappedLanguage, CultureInfo cultureInfo);
	}
}