using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCommunityCleanUpTasks.TellMe
{
	public class CleanUpTasksSourceCodeAction : AbstractTellMeAction
    {
        public CleanUpTasksSourceCodeAction()
        {
            Name = "CleanUpTasks Source Code";
        }

        public override void Execute()
        {
            Process.Start("https://github.com/RWS/Sdl-Community/tree/master/CleanUpTasks");
        }

		public override bool IsAvailable => true;
		public override string Category => "CleanUpTasks results";
		public override Icon Icon => PluginResources.TellMe_SourceCode;
	}
}