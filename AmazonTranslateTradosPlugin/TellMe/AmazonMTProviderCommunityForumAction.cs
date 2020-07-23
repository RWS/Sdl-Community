using System.Diagnostics;
using System.Drawing;
using Sdl.Community.AmazonTranslateProvider;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin.TellMe
{
	public class AmazonMTProviderCommunityForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Amazon Translate MT Provider results";
		public override Icon Icon => PluginResources.ForumIcon;

		public AmazonMTProviderCommunityForumAction()
		{
			Name = "SDL Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("http://community.sdl.com/appsupport");
		}
	}
}