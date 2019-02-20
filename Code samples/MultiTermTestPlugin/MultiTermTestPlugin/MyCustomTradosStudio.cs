using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace MultiTermTestPlugin
{
	[Action("MultiTermTestPlugin")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			Application oMt = new ApplicationClass();

			var oServerRep = oMt.ServerRepository;
			oServerRep.Location = "http://gs2017dev.sdl.com";
			oServerRep.Connect("SDLCommunity", "Commun1tyRocks");
			Console.WriteLine("Connection successful: " + oServerRep.IsConnected);

			var oTbs = oServerRep.Termbases;
		}
	}
}