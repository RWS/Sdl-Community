using System.Net.Http;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public delegate void TranslationFeedbackEventRaiser(FeedbackRequest translationFeedback);

	public interface ITranslationService
	{
		IConnectionService ConnectionService { get; }

		Options Options { get; set; }

		Task<MTCloudDictionaryInfo> GetDictionaries(string accountId);

		Task<SubscriptionInfo> GetLanguagePairs(string accountId);

		Task<HttpResponseMessage> SendFeedback(SegmentId? segmentId, dynamic rating, string original, string improvement);

		Task<Segment[]> TranslateText(string text, LanguageMappingModel model);
	}
}