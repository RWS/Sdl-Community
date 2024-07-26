using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AntidoteVerifier.TellMe
{
	public class DocumentationAction : AbstractTellMeAction
	{
		public DocumentationAction()
		{
			Name = "Antidote Verifier Documentation";
		}

		public override Icon Icon => PluginResources.Question;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start(
				"https://appstore.rws.com/Plugin/3?tab=documentation");
		}
	}
}