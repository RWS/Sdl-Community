using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	public class SdlTmImportDocumentationAction:AbstractTellMeAction
	{
		public SdlTmImportDocumentationAction()
		{
			Name = "SDLTM Import Plus Documentation";
		}
		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/89?tab=documentation");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLTM Import Plus results";
		public override Icon Icon => PluginResources.TellMe_Documentation;
	}
}
