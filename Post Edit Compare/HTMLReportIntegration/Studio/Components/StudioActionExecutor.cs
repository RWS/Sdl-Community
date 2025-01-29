using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
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

        public void NavigateToSegment(string segmentId, string fileId, string projectId)
        {
            try
            {
                OpenFile(fileId, projectId, true);
                NavigateToSegment(segmentId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ChangeStatusOfSegment(string statusString, ISegmentPair segmentPair)
        {
            if (!Enum.TryParse<ConfirmationLevel>(statusString, out var confirmationStatus))
                confirmationStatus = OriginalStatuses[segmentPair.Properties.Id.Id];

            if (!OriginalStatuses.ContainsKey(segmentPair.Properties.Id.Id))
                OriginalStatuses[segmentPair.Properties.Id.Id] = confirmationStatus;

            var segmentPairProperties = segmentPair.Target.Properties;
            segmentPairProperties.ConfirmationLevel = confirmationStatus;
            EditorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPairProperties);
        }

        private ISegmentPair GetSegmentPair(string segmentId, string fileId, string projectId)
        {
            ISegmentPair segmentPair = null;
            try
            {
                EditorController.ActiveDocument.SetActiveSegmentPair(OpenFile(fileId, projectId), segmentId);
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

        private ProjectFile OpenFile(string fileId, string projectId, bool forceOpen = false)
        {
            var fileBasedProject = OpenProject(projectId);
            var projectFile = OpenFile(fileId, fileBasedProject, forceOpen);
            return projectFile;
        }

        private ProjectFile OpenFile(string fileId, FileBasedProject project, bool forceOpen = false)
        {
            if (!forceOpen && EditorController.ActiveDocument is not null &&
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