using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components
{
    public class EditorEventListener
    {
        public event Action<List<CommentInfo>, string> CommentsChanged;

        public (List<IComment> Comments, ISegmentPair ActiveSegmentPair) CurrentComments { get; set; }

        private IStudioDocument ActiveDocument { get; set; }

        private Timer PollingTimer { get; } = new(500);

        public void StartListening()
        {
            AppInitializer.EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
            SetUpActiveDocument();
        }

        public void StopListening()
        {
            AppInitializer.EditorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
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
                Date = c.Date.ToString("yyyy-MMM-dd"),
                Text = c.Text,
                Severity = c.Severity
            }).ToList();

            CommentsChanged?.Invoke(commentInfo, activeSegmentPair.Properties.Id.ToString());
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
            if (AppInitializer.EditorController.ActiveDocument is null) return;
            ActiveDocument = AppInitializer.EditorController.ActiveDocument;

            ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
            ActiveDocument_ActiveSegmentChanged(null, null);

            PollingTimer.Elapsed += PollingTimer_Elapsed;
        }

        private void StopListeningPreviousDocument()
        {
            if (ActiveDocument is null) return;
            PollingTimer.Elapsed -= PollingTimer_Elapsed;
            PollingTimer.Stop();
        }
    }
}