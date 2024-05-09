using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
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

        public override bool IsAvailable
        {
            get
            {
                var selectedProject = GetSelectedProject(out _);
                return selectedProject.GetProjectInfo().ProjectType != ProjectType.InLanguageCloud &&
                       (selectedProject == null || selectedProject.GetProjectInfo().ProjectOrigin !=
                           Constants.ProjectOrigin_TranscreateProject);
            }
        }

        private ProjectsController ProjectsController =>
            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public override void Execute()
        {
            GetSelectedProject(out var projectsTotal);

            switch (projectsTotal)
            {
                case > 1:
                    new SettingsActionWarning(PluginResources.SettingsAction_ConvertToTranscreate_MoreThanOneFileSelected,
                        1).ShowDialog();
                    return;

                case 0:
                    new SettingsActionWarning(PluginResources.SettingsAction_ConvertToTranscreate_NoFileSelected, 0).ShowDialog();
                    return;
            }

            SdlTradosStudio.Application.ExecuteAction<ConvertProjectAction>();
        }

        public FileBasedProject GetSelectedProject(out int projectsTotal)
        {
            var currentProject = ProjectsController.CurrentProject;

            projectsTotal = ProjectsController.SelectedProjects.Count();
            if (projectsTotal == 0 && currentProject != null) projectsTotal = 1;

            return projectsTotal is > 1 or 0 ? null : ProjectsController.SelectedProjects.FirstOrDefault() ?? currentProject;
        }
    }
}