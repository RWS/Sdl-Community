using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	[TellMeProvider]
	public class NumberVerifierTellMeProvider : ITellMeProvider
	{
		public string Name => "Number Verifier tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new NumVerifierCommunityWikiAction
			{
				Keywords = new[] {"number verifier", "number verifier community", "number verifier support", "number verifier wiki" }
			},
			new NumVerifierCommunityForumAction
			{
				Keywords = new[] { "number verifier", "number verifier community", "number verifier support", "number verifier forum" }
			},
			new NumVerifierStoreAction
			{
				Keywords = new[] { "number verifier", "number verifier store", "number verifier download", "number verifier appstore" }
			}
		};
	}
}