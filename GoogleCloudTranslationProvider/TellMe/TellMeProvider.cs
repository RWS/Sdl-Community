using Sdl.TellMe.ProviderApi;

namespace GoogleCloudTranslationProvider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => $"{Constants.GoogleNaming_FullName} - TellMe";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new StoreAction
			{
				Keywords = AppStoreKeywords
			},
			new CommunityForumAction()
			{
				Keywords = CommunityKeywords
			},
			new HelpAction
			{
				Keywords = HelpKeywords
			},
		};

		private readonly string[] HelpKeywords = { "gctp", "google cloud provider", "machine translation", "help", "guide" };
		private readonly string[] CommunityKeywords = { "gctp", "google cloud provider", "machine translation", "community", "support" };
		private readonly string[] AppStoreKeywords = { "gctp", "google cloud provider", "machine translation", "store", "download", "appstore", "update" };
	}
}