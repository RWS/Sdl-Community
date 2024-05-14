using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.TermInjector.TellMe.WarningWindow;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TermInjector.TellMe.Actions
{
    public class SettingsAction : AbstractTellMeAction
    {
        private SettingsActionWarning _settingsInfoDialog;

        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.Settings;
        public override bool IsAvailable => true;

        public SettingsActionWarning SettingsInfoDialog => _settingsInfoDialog ??=
            new SettingsActionWarning("https://appstore.rws.com/Plugin/75?tab=documentation");

        public override void Execute()
        {
            var selectedProject = GetSelectedProject();
            if (selectedProject == null)
            {
                ShowInfoDialog();
                return;
            }

            var tpConfig = selectedProject.GetTranslationProviderConfiguration();

            var termInjectorProvider =
                tpConfig.Entries.FirstOrDefault(e => e.MainTranslationProvider.Uri.OriginalString.Contains("terminjector"));
            if (termInjectorProvider == null)
            {
                ShowInfoDialog();
                return;
            }

            var termInjectorConfigDialog =
                new TermInjectorTranslationProviderConfDialog(new TermInjectorTranslationOptions(termInjectorProvider.MainTranslationProvider.Uri), null, null);

            if (termInjectorConfigDialog.ShowDialog() != DialogResult.OK) return;

            termInjectorProvider.MainTranslationProvider.Uri = termInjectorConfigDialog.Options.Uri;
            selectedProject.UpdateTranslationProviderConfiguration(tpConfig);
        }

        private static FileBasedProject GetSelectedProject()
        {
            var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            var selectedProjects = projectsController.SelectedProjects;

            FileBasedProject selectedProject = null;
            if (selectedProjects.Any())
                selectedProject = selectedProjects.LastOrDefault();

            return selectedProject ?? projectsController.CurrentProject;
        }

        private void ShowInfoDialog() => SettingsInfoDialog.ShowDialog();
    }
}