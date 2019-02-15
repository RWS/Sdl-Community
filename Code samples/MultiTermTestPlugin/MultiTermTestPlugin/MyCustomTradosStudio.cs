using System;
using MultiTermIX;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace MultiTermTestPlugin
{
	[Action("MultiTermTest")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			Application oMt = new ApplicationClass();

			var oServerRep = oMt.ServerRepository;
			oServerRep.Location = "";
			oServerRep.Connect("", "");
			Console.WriteLine("Connection successful: " + oServerRep.IsConnected);

			var oTbs = oServerRep.Termbases;
		}
	}
}
