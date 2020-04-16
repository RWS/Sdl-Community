using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ITranslationService
	{
		IConnectionService ConnectionService { get; }	

		Task<Segment[]> TranslateText(string text, LanguageMappingModel model);

		Task<SubscriptionInfo> GetLanguagePairs(string accountId);

		Task<MTCloudDictionaryInfo> GetDictionaries(string accountId);
	}
}
