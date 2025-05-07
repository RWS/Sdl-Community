using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Send_feedback
{
    public class CloudFeedback(ISegmentPair segmentPair)
    {
        public async Task Send(AccessToken accessToken)
        {
            var translationOrigin = segmentPair.Properties.TranslationOrigin;
            var feedbackItem = GetCloudFeedbackRequest(segmentPair);
            var feedbackId = translationOrigin.GetMetaData(Constants.SegmentMetadata_FeedbackId);

            if (!string.IsNullOrWhiteSpace(feedbackId))
                await CloudService.UpdateFeedback(accessToken, feedbackId, feedbackItem);
            else
            {
                feedbackId = await CloudService.SendFeedback(accessToken, feedbackItem);
                translationOrigin.SetMetaData(Constants.SegmentMetadata_FeedbackId, feedbackId);
            }
        }

        private static CloudFeedbackItem GetCloudFeedbackRequest(ISegmentPair segmentPair)
        {
            var translationOrigin = segmentPair.Properties.TranslationOrigin;

            var modelName = translationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
            var sourceCode = modelName.Substring(0, 3);
            var targetCode = modelName.Substring(3, 3);

            var feedback = new CloudFeedbackItem
            {
                Translation = new Translation
                {
                    SourceLanguageId = sourceCode,
                    TargetLanguageId = targetCode,
                    Model = translationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName),
                    SourceText = segmentPair.Source.ToString(),
                    TargetMTText = translationOrigin.GetMetaData(Constants.SegmentMetadata_Translation),
                    QualityEstimationMT = translationOrigin.GetMetaData(Constants.SegmentMetadata_QE)
                },
                Improvement = new Improvement(segmentPair.Target.ToString())
            };
            return feedback;
        }
    }
}