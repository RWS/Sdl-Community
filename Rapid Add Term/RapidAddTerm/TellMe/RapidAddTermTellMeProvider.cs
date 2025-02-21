using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	[TellMeProvider]
	public class RapidAddTermTellMeProvider : ITellMeProvider
	{
		public string Name => "Rapid Add Term Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new RapidAddTermDocumentationAction
			{
				Keywords = new[] {"rat", "rapid add term", "rapid add term documentation", "documentation"}
			},
			new RapidAddTermCommunitySupportAction
			{
				Keywords = new[] {"rat", "rapid add term", "rapid add term community", "rapid add term support", "forum"}
			},
			new RapidAddTermSourceCodeAction
			{
				Keywords = new[] {"rat", "rapid add term", "rapid add term source code", "source", "code" }
			}
            ,
            new RapiAddTermSettingsAction
            {
                Keywords = new[] {"rat", "rapid add term", "rapid add term settings", "settings" }
            }
        };
	}
}
