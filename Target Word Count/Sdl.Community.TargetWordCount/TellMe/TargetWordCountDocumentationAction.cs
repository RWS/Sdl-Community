using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	public class TargetWordCountDocumentationAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.TellmeDocumentation;

		public TargetWordCountDocumentationAction()
		{
			Name = $"{PluginResources.Plugin_Name} Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/74?tab=documentation");
		}
	}
}