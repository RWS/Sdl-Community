using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	public class FailSafeTaskDocumantationAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Fail Safe Task results";
		public override Icon Icon => PluginResources.TellMe_Documentation;

		public FailSafeTaskDocumantationAction()
		{
			Name = "Fail Safe Task Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/28?tab=documentation");
		}
	}
}