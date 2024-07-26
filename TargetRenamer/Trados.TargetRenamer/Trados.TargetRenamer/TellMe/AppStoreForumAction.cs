using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Trados.TargetRenamer.TellMe
{
	public class AppStoreForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.TargetRenamer_Name);

		public override Icon Icon => PluginResources.ForumIcon;

		public AppStoreForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}
