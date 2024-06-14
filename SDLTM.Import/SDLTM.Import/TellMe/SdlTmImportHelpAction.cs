using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	public class SdlTmImportHelpAction:AbstractTellMeAction
	{
		public SdlTmImportHelpAction()
		{
			Name = "SDLTM Import Plus in the RWS Community";
		}
		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5251/sdltm-import-plus");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLTM Import Plus results";
		public override Icon Icon => PluginResources.Question;
	}
}
