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
				Keywords = HelpKeywords
			},
			new StoreAction
			{
				Keywords = AppStoreKeywords
			},
		};

		private readonly string[] AppStoreKeywords = {
			"google", "google app", "google store", "google appstore", "google app store", "google download", "google update",
			"google translator", "google translator app", "google translator store", "google translator appstore", "google translator app store", "google translator download", "google translator update",
			"googletranslator", "googletranslator app", "googletranslator store", "googletranslator appstore", "googletranslator app store", "googletranslator download", "googletranslator update",
			"google provider", "google provider app", "google provider store", "google provider appstore", "google provider app store", "google provider download", "google provider update",
			"googleprovider", "googleprovider app", "googleprovider store", "googleprovider appstore", "googleprovider app store", "googleprovider download", "googleprovider update",
			"google translate", "google translate app", "google translate store", "google translate appstore", "google translate app store", "google translate download", "google translate update",
			"googletranslate", "googletranslate app", "googletranslate store", "googletranslate appstore", "googletranslate app store", "googletranslate download", "googletranslate update",
			"google translator provider", "google translator provider app", "google translator provider store", "google translator provider appstore", "google translator provider app store", "google translator provider download", "google translator provider update",
			"googletranslatorprovider", "googletranslatorprovider app", "googletranslatorprovider store", "googletranslatorprovider appstore", "googletranslatorprovider app store", "googletranslatorprovider download", "googletranslatorprovider update",
			"google translate provider", "google translate provider app", "google translate provider store", "google translate provider appstore", "google translate provider app store", "google translate provider download", "google translate provider update",
			"googletranslateprovider", "googletranslateprovider app", "googletranslateprovider store", "googletranslateprovider appstore", "googletranslateprovider app store", "googletranslateprovider download", "googletranslateprovider update",
			"gt", "gt app", "gt store", "gt appstore", "gt app store", "gt download", "gt update",
			"gtp", "gtp app", "gtp store", "gtp appstore", "gtp app store", "gtp download", "gtp update" };

		private readonly string[] SettingsKeywords = {
			"google", "google settings", "google options", "google project settings", "google translator", "google translator settings", "google translator options",
			"google translator project settings", "googletranslator", "googletranslator settings", "googletranslator options", "googletranslator project settings", "google provider", "google provider settings",
			"google provider options", "google provider project settings", "googleprovider", "googleprovider settings", "googleprovider options", "googleprovider project settings", "google translate",
			"google translate settings", "google translate options", "google translate project settings", "googletranslate", "googletranslate settings", "googletranslate options", "googletranslate project settings",
			"google translator provider", "google translator provider settings", "google translator provider options", "google translator provider project settings",
			"googletranslatorprovider", "googletranslatorprovider settings", "googletranslatorprovider options", "googletranslatorprovider project settings",
			"google translate provider", "google translate provider settings", "google translate provider options", "google translate provider project settings", "googletranslateprovider", "googletranslateprovider settings",
			"googletranslateprovider options", "googletranslateprovider project settings", "gt", "gt settings", "gt options", "gt project settings", "gtp",
			"gtp settings", "gtp options", "gtp project settings" };

		private readonly string[] HelpKeywords = {
			"google", "google help", "google guide", "google wiki", "google community", "google forum", "google question",
			"google report", "google translator", "google translator help", "google translator guide", "google translator wiki", "google translator community", "google translator forum",
			"google translator question", "google translator report", "googletranslator", "googletranslator help", "googletranslator guide", "googletranslator wiki", "googletranslator community",
			"googletranslator forum", "googletranslator question", "googletranslator report", "google provider", "google provider help", "google provider guide", "google provider wiki",
			"google provider community", "google provider forum", "google provider question", "google provider report", "googleprovider", "googleprovider help", "googleprovider guide",
			"googleprovider wiki", "googleprovider community", "googleprovider forum", "googleprovider question", "googleprovider report", "google translate", "google translate help",
			"google translate guide", "google translate wiki", "google translate community", "google translate forum", "google translate question", "google translate report", "googletranslate",
			"googletranslate help", "googletranslate guide", "googletranslate wiki", "googletranslate community", "googletranslate forum", "googletranslate question", "googletranslate report",
			"google translator provider", "google translator provider help", "google translator provider guide", "google translator provider wiki", "google translator provider community", "google translator provider forum", "google translator provider question",
			"google translator provider report", "googletranslatorprovider", "googletranslatorprovider help", "googletranslatorprovider guide", "googletranslatorprovider wiki", "googletranslatorprovider community", "googletranslatorprovider forum",
			"googletranslatorprovider question", "googletranslatorprovider report", "google translate provider", "google translate provider help", "google translate provider guide", "google translate provider wiki", "google translate provider community",
			"google translate provider forum", "google translate provider question", "google translate provider report", "googletranslateprovider", "googletranslateprovider help", "googletranslateprovider guide", "googletranslateprovider wiki",
			"googletranslateprovider community", "googletranslateprovider forum", "googletranslateprovider question", "googletranslateprovider report", "gt", "gt help", "gt guide",
			"gt wiki", "gt community", "gt forum", "gt question", "gt report", "gtp", "gtp help",
			"gtp guide", "gtp wiki", "gtp community", "gtp forum", "gtp question", "gtp report" };
	}
}