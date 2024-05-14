using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	[TellMeProvider]
	public class NumberVerifierTellMeProvider : ITellMeProvider
	{
		public string Name => "Number Verifier Tell Me provider";

		public AbstractTellMeAction[] ProviderActions =>
		[
			new NumVerifierCommunityDocumentation
			{
				Keywords = ["number verifier", "number verifier community", "number verifier documentation"]
			},
			new NumVerifierCommunityForumAction
			{
				Keywords = ["number verifier", "number verifier community", "number verifier support", "number verifier forum"
				]
			},
			new NumVerifierSourceCodeAction
			{
				Keywords = ["number verifier","number verifier source", "number verifier source code"]
			},
			new SettingsAction
			{
				Keywords = ["number verifier settings"]
			}
		];
	}
}