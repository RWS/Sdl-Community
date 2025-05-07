using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Services.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Send_feedback
{
    public class EdgeFeedback(ISegmentPair segmentPair)
    {
        public async Task Send(AccessToken accessToken)
        {
            var translationOrigin = segmentPair.Properties.TranslationOrigin;
            var feedbackItem = GetFeedbackItem(translationOrigin);
            var feedbackId = translationOrigin.GetMetaData(Constants.SegmentMetadata_FeedbackId);

            if (!string.IsNullOrWhiteSpace(feedbackId))
                await EdgeService.UpdateFeedback(accessToken, feedbackId, feedbackItem);
            else
            {
                feedbackId = await EdgeService.SendFeedback(accessToken, feedbackItem);
                translationOrigin.SetMetaData(Constants.SegmentMetadata_FeedbackId, feedbackId);
            }
        }

        private EdgeFeedbackItem GetFeedbackItem(ITranslationOrigin translationOrigin)
        {
            var languagePairId = translationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName);
            var originalTranslation = translationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);

            var targetText = segmentPair.Target.ToString();
            var suggestedTranslation = !originalTranslation.Equals(targetText) ? targetText : null;

            var sourceText = segmentPair.Source.ToString();
            var feedbackItem = new EdgeFeedbackItem
            {
                SourceText = sourceText,
                LanguagePairId = languagePairId,
                MachineTranslation = originalTranslation,
                Comment = null,
                SuggestedTranslation = suggestedTranslation
            };
            return feedbackItem;
        }
    }
}