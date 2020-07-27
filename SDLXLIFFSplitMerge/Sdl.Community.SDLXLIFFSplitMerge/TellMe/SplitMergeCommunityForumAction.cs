using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLXLIFFSplitMerge.TellMe
{
	public class SplitMergeCommunityForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDLXLIFF Split/Merge results";
		public override Icon Icon => PluginResources.ForumIcon;

		public SplitMergeCommunityForumAction()
		{
			Name = "SDL Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("http://community.sdl.com/appsupport");
		}
	}
}