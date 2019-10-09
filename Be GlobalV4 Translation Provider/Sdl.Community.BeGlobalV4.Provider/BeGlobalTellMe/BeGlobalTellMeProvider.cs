using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	[TellMeProvider]
	public class BeGlobalTellMeProvider : ITellMeProvider
	{
		public string Name => "MachineTranslationCloud tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new BeGlobalCommunitySupportAction
			{
				Keywords = new []{ "machinetranslationcloud", "machinetranslationcloud community", "machinetranslationcloud support" }
			},
			new BeGlobalContactAction
			{
				Keywords = new []{ "machinetranslationcloud", "machinetranslationcloud contact", "machinetranslationcloud trial" }
			},
			new BeGlobalHelpAction
			{
				Keywords = new []{ "machinetranslationcloud", "machinetranslationcloud help", "machinetranslationcloud guide" }
			},
			new BeGlobalStoreAction
			{
				Keywords = new []{ "machinetranslationcloud", "machinetranslationcloud store", "machinetranslationcloud download" }
			},
			new BeGlobalSettingsAction
			{
				Keywords = new []{ "machinetranslationcloud", "machinetranslationcloud settings ", "settings" }
			}
		};
	}
}