using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	public class CleanUpTasksHelpAction : AbstractTellMeAction
    {
        public CleanUpTasksHelpAction()
        {
            Name = "CleanUpTasks wiki in the RWS Community";
        }

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/4040/cleanup-tasks");
        }

		public override bool IsAvailable => true;
		public override string Category => "CleanUpTasks results";
		public override Icon Icon => PluginResources.Question;
	}
}