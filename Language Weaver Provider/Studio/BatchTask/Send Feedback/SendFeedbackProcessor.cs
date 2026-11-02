using LanguageWeaverProvider.Send_feedback;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace LanguageWeaverProvider.Studio.BatchTask.Send_Feedback
{
    public class SendFeedbackProcessor : AbstractBilingualContentProcessor
    {
        public const string MissingMetadataError = "Feedback was not sent: no Language Weaver translation data (model and original machine translation) is stored in this segment. Translate the segment again with Language Weaver, then send feedback.";

        public List<SegmentError> Errors { get; set; } = new();

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            base.ProcessParagraphUnit(paragraphUnit);
            if (paragraphUnit.IsStructure) return;
            foreach (var segmentPair in paragraphUnit.SegmentPairs)
            {
                if (!IsApproved(segmentPair.Properties.ConfirmationLevel)) continue;

                var feedback = LanguageWeaverFeedbackFactory.Create(segmentPair);
                if (feedback is null) continue;

                var translationOrigin = segmentPair.Properties.TranslationOrigin;
                if (string.IsNullOrEmpty(translationOrigin.GetMetaData(Constants.SegmentMetadata_LongModelName))
                    || string.IsNullOrEmpty(translationOrigin.GetMetaData(Constants.SegmentMetadata_Translation)))
                {
                    AddError(segmentPair, feedback, MissingMetadataError);
                    continue;
                }

                try
                {
                    //Window batchTaskWindow = null;
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    // Update UI element here
                    //    batchTaskWindow = ApplicationInitializer.GetBatchTaskWindow();
                    //});
                    //batchTaskWindow.Dispatcher.Invoke(() =>
                    //{
                        feedback.Send().Wait();
                    //});
                }
                catch (Exception ex)
                {
                    AddError(segmentPair, feedback, ex.InnerException?.Message);
                }
            }
        }

        private void AddError(ISegmentPair segmentPair, LanguageWeaverFeedback feedback, string error) =>
            Errors.Add(new SegmentError
            {
                Provider = feedback is CloudFeedback ? Constants.CloudService : Constants.EdgeService,
                Error = error,
                SourceSegment = segmentPair.Source.ToString(),
                Id = segmentPair.Properties.Id.Id
            });

        private static bool IsApproved(ConfirmationLevel confirmationLevel) =>
            confirmationLevel is
                ConfirmationLevel.ApprovedTranslation or
                ConfirmationLevel.Translated or
                ConfirmationLevel.ApprovedSignOff;
    }
}