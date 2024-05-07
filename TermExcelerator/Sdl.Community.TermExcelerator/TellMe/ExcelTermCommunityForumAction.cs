using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	public class ExcelTermCommunityForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.ForumIcon;

		public ExcelTermCommunityForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}