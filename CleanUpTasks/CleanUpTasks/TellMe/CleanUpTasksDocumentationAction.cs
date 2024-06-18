using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	public class CleanUpTasksDocumentationAction : AbstractTellMeAction
    {
        public CleanUpTasksDocumentationAction()
        {
            Name = "CleanUpTasks Documentation";
        }

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/23?tab=documentation");
        }

		public override bool IsAvailable => true;
		public override string Category => "CleanUpTasks results";
		public override Icon Icon => PluginResources.TellMe_Documentation;
	}
}