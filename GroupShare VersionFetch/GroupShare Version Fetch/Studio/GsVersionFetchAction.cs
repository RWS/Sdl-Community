using NLog;
using Sdl.Community.GSVersionFetch.Interface;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Community.GSVersionFetch.View;
using Sdl.Community.GSVersionFetch.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System.Collections.ObjectModel;
using System.Windows.Forms.Integration;

namespace Sdl.Community.GSVersionFetch.Studio
{
    [Action("GsVersion",
        Name = "GS Version Fetch",
        Icon = "gs_fetch_versions_Hep_icon",
        Description = "GroupShare Version Fetch")]
    [ActionLayout(typeof(GsVersionFetchRibbon), 20, DisplayType.Large)]
    public class GsVersionFetchAction : AbstractAction
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override void Execute()
        {
            Logger.Info("GroupShare Version Fetch started...");
            var wizardModel = new WizardModel
            {
                UserCredentials = new Credentials(),
                GsProjects = new ObservableCollection<GsProject>(),
                ProjectsForCurrentPage = new ObservableCollection<GsProject>(),
                GsFiles = new ObservableCollection<GsFile>(),
                FileVersions = new ObservableCollection<GsFileVersion>(),
                Organizations = new ObservableCollection<OrganizationResponse>(),
                SelectedOrganization = new OrganizationResponse(),
                Version = string.Empty
            };
            var pages = CreatePages(wizardModel);

            using var projectWizard = new ProjectWizard(pages);
            projectWizard.ShowDialog();
        }

        private ObservableCollection<IProgressHeaderItem> CreatePages(WizardModel wizardModel)
        {
            return new()
            {
                new ProjectsViewModel(wizardModel, new MessageBoxService()),
                new FilesViewModel(wizardModel),
                new FilesVersionsViewModel(wizardModel)
            };
        }
    }

    [RibbonGroup("GsVersion", Name = "", ContextByType = typeof(ProjectsController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class GsVersionFetchRibbon : AbstractRibbonGroup
    {
    }
}