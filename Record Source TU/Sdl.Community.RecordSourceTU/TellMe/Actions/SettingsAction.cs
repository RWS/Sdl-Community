using System.Collections.Generic;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using System.Linq;
using Sdl.Community.RecordSourceTU.TellMe.WarningWindow;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.RecordSourceTU.TellMe.Actions
{
    public class SettingsAction : AbstractTellMeAction
    {
        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Settings2;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            var selectedProject = GetSelectedProject();
            if (selectedProject is null)
            {
                ShowInfo(PluginResources.SettingsAction_NoProjectSelected);
                return;
            }

            var tpConfig = selectedProject.GetTranslationProviderConfiguration().Entries
                .FirstOrDefault(e => e.MainTranslationProvider.Uri.ToString().Contains("addsourcetm"));
            if (tpConfig is null)
            {
                ShowInfo(PluginResources.SettingsAction_RecordSourceNotConfigured);
                return;
            }

            using var sourceConfigurationForm = new SourceTmConfiguration(tpConfig.MainTranslationProvider.Uri);
            sourceConfigurationForm.ShowDialog();
        }

        public void ShowInfo(string text) => new SettingsActionWarning(text).ShowDialog();

        private FileBasedProject GetSelectedProject()
        {
            var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            return projectsController.SelectedProjects.FirstOrDefault() ??
                   projectsController.CurrentProject;
        }
    }
}