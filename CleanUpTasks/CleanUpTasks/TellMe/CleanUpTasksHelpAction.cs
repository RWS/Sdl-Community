using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	public class CleanUpTasksHelpAction : AbstractTellMeAction
    {
        public CleanUpTasksHelpAction()
        {
            Name = "CleanUpTasks wiki in the SDL Community";
        }

        public override void Execute()
        {
            Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/4040.cleanup-tasks");
        }

		public override bool IsAvailable => true;
		public override string Category => "CleanUpTasks results";
		public override Icon Icon => PluginResources.Question;
	}
}