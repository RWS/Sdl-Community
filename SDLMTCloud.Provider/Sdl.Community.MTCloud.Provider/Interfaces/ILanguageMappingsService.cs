using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ILanguageMappingsService
	{
		SubscriptionInfo SubscriptionInfo { get; }

		List<MTCloudDictionary> Dictionaries { get; }

		List<TranslationModel> GetTranslationModels(MTCloudLanguage mtCloudSource, MTCloudLanguage mtCloudTarget,
			string source, string target);

		List<MTCloudDictionary> GetDictionaries(MTCloudLanguage mtCloudSource, MTCloudLanguage mtCloudTarget);

		List<MTCloudLanguage> GetMTCloudLanguage(Languages.Provider.Model.Language mtCloudLanguage,
			CultureInfo language);
	}
}
