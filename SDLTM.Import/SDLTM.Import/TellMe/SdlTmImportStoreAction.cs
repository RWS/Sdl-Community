using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	public class SdlTmImportStoreAction: AbstractTellMeAction
	{
		public SdlTmImportStoreAction()
		{
			Name = "Download SDLTM Import Plus from AppStore";
		}
		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/89");
		}
		public override bool IsAvailable => true;
		public override string Category => "SDLTM Import Plus results";
		public override Icon Icon => PluginResources.Download;
	}
}
