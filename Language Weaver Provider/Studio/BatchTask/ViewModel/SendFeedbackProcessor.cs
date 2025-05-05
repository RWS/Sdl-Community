using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Studio.BatchTask.ViewModel
{
    public class SendFeedbackProcessor : AbstractBilingualContentProcessor
    {
        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            base.ProcessParagraphUnit(paragraphUnit);
            if (paragraphUnit.IsStructure) return;
            foreach (var segmentPair in paragraphUnit.SegmentPairs)
            {
                var translationOrigin = segmentPair.Properties.TranslationOrigin;
                var originSystem = translationOrigin?.OriginSystem;

                if (originSystem is null ||
                    !IsLanguageWeaverOrigin(originSystem) ||
                    !IsApproved(segmentPair.Properties.ConfirmationLevel)) continue;

                switch (originSystem)
                {
                    case Constants.PluginNameCloud:
                        SendLwCloudFeedback(segmentPair);
                        break;

                    case Constants.PluginNameEdge:
                        SendLwEdgeFeedback(segmentPair);
                        break;
                }
            }
        }

        private static AccessToken GetAccessToken(PluginVersion pluginVersion)
        {
            var translationOptions = new TranslationOptions
            {
                PluginVersion = pluginVersion
            };
            CredentialManager.GetCredentials(translationOptions, true);
            var accessToken = translationOptions.AccessToken;
            return accessToken;
        }

        private static bool IsApproved(ConfirmationLevel confirmationLevel) =>
            confirmationLevel is
                ConfirmationLevel.ApprovedTranslation or
                ConfirmationLevel.Translated or
                ConfirmationLevel.ApprovedSignOff;

        private static bool IsLanguageWeaverOrigin(string originSystem) =>
            originSystem.Contains(Constants.PluginShortName);

        private void SendLwCloudFeedback(ISegmentPair segmentPair)
        {
            var translationOrigin = segmentPair.Properties.TranslationOrigin;
            var modelName = translationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName);
            var sourceCode = modelName.Substring(0, 3);
            var targetCode = modelName.Substring(3, 3);

            var feedback = new FeedbackRequest
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

            var accessToken = GetAccessToken(PluginVersion.LanguageWeaverCloud);
            CloudService.CreateFeedback(accessToken, feedback, false).Wait();
        }

        private void SendLwEdgeFeedback(ISegmentPair segmentPair)
        {
            var sourceText = segmentPair.Source.ToString();
            var languagePairId = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_ShortModelName);
            var originalTranslation = segmentPair.Properties.TranslationOrigin.GetMetaData(Constants.SegmentMetadata_Translation);

            var targetText = segmentPair.Target.ToString();
            var suggestedTranslation = !originalTranslation.Equals(targetText) ? targetText : null;

            var feedbackItem = new EdgeFeedbackItem
            {
                SourceText = sourceText,
                LanguagePairId = languagePairId,
                MachineTranslation = originalTranslation,
                Comment = null,
                SuggestedTranslation = suggestedTranslation
            };

            var accessToken = GetAccessToken(PluginVersion.LanguageWeaverEdge);
            EdgeService.SendFeedback(accessToken, feedbackItem, false).Wait();
        }
    }
}