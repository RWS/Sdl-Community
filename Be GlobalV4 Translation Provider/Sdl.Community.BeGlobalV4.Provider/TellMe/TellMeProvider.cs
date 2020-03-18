using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "SDLMachineTranslationCloud tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud community", "sdlmachinetranslationcloud support" }
			},
			new ContactAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud contact", "sdlmachinetranslationcloud trial" }
			},
			new HelpAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud help", "sdlmachinetranslationcloud guide" }
			},
			new StoreAction
			{
				Keywords = new []{ "sdlmachinetranslationcloud", "sdl machine translation cloud", "sdlmachinetranslationcloud store", "sdlmachinetranslationcloud download" }
			}
		};
	}
}