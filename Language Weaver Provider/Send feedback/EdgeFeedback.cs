using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Services.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Send_feedback
{
    public class EdgeFeedback(ISegmentPair segmentPair) : LanguageWeaverFeedback
    {
        public string Comment { get; set; }

        public override async Task<bool> Send()
        {
            var accessToken = GetAccessToken(Constants.EdgeFullScheme);
            if (accessToken is null) return false;

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

            return true;
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
                Comment = Comment,
                SuggestedTranslation = suggestedTranslation
            };
            return feedbackItem;
        }
    }
}