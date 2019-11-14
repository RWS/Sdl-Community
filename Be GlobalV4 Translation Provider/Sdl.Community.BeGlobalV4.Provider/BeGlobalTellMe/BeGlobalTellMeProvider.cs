using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	[TellMeProvider]
	public class BeGlobalTellMeProvider : ITellMeProvider
	{
		public string Name => "SDLMachineTranslationCloud tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new BeGlobalCommunitySupportAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud community", "sdlmachinetranslationcloud support" }
			},
			new BeGlobalContactAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud contact", "sdlmachinetranslationcloud trial" }
			},
			new BeGlobalHelpAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud help", "sdlmachinetranslationcloud guide" }
			},
			new BeGlobalStoreAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud store", "sdlmachinetranslationcloud download" }
			}
		};
	}
}