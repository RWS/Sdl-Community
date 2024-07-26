using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class SourceCodeAction : DsiViewerAbstractTellMeAction
	{
		public SourceCodeAction()
		{
			Name = "DSI Viewer Source Code";
		}

		public override Icon Icon => PluginResources.ForumIcon;

		public override void Execute()
		{
			Process.Start(
				"https://github.com/RWS/Sdl-Community/tree/master/DSI%20Viewer");
		}
	}
}