using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Trados.Transcreate.TellMe
{
	public class CommunityAppStoreForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format("{0} results", PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.ForumIcon;

		public CommunityAppStoreForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}
