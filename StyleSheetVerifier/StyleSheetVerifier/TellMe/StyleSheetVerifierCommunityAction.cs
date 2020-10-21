using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StyleSheetVerifier.TellMe
{
	public class StyleSheetVerifierCommunityAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "StyleSheet Verifier results";
		public override Icon Icon => PluginResources.ForumIcon;

		public StyleSheetVerifierCommunityAction()
		{
			Name = "SDL Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("http://community.sdl.com/appsupport");
		}
	}
}