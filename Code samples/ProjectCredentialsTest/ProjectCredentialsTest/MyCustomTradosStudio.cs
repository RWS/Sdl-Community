using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace ProjectCredentialsTest
{
	[Action("ProjectCredentialsTest")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var termbaseUri = new Uri("sdltm.http://gs2017dev.sdl.com");


			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			projectController.CurrentProject.Credentials.AddCredential(termbaseUri, true, "", "");
		}
	}
}