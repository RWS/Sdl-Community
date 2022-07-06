using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => $"{PluginResources.SDLMTCloud_Provider_Name} tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ $"{PluginResources.SDLMTCloud_Provider_Name.ToLower()}", "machine translation", "community", "support", "lw" }
			},
			new ContactAction
			{
				Keywords = new []{ $"{PluginResources.SDLMTCloud_Provider_Name.ToLower()}", "machine translation", "contact", "trial", "lw" }
			},
			new HelpAction
			{
				Keywords = new []{ $"{PluginResources.SDLMTCloud_Provider_Name.ToLower()}", "machine translation", "help", "guide", "lw" }
			},
			new StoreAction
			{
				Keywords = new []{ $"{PluginResources.SDLMTCloud_Provider_Name.ToLower()}", "machine translation", "store", "download", "lw" }
			}
		};
	}
}