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
        public List<SegmentError> Errors { get; set; } = new();

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            base.ProcessParagraphUnit(paragraphUnit);
            if (paragraphUnit.IsStructure) return;
            foreach (var segmentPair in paragraphUnit.SegmentPairs)
            {
                if (!IsApproved(segmentPair.Properties.ConfirmationLevel)) continue;

                var feedback = LanguageWeaverFeedbackFactory.Create(segmentPair);

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
                        feedback?.Send().Wait();
                    //});
                }
                catch (Exception ex)
                {
                    Errors.Add(new SegmentError
                    {
                        Provider = feedback is CloudFeedback ? Constants.CloudService : Constants.EdgeService,
                        Error = ex.InnerException?.Message,
                        SourceSegment = segmentPair.Source.ToString(),
                        Id = segmentPair.Properties.Id.Id
                    });
                }
            }
        }

        private static bool IsApproved(ConfirmationLevel confirmationLevel) =>
            confirmationLevel is
                ConfirmationLevel.ApprovedTranslation or
                ConfirmationLevel.Translated or
                ConfirmationLevel.ApprovedSignOff;
    }
}