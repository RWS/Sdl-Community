using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.DesktopEditor.EditorApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components
{
    public class EditorEventListener
    {
        public event Action<List<CommentInfo>, string, string> CommentsChanged;
        public event Action<string, string, string> StatusChanged;
        private EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();

        public (List<IComment> Comments, ISegmentPair ActiveSegmentPair) CurrentComments { get; set; }

        private IStudioDocument ActiveDocument { get; set; }

        private Timer PollingTimer { get; } = new(500);

        public void StartListening()
        {
            EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
            SetUpActiveDocument();
        }

        public void StopListening()
        {
            EditorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
            StopListeningPreviousDocument();
        }

        private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
        {
            PollingTimer.Stop();

            var activeSegmentPair = ActiveDocument.GetActiveSegmentPair();
            CurrentComments = (ActiveDocument.GetCommentsFromSegment(activeSegmentPair)?.ToList(), activeSegmentPair);

            PollingTimer.Start();
        }

        private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e) => SetUpActiveDocument();

        private void PollCommentChanges()
        {
            var activeSegmentPair = ActiveDocument.GetActiveSegmentPair();
            if (activeSegmentPair is null) return;

            var comments = ActiveDocument.GetCommentsFromSegment(activeSegmentPair)?.ToList();

            if (CurrentComments.Comments == null && comments == null || CurrentComments.Comments != null &&
                 CurrentComments.Comments.SequenceEqual(comments, new CommentComparer())) return;

            CurrentComments = (comments, activeSegmentPair);

            var commentInfo = comments?.Select(c => new CommentInfo
            {
                Author = c.Author,
                Date = c.Date.ToString(MessagingConstants.DateFormat),
                Text = c.Text,
                Severity = c.Severity.ToString()
            }).ToList();

            CommentsChanged?.Invoke(commentInfo, activeSegmentPair.Properties.Id.ToString(),
                AppInitializer.GetActiveFileId());
        }

        

        private void PollingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PollCommentChanges();
        }

        private void SetUpActiveDocument()
        {
            StopListeningPreviousDocument();
            StartListeningCurrentDocument();
        }

        private void StartListeningCurrentDocument()
        {
            if (EditorController.ActiveDocument is null) return;
            ActiveDocument = EditorController.ActiveDocument;

            ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
            ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
            ActiveDocument_ActiveSegmentChanged(null, null);

            PollingTimer.Elapsed += PollingTimer_Elapsed;
        }

        private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            if (sender is not ISegmentContainerNode segment) return;

            var segmentStatusFriendly =
                ReportUtils.GetVisualSegmentStatus(segment.Segment.Properties.ConfirmationLevel.ToString());
            var activeFileId = AppInitializer.GetActiveFileId();

            StatusChanged?.Invoke(segmentStatusFriendly, segment.Segment.Properties.Id.Id, activeFileId);
        }

        private void StopListeningPreviousDocument()
        {
            if (ActiveDocument is null) return;

            ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
            ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;

            PollingTimer.Elapsed -= PollingTimer_Elapsed;
            PollingTimer.Stop();
        }
    }
}