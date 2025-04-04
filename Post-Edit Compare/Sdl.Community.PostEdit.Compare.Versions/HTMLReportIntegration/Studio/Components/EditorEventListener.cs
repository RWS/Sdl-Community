using Sdl.Community.PostEdit.Versions.Extension;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components
{
    //TODO Derive this from Core.EditorEventListener
    public class EditorEventListener
    {
        public event Action<List<CommentInfo>, string, string> CommentsChanged;

        public event Action<string, string, string> StatusChanged;

        public (List<IComment> Comments, ISegmentPair ActiveSegmentPair) CurrentComments { get; set; } = ([], null);
        private IStudioDocument ActiveDocument { get; set; }
        private EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();
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

        private void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            if (sender is not ISegmentContainerNode segment) return;

            var segmentStatus = segment.Segment.Properties.ConfirmationLevel.ToString();
            var activeFileId = AppInitializer.GetActiveFileId();

            StatusChanged?.Invoke(segmentStatus, segment.Segment.Properties.Id.Id, activeFileId);
        }

        private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e) => SetUpActiveDocument();

        private void PollCommentChanges()
        {
            var activeSegmentPair = ActiveDocument.GetActiveSegmentPair();
            if (activeSegmentPair is null) return;

            var comments = ActiveDocument.GetCommentsFromSegment(activeSegmentPair)?.ToList() ?? [];

            if (activeSegmentPair.Properties.Id != CurrentComments.ActiveSegmentPair?.Properties.Id)
            {
                CurrentComments = (comments, activeSegmentPair);
                return;
            }

            if (CurrentComments.Comments == null && comments == null ||
                CurrentComments.Comments != null &&
                CurrentComments.Comments.SequenceEqual(comments, new CommentComparer())) return;

            CurrentComments = (comments, activeSegmentPair);

            var commentInfo = comments.ToCommentInfoList();

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