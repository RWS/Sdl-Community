using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MtComparisonTool.Models;
using MtComparisonTool.Service;
using Newtonsoft.Json;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace MtComparisonTool.Studio
{
	[Action("MT Comparison",
		Name = "MT Comparison Tool",
		Description = "MT Comparison Tool")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",true)]
	public class MtComparisonAction : AbstractAction
	{
		protected override async void Execute()
		{
			var authenticationService = new AuthenticationService();
			var loginResponse = await authenticationService.Login("", "");
			if (loginResponse.IsSuccessful)
			{
				var response = JsonConvert.DeserializeObject<UserResponse>(loginResponse.Content);

				//var session authenticationService.Session(response.Sid,"253263")

			}
		}
	}
}
