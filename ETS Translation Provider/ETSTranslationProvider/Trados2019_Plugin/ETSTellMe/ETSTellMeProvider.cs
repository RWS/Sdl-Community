using Sdl.TellMe.ProviderApi;

namespace ETSTranslationProvider.ETSTellMe
{
	[TellMeProvider]
	public class ETSTellMeProvider : ITellMeProvider
	{
		public string Name => "SDL MT Edge tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new ETSMTAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge", "sdl mt edge documentation" }
			},
			new ETSStoreAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge store", "sdl mt edge download" }
			},
			new ETSTranslationServerAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge", "enterprise translation server", "enterprise translation server documentation", "enterprise translation server website" }
			},
			new ETSOfficialWebAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge official", "sdl mt edge official web site", "sdl mt edge official documentation" }
			},
			new ETSLanguagePairsAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge language pairs" }
			},
			new ETSCommunitySupportAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge community", "sdl mt edge support" }
			}
		};
	}
}