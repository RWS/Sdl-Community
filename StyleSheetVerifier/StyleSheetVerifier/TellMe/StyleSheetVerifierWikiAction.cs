using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StyleSheetVerifier.TellMe
{
	public class StyleSheetVerifierWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "StyleSheet Verifier results";
		public override Icon Icon => PluginResources.Question;

		public StyleSheetVerifierWikiAction()
		{
			Name = "StyleSheet Verifier Wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3170.stylesheet-preview");
		}
	}
}