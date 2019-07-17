using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	public class NumVerifierStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Number Verifier results";
		public override Icon Icon => PluginResources.Download;

		public NumVerifierStoreAction()
		{
			Name = "Download Number Verifier from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdl-number-verifier/440/");
		}
	}
}