using Sdl.TellMe.ProviderApi;

namespace GoogleTranslatorProvider.TellMe
{
	[TellMeProvider]
	public class GTPTellMeProvider : ITellMeProvider
	{
		public string Name => $"{Constants.GooglePluginName} - TellMe";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new HelpAction
			{
				Keywords = HelpKeywords
			},
			new SettingsAction()
			{
				Keywords = SettingsKeywords
			},
			new CommunityForumAction()
			{
				Keywords = CommunityKeywords
			},
			new StoreAction
			{
				Keywords = AppStoreKeywords
			},
		};

		private readonly string[] HelpKeywords = { "gtp", "google provider", "machine translation", "help", "guide" };
		private readonly string[] SettingsKeywords = { "gtp", "google provider", "machine translation", "settings", "options" };
		private readonly string[] CommunityKeywords = { "gtp", "google provider", "machine translation", "community", "support" };
		private readonly string[] AppStoreKeywords = { "gtp", "google provider", "machine translation", "store", "download", "appstore", "update" };
	}
}