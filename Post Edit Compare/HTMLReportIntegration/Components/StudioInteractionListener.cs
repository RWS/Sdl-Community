using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Community.PostEdit.Versions.ReportViewer.Model;
using Sdl.Community.PostEdit.Versions.ReportViewer.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Components
{
    public class StudioInteractionListener
    {
        public event Action<List<CommentInfo>, string> CommentsChanged;

        public (List<IComment> Comments, ISegmentPair ActiveSegmentPair) CurrentComments { get; set; }
        public EditorController EditorController => AppInitializer.EditorController;

        private IStudioDocument ActiveDocument { get; set; }

        private Timer PollingTimer { get; set; } = new(500);

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

            if (CurrentComments.Comments == null && comments == null || (CurrentComments.Comments != null &&
                 CurrentComments.Comments.SequenceEqual(comments, new CommentComparer()))) return;

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
            if (EditorController.ActiveDocument is null) return;
            ActiveDocument = EditorController.ActiveDocument;

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