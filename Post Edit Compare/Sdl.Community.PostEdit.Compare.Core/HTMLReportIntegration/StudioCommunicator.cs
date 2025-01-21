using Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration.Components;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Linq;
using System.Windows;

namespace Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration
{
    public class StudioCommunicator
    {
        private EditorController _editorController;
        private ProjectsController _projectsController;

        private EditorController EditorController =>
            _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();

        private ProjectsController ProjectsController =>
            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public void ChangeStatusOfSegment(string status, string segmentId, string fileId, string projectId)
        {
            try
            {
                NavigateToSegment(segmentId, fileId, projectId);

                var segmentPair = EditorController.ActiveDocument.GetActiveSegmentPair();
                if (segmentPair is null)
                    throw new Exception(
                        "The segment pair was not found in the active document.");

                var segment = segmentPair.Target;
                var confirmationStatus = (ConfirmationLevel)Enum.Parse(typeof(ConfirmationLevel), status);
                var segmentPairProperties = segment.Properties;
                segmentPairProperties.ConfirmationLevel = confirmationStatus;
                EditorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPairProperties);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void NavigateToSegment(string segmentId, string fileId, string projectId)
        {
            try
            {
                var fileBasedProject = OpenProject(projectId);
                OpenFile(fileId, fileBasedProject);
                NavigateToSegment(segmentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void InvokeAction(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        private void NavigateToSegment(string segmentId)
        {
            InvokeAction(() =>
                EditorController.ActiveDocument.SetActiveSegmentPair(EditorController.ActiveDocument.Files.First(),
                    segmentId, true));
        }

        private void OpenFile(string fileInfo, FileBasedProject project)
        {
            if (EditorController.ActiveDocument is not null &&
                EditorController.ActiveDocument.ActiveFile.LocalFilePath == fileInfo) return;

            var projectFile = project.GetTargetLanguageFiles().FirstOrDefault(file =>
            {
                var projFileInfo = FileIdentifier.GetFileInfo(file.LocalFilePath);
                return projFileInfo == fileInfo;
            });

            if (projectFile is null)
                throw new Exception(
                    "The project file was not found in the project.");

            InvokeAction(() => EditorController.Open(projectFile, EditingMode.Translation));
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