using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ITranslationService
	{
		IConnectionService ConnectionService { get; }

		ILanguageMappingsService LanguageMappingsService { get; }

		List<LanguageMappingModel> LanguageMappings { get; }

		void UpdateLanguageMappings();

		Task<Segment[]> TranslateText(string text, string source, string target);

		Task<SubscriptionInfo> GetLanguagePairs(string accountId);

		Task<MTCloudDictionaryInfo> GetDictionaries(string accountId);
	}
}
