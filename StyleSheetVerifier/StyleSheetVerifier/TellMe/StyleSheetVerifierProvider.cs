using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StyleSheetVerifier.TellMe
{
	[TellMeProvider]
	public class StyleSheetVerifierProvider : ITellMeProvider
	{
		public string Name => "StyleSheet Verifier Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new StyleSheetVerifierWikiAction
			{
				Keywords = new[] {"stylesheetverifier", "stylesheet verifier", "stylesheetverifier community", "stylesheet verifier community", "stylesheetverifier support",
					"stylesheet verifier support", "stylesheetverifier wiki", "stylesheet verifier wiki" }
			},
			new StyleSheetVerifierCommunityAction
			{
				Keywords = new[] { "stylesheetverifier", "stylesheet verifier", "stylesheetverifier community", "stylesheet verifier community", "stylesheetverifier support",
					"stylesheet verifier support", "stylesheetverifier forum", "stylesheet verifier forum" }
			},
			new StyleSheetVerifierStoreAction
			{
				Keywords = new[] { "stylesheetverifier", "stylesheet verifier", "stylesheetverifier store", "stylesheet verifier store", "stylesheetverifier download",
			"stylesheet verifier download", "stylesheetverifier appstore", "stylesheet verifier appstore" }
			}
		};
	}
}