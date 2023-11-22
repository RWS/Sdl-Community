using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => $"{Constants.MicrosoftNaming_FullName} - TellMe";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new StoreAction
			{
				Keywords = new[] { "mtp", "microsoft", "translator", "provider", "store", "update", "download" }
			},
			new CommunityForumAction()
			{
				Keywords = new[] { "mtp", "microsoft", "translator", "provider", "forum", "report", "community", "support" }
			},
			new HelpAction
			{
				Keywords = new[] { "mtp", "microsoft", "translator", "provider", "help", "guide", "wiki" }
			},
		};
	}
}