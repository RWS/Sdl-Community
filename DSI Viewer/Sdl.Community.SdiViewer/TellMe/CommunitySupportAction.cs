using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.DsiViewer.TellMe
{
	public class CommunitySupportAction : DsiViewerAbstractTellMeAction
	{
		public CommunitySupportAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override Icon Icon => PluginResources.ForumIcon;

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}
	}
}