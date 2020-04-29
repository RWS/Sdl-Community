using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	public class FailSafeTaskStoreAction : AbstractTellMeAction
	{	
		public override bool IsAvailable => true;
		public override string Category => "Fail safe task results";
		public override Icon Icon => PluginResources.Download;

		public FailSafeTaskStoreAction()
		{
			Name = "Download Fail safe task plugin from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/fail-safe-task/560/");
		}
	}
}