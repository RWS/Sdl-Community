using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class HelpAction : DsiViewerAbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "DSI Viewer wiki in the RWS Community";
		}

		public override Icon Icon => PluginResources.Question;

		public override void Execute()
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5563/dsi-viewer");
		}
	}
}