using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	public class CleanUpTasksStoreAction : AbstractTellMeAction
    {
        public CleanUpTasksStoreAction()
        {
            Name = "Download CleanUpTasks from AppStore";
        }

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/23");
        }

		public override bool IsAvailable => true;
		public override string Category => "CleanUpTasks results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}