using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMDocumentationAction : AbstractTellMeAction
	{
		public ApplyTMDocumentationAction()
		{
			Name = "applyTM Template Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/21?tab=documentation");
		}

		public override bool IsAvailable => true;

		public override string Category => "applyTM Template results";

		public override Icon Icon => PluginResources.TellMe_Documentation;
	}
}