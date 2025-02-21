using Sdl.ProjectAutomation.Core;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using System.Linq;
using Trados.Transcreate.Actions;
using Trados.Transcreate.Common;
using Trados.Transcreate.TellMe.WarningWindow;

namespace Trados.Transcreate.TellMe.Actions
{
    public class ConvertToTranscreateProjectAction : AbstractTellMeAction
    {
        private ProjectsController _projectsController;

        public ConvertToTranscreateProjectAction()
        {
            Name = $"{PluginResources.Plugin_Name} Convert Project";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Icon;

        public override bool IsAvailable => true;

        private ProjectsController ProjectsController =>
            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public override void Execute()
        {
            var projectsTotal = GetSelectedProjectsTotal();

            if (projectsTotal > 1)
            {
                new SettingsActionWarning(PluginResources.SettingsAction_ConvertToTranscreate_MoreThanOneFileSelected)
                    .ShowDialog();
                return;
            }

            if (projectsTotal == 0)
            {
                new SettingsActionWarning(PluginResources.SettingsAction_ConvertToTranscreate_NoFileSelected)
                    .ShowDialog();
                return;
            }

            var selectedProject = ApplicationInstance.GetSelectedProject();
            var selectedProjectInfo = selectedProject.GetProjectInfo();

            if (selectedProjectInfo.ProjectOrigin == Constants.ProjectOrigin_TranscreateProject)
            {
                new SettingsActionWarning(PluginResources.SettingsAction_ProjectAlreadyTranscreate)
                    .ShowDialog();
                return;
            }

            if (selectedProjectInfo.ProjectType == ProjectType.InLanguageCloud)
            {
                new SettingsActionWarning(PluginResources.SettingsAction_ProjectIsInLC)
                    .ShowDialog();
                return;
            }

            SdlTradosStudio.Application.ExecuteAction<ConvertProjectAction>();
        }

        public int GetSelectedProjectsTotal()
        {
            var currentProject = ProjectsController.CurrentProject;

            var projectsTotal = ProjectsController.SelectedProjects.Count();
            if (projectsTotal == 0 && currentProject != null) projectsTotal = 1;

            return projectsTotal;
        }
    }
}