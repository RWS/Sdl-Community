using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class StoreAction : DsiViewerAbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download DSI Viewer from AppStore";
		}

		public override Icon Icon => PluginResources.TellMe;

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/dsi-viewer/995/");
		}
	}
}