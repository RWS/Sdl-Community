using System.Diagnostics;
using System.Drawing;

namespace SdlXliffToolkit.TellMe.Actions
{
	public class AppStoreForumAction : ToolkitAbastractTellMeAction
	{
		public AppStoreForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override Icon Icon => PluginResources.ForumIcon;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}