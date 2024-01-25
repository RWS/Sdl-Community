using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class HelpAction : DsiViewerAbstractTellMeAction
	{
		public HelpAction()
		{
			Name = "DSI Viewer Documentation";
		}

		public override Icon Icon => default;

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/25?tab=documentation");
		}
	}
}