using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StudioViews.TellMe
{
	public class CommunityAppStoreForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		
		public override string Category => "Studio Views results";
		
		public override Icon Icon => PluginResources.ForumIcon;

		public CommunityAppStoreForumAction()
		{
			Name = "SDL Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("http://community.sdl.com/appsupport");
		}
	}
}
