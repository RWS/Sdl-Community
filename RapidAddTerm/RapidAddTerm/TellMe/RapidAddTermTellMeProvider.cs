using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	[TellMeProvider]
	public class RapidAddTermTellMeProvider : ITellMeProvider
	{
		public string Name => "Rapid Add Term tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new RapidAddTermStoreAction
			{
				Keywords = new[] {"rapid add term", "rapid add term store", "rapid add term download"}
			},
			new RapidAddTermCommunitySupportAction
			{
				Keywords = new[] {"rapid add term", "rapid add term community", "rapid add term support"}
			},
			new RapidAddTermHelpAction
			{
				Keywords = new[] {"rapid add term", "rapid add term help", "rapid add term guide", "rapid add term wiki" }
			}
		};
	}
}
