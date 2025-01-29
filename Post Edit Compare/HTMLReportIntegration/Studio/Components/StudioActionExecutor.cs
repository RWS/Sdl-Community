using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Linq;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components
{
    public class StudioActionExecutor
    {
        private EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();
        private ProjectsController _projectsController;

        private ProjectsController ProjectsController =>
            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public void ChangeStatusOfSegment(string status, string segmentId, string fileId, string projectId)
        {
            try
            {
                var segmentPair = GetSegmentPair(segmentId, fileId, projectId);
                ChangeStatusOfSegment(status, segmentPair);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private ISegmentPair GetSegmentPair(string segmentId, string fileId, string projectId)
        {
            EditorController.ActiveDocument.SetActiveSegmentPair(OpenFile(fileId, projectId), segmentId);

            var segmentPair = EditorController.ActiveDocument.GetActiveSegmentPair();

            if (segmentPair is null)
                throw new Exception(
                    "The segment pair was not found in the active document.");

            return segmentPair;
        }

        private void ChangeStatusOfSegment(string status, ISegmentPair segmentPair)
        {
            var segment = segmentPair.Target;
            var confirmationStatus = (ConfirmationLevel)Enum.Parse(typeof(ConfirmationLevel), status);
            var segmentPairProperties = segment.Properties;
            segmentPairProperties.ConfirmationLevel = confirmationStatus;
            EditorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPairProperties);
        }

        public void NavigateToSegment(string segmentId, string fileId, string projectId)
        {
            try
            {
                OpenFile(fileId, projectId);
                NavigateToSegment(segmentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private ProjectFile OpenFile(string fileId, string projectId)
        {
            var fileBasedProject = OpenProject(projectId);
            var projectFile = OpenFile(fileId, fileBasedProject);
            return projectFile;
        }

        private void InvokeAction(Action action) => Application.Current.Dispatcher.Invoke(action);

        private void NavigateToSegment(string segmentId)
        {
            InvokeAction(() =>
                EditorController.ActiveDocument.SetActiveSegmentPair(EditorController.ActiveDocument.Files.First(),
                    segmentId, true));
        }

        private ProjectFile OpenFile(string fileId, FileBasedProject project)
        {
            if (EditorController.ActiveDocument is not null &&
                AppInitializer.GetActiveFileId() == fileId)
                return EditorController.ActiveDocument.ActiveFile;

            var projectFile = project.GetTargetLanguageFiles().FirstOrDefault(file =>
            {
                var projFileInfo = FileIdentifier.GetFileInfo(file.LocalFilePath);
                return projFileInfo == fileId;
            });

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