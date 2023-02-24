using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ITranslationService
	{
		event TranslationReceivedEventHandler TranslationReceived;

		IConnectionService ConnectionService { get; }
		bool IsActiveModelQeEnabled { get; }
		Options Options { get; set; }

		Task AddTermToDictionary(Term term);

		Task<MTCloudDictionaryInfo> GetDictionaries();

		Task<SubscriptionInfo> GetLanguagePairs();

		Task<LinguisticOptions> GetLinguisticOptions(string modelName);

		Task<HttpResponseMessage> SendFeedback(FeedbackInfo feedbackInfo);

		Task<Segment[]> TranslateText(string text, LanguageMappingModel model, FileAndSegmentIds fileAndSegmentIds);
	}
}