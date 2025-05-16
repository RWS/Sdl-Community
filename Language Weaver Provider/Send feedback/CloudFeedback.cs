using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Send_feedback
{
    public class CloudFeedback(ISegmentPair segmentPair) : LanguageWeaverFeedback
    {
        public string OriginalQe { get; set; }

        public string Qe { get; set; }

        public Rating Rating { get; set; }

        public override async Task<bool> Send()
        {
            var feedbackItem = GetFeedbackRequest(segmentPair);
            var accessToken = GetAccessToken(Constants.CloudFullScheme);
            if (accessToken is null) return false;

            var translationOrigin = segmentPair.Properties.TranslationOrigin;
            var feedbackId = translationOrigin.GetMetaData(Constants.SegmentMetadata_FeedbackId);

            if (!string.IsNullOrWhiteSpace(feedbackId))
                await CloudService.UpdateFeedback(accessToken, feedbackId, feedbackItem).ConfigureAwait(false);
            else
            {
                feedbackId = await CloudService.SendFeedback(accessToken, feedbackItem).ConfigureAwait(false);
                translationOrigin.SetMetaData(Constants.SegmentMetadata_FeedbackId, feedbackId);
            }

            return true;
        }

        private CloudFeedbackItem GetFeedbackRequest(ISegmentPair segmentPair)
        {
            var translationOrigin = segmentPair.Properties.TranslationOrigin;

            var modelName = translationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
            var sourceCode = modelName.Substring(0, 3);
            var targetCode = modelName.Substring(3, 3);

            var qualityEstimationMt = translationOrigin.GetMetaData(Constants.SegmentMetadata_QE);
            if (string.IsNullOrWhiteSpace(qualityEstimationMt)) qualityEstimationMt = null;
            var feedback = new CloudFeedbackItem
            {
                Translation = new Translation
                {
                    SourceLanguageId = sourceCode,
                    TargetLanguageId = targetCode,
                    Model = translationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName),
                    SourceText = segmentPair.Source.ToString(),
                    TargetMTText = translationOrigin.GetMetaData(Constants.SegmentMetadata_Translation),
                    QualityEstimationMT = qualityEstimationMt
                },
                Improvement = new Improvement(segmentPair.Target.ToString()),
                Rating = Rating
            };
            return feedback;
        }
    }
}