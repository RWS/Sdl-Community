using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Versions.Extension;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components
{
    public class StudioActionExecutor
    {
        private ProjectsController _projectsController;
        private EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();
        private Dictionary<string, ConfirmationLevel> OriginalStatuses { get; set; } = new();

        private ProjectsController ProjectsController =>
                    _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public void AddComment(string comment, string severityString, string segmentId, string fileId, string projectId)
        {
            try
            {
                var fileBasedProject = OpenProject(projectId);
                var projectFile = OpenFile(fileId, fileBasedProject);
                var segmentPair = GetSegmentPair(segmentId, projectFile);

                if (Enum.TryParse<Severity>(severityString, out var severity))
                    EditorController.ActiveDocument.AddCommentOnSegment(segmentPair, comment, severity);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void AddCommentsToEditorSegment(IEnumerable<CommentInfo> comments, string segmentId,
                    string fileId, string projectId)
        {
            foreach (var comment in comments)
                AddComment(comment.Text, comment.Severity,
                    segmentId, fileId, projectId);
        }

        public void ChangeStatusOfEditorSegments(List<StatusInfo> statuses)
        {
            foreach (var status in statuses)
                ChangeStatusOfSegment(status.Status, status.SegmentId,
                    status.FileId, status.ProjectId);
        }

        public void ChangeStatusOfSegment(string status, string segmentId, string fileId, string projectId)
        {
            try
            {
                var fileBasedProject = OpenProject(projectId);
                var projectFile = OpenFile(fileId, fileBasedProject);
                var segmentPair = GetSegmentPair(segmentId, projectFile);
                ChangeStatusOfSegment(status, segmentPair);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public List<CommentInfo> GetEditorComments(string segmentId, string fileId, string projectId)
        {
            IEnumerable<IComment> comments = [];
            try
            {
                var fileBasedProject = OpenProject(projectId);
                var projectFile = OpenFile(fileId, fileBasedProject);
                EditorController.ActiveDocument.SetActiveSegmentPair(projectFile, segmentId);

                var segmentPair = EditorController.ActiveDocument.GetActiveSegmentPair();
                segmentPair ??=
                    EditorController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id.Id == segmentId);

                comments = EditorController.ActiveDocument.GetCommentsFromSegment(segmentPair) ?? [];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return comments.ToCommentInfoList();
        }

        public void NavigateToSegment(string segmentId, string fileId, string projectId)
        {
            try
            {
                var fileBasedProject = OpenProject(projectId);
                OpenFile(fileId, fileBasedProject, true);
                NavigateToSegment(segmentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void SaveProject() => ProjectsController.CurrentProject.Save();

        private void ChangeStatusOfSegment(string statusString, ISegmentPair segmentPair)
        {
            if (!EnumHelper.TryGetConfirmationLevel(statusString, out var confirmationStatus))
                confirmationStatus = OriginalStatuses[segmentPair.Properties.Id.Id];

            if (!OriginalStatuses.ContainsKey(segmentPair.Properties.Id.Id))
                OriginalStatuses[segmentPair.Properties.Id.Id] = confirmationStatus;

            var segmentPairProperties = segmentPair.Target.Properties;
            segmentPairProperties.ConfirmationLevel = confirmationStatus;
            EditorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPairProperties);
        }

        private ISegmentPair GetSegmentPair(string segmentId, ProjectFile projectFile)
        {
            ISegmentPair segmentPair = null;
            try
            {
                EditorController.ActiveDocument.SetActiveSegmentPair(projectFile, segmentId);
                segmentPair = EditorController.ActiveDocument.GetActiveSegmentPair();
            }
            catch { }

            segmentPair ??= EditorController.ActiveDocument.SegmentPairs.FirstOrDefault(sp => sp.Properties.Id.Id == segmentId);

            if (segmentPair is null)
                throw new Exception(
                    "The segment pair was not found in the active document.");

            return segmentPair;
        }

        private void InvokeAction(Action action) => Application.Current.Dispatcher.Invoke(action);

        private void NavigateToSegment(string segmentId)
        {
            InvokeAction(() =>
                EditorController.ActiveDocument.SetActiveSegmentPair(EditorController.ActiveDocument.Files.First(),
                    segmentId, true));
        }

        private ProjectFile OpenFile(string fileId, FileBasedProject project, bool forceOpen = false)
        {
            if (!forceOpen && EditorController.ActiveDocument is not null &&
                AppInitializer.GetActiveFileId() == fileId)
                return EditorController.ActiveDocument.ActiveFile;

            var projectFile = project.GetTargetLanguageFiles()
                .FirstOrDefault(file => FileIdentifier.GetFileInfo(file.LocalFilePath) == fileId);

            if (projectFile is null)
                throw new Exception(
                    "The project file was not found in the project.");

            InvokeAction(() => EditorController.Open(projectFile, EditingMode.Translation));
            return projectFile;
        }

        private FileBasedProject OpenProject(string projectId)
        {
            if (ProjectsController is null)
                throw new Exception(
                    "ProjectsController is null. Please go at least once to Projects View and then retry the action.");

            var currentProject = ProjectsController.CurrentProject;
            if (currentProject.GetProjectInfo().Id.ToString() == projectId) return currentProject;

            var projects = ProjectsController.GetAllProjects();
            foreach (var fileBasedProject in projects)
            {
                if (fileBasedProject.GetProjectInfo().Id.ToString() != projectId) continue;
                InvokeAction(() => ProjectsController.Open(fileBasedProject));
                return fileBasedProject;
            }

            throw new Exception(
                "The project was not found in the Studio project list.");
        }
    }
}