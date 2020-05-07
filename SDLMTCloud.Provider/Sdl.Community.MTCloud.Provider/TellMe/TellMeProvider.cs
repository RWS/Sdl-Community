using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "SDL MT Cloud tell me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "sdl mt cloud", "machine translation", "community", "support" }
			},
			new ContactAction
			{
				Keywords = new []{ "sdl mt cloud", "machine translation", "contact", "trial" }
			},
			new HelpAction
			{
				Keywords = new []{ "sdl mt cloud", "machine translation", "help", "guide" }
			},
			new StoreAction
			{
				Keywords = new []{ "sdl mt cloud", "machine translation", "store", "download" }
			}
		};
	}
}