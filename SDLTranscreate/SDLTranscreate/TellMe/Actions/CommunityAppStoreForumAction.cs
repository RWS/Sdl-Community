using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Trados.Transcreate.TellMe.Actions
{
	public class CommunityAppStoreForumAction : AbstractTellMeAction
	{
		public CommunityAppStoreForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.ForumIcon;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}