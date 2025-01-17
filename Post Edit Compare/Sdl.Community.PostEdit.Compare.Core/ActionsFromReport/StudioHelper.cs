using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Linq;
using System.Windows;

namespace Sdl.Community.PostEdit.Compare.Core.ActionsFromReport
{
    public class StudioHelper
    {
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

        private static void InvokeAction(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        private static void NavigateToSegment(string segmentId)
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            InvokeAction(() =>
                editorController.ActiveDocument.SetActiveSegmentPair(editorController.ActiveDocument.Files.First(),
                    segmentId, true));
        }

        private static void OpenFile(string fileInfo, FileBasedProject project)
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            if (editorController is null)
                throw new Exception(
                    "EditorController is null. Please go at least once to Editor View and then retry the action.");

            if (editorController.ActiveDocument is not null &&
                editorController.ActiveDocument.ActiveFile.LocalFilePath == fileInfo) return;

            var projectFile = project.GetTargetLanguageFiles().FirstOrDefault(file =>
            {
                var projFileInfo = FileIdentifier.GetFileInfo(file.LocalFilePath);
                return projFileInfo == fileInfo;
            });

            if (projectFile is null)
                throw new Exception(
                    "The project file was not found in the project.");

            InvokeAction(() => editorController.Open(projectFile, EditingMode.Translation));
        }

        private static FileBasedProject OpenProject(string projectId)
        {
            var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            if (projectsController is null)
                throw new Exception(
                    "ProjectsController is null. Please go at least once to Projects View and then retry the action.");

            var currentProject = projectsController.CurrentProject;
            if (currentProject.GetProjectInfo().Id.ToString() == projectId) return currentProject;

            var projects = projectsController.GetAllProjects();
            foreach (var fileBasedProject in projects)
            {
                if (fileBasedProject.GetProjectInfo().Id.ToString() != projectId) continue;
                InvokeAction(() => projectsController.Open(fileBasedProject));
                return fileBasedProject;
            }

            throw new Exception(
                "The project was not found in the Studio project list.");
        }
    }
}