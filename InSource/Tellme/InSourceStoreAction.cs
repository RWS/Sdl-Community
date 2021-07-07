using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InSource.Tellme
{
	public class InSourceStoreAction : AbstractTellMeAction
	{
		public InSourceStoreAction()
		{
			Name = "Download Trados InSource! from AppStore";
		}
		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/trados-insource/548/");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados InSource! results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}
