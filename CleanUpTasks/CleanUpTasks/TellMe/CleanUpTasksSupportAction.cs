using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	public class CleanUpTasksSupportAction : AbstractTellMeAction
    {
        public CleanUpTasksSupportAction()
        {
            Name = "RWS Community AppStore forum";
        }

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f/rws-appstore");
        }

		public override bool IsAvailable => true;
		public override string Category => "CleanUpTasks results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}