using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportToExcel.TellMe
{
	public class CommunityForumAction : AbstractTellMeAction
	{
		public CommunityForumAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override void Execute()
		{
			Process.Start(
				"https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}

		public override bool IsAvailable => true;

		public override string Category => "Export to excel results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}
