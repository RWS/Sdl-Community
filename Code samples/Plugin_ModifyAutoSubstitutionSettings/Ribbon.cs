using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Linq;

namespace Plugin_ModifyAutoSubstitutionSettings
{
    [Action(nameof(TestRibbonAction), Icon = "Nuclear", Name = "Do/test something",
        Description = "Describing...")]
    [ActionLayout(typeof(TestRibbonGroup), 10, DisplayType.Large)]
    public class TestRibbonAction : AbstractAction
    {
        protected override void Execute()
        {
            var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

            var project = projectsController.CurrentProject;

            var lang = project.GetProjectInfo().TargetLanguages.First();

            var settings = project.GetSettings(lang);

            var translationMemorySettings = settings.GetSettingsGroup<TranslationMemorySettings>();

            translationMemorySettings.SetSetting(Constants.AlphaNumericAutoLocalizationEnabled, false);

            project.UpdateSettings(lang, settings);
            project.Save();
        }
    }

    [RibbonGroup(nameof(TestRibbonGroup), Name = "Test")]
    [RibbonGroupLayout(LocationByType =
        typeof(Sdl.Desktop.IntegrationApi.DefaultLocations.StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class TestRibbonGroup : AbstractRibbonGroup
    {
    }
}