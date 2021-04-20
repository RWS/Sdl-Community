using System.Net.Http;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ITranslationService
	{
		event TranslationReceivedEventHandler TranslationReceived;

		IConnectionService ConnectionService { get; }
		Options Options { get; set; }

		Task<MTCloudDictionaryInfo> GetDictionaries();

		Task<SubscriptionInfo> GetLanguagePairs();

		Task<HttpResponseMessage> SendFeedback(SegmentId? segmentId, dynamic rating, string original, string improvement, QualityEstimation qualityEstimation);

		Task<Segment[]> TranslateText(string text, LanguageMappingModel model, FileAndSegmentIds fileAndSegmentIds);
		Task AddTermToDictionary(Term term);
	}
}