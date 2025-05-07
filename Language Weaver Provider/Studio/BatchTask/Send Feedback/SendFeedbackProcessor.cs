using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Send_feedback;
using LanguageWeaverProvider.Services;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;

namespace LanguageWeaverProvider.Studio.BatchTask.Send_Feedback
{
    public class SendFeedbackProcessor : AbstractBilingualContentProcessor
    {
        public List<SegmentError> Errors { get; set; } = new();

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
            Service.ValidateToken(translationOptions);
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
            var accessToken = GetAccessToken(PluginVersion.LanguageWeaverCloud);

            try
            {
                var feedback = new CloudFeedback(segmentPair);
                feedback.Send(accessToken).Wait();
            }
            catch (Exception ex)
            {
                Errors.Add(new SegmentError
                {
                    Provider = Constants.CloudService,
                    Error = ex.InnerException?.Message,
                    SourceSegment = segmentPair.Source.ToString(),
                    Id = segmentPair.Properties.Id.Id
                });
            }
        }

        private void SendLwEdgeFeedback(ISegmentPair segmentPair)
        {
            var accessToken = GetAccessToken(PluginVersion.LanguageWeaverEdge);

            try
            {
                var feedback = new EdgeFeedback(segmentPair);
                feedback.Send(accessToken).Wait();
            }
            catch (Exception ex)
            {
                Errors.Add(new SegmentError
                {
                    Provider = Constants.EdgeService,
                    Error = ex.InnerException?.Message,
                    SourceSegment = segmentPair.Source.ToString(),
                    Id = segmentPair.Properties.Id.Id
                });
            }
        }
    }
}