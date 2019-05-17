using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.View;
using Sdl.Community.GSVersionFetch.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.GSVersionFetch.Studio
{
	[RibbonGroup("GsVersion", Name = "GS Version Fetch", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class GsVersionFetchRibbon : AbstractRibbonGroup
	{
	}

	[Action("GsVersion",
		Name = "GS Version Fetch",
		Icon = "",
		Description = "GroupShare Version Fetch")]
	[ActionLayout(typeof(GsVersionFetchRibbon), 20, DisplayType.Large)]

	public class GsVersionFetchAction : AbstractAction
	{
		protected override void Execute()
		{
			//if (Application.Current == null)
			//{
			//	new Application();
			//}
			//if (Application.Current != null)
			//{
			//	Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			//}
			var wizardModel = new WizardModel
			{
				UserCredentials = new Credentials(),
				GsProjects = new ObservableCollection<ProjectDetails>()
			};
			var pages = CreatePages(wizardModel);

			var projectWizard = new ProjectWizard(pages);
			ElementHost.EnableModelessKeyboardInterop(projectWizard);
			projectWizard.Show();
			//Dispatcher.Run();
		}

		private ObservableCollection<ProjectWizardViewModelBase> CreatePages(WizardModel wizardModel)
		{

			return new ObservableCollection<ProjectWizardViewModelBase>
			{
				new LoginViewModel(wizardModel,new LoginView()),
				new ProjectsViewModel(wizardModel,new ProjectsView()),
				new FilesViewModel(new FilesView())
			};
		}
	}
}
