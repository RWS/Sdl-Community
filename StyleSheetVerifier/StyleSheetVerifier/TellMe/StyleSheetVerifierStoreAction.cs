using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StyleSheetVerifier.TellMe
{
	public class StyleSheetVerifierStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "StyleSheet Verifier results";
		public override Icon Icon => PluginResources.Download;

		public StyleSheetVerifierStoreAction()
		{
			Name = "Download StyleSheet Verifier from the AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/stylesheet-verifier/870/");
		}
	}
}