using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.TellMe
{
	public class CommunityForumAction : AbstractTellMeAction
	{
		public CommunityForumAction()
		{
			Name = "SDL Community AppStore forum";
		}

		public override string Category => "Community Advanced Display Filter results";

		public override Icon Icon => PluginResources.ForumIcon;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/f/160");
		}
	}
}