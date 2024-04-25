using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	public class NumVerifierCommunityDocumentation : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Number Verifier results";
		public override Icon Icon => PluginResources.TellmeDocumentation;

		public NumVerifierCommunityDocumentation()
		{
			Name = "Trados Number Verifier Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/33?tab=documentation");
		}
	}
}