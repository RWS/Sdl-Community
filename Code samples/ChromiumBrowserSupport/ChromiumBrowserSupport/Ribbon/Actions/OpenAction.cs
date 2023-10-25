using ChromiumBrowserSupport.View;
using ChromiumBrowserSupport.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace ChromiumBrowserSupport.Ribbon.Actions
{
	[Action("ChromiumBrowserSupport_OpenAction_Id",
		Name = "ChromiumBrowserSupport_OpenAction_Name",
		Description = "ChromiumBrowserSupport_OpenAction_Description",
		ContextByType = typeof(ProjectsController),
		Icon = "ChromiumBrowserSupport_OpenAction_Icon"
	)]
	[ActionLayout(typeof(RibbonGroups.ConfigurationGroup), 10, DisplayType.Large)]
	internal class OpenAction : AbstractAction
	{
		protected override void Execute()
		{
			var viewModel = new BrowserViewModel
			{
				Uri = "https://www.microsoft.com",
				RedirectUri = "https:///RedirectUriTest",
				NeedsClearingCookies = false
			};

			var view = new BrowserView {DataContext = viewModel};

			view.ShowDialog();
		}
	}
}
