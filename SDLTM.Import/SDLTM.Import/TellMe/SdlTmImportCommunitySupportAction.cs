using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	public class SdlTmImportCommunitySupportAction:AbstractTellMeAction
	{
		public SdlTmImportCommunitySupportAction()
		{
			Name = "SDLTM Import Plus AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLTM Import Plus results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}
