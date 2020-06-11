using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	[TellMeProvider]
	public class SDLMTEdgeTellMeProvider : ITellMeProvider
	{
		public string Name => "SDL MT Edge tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SDLMTEdgeAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge", "sdl mt edge documentation" }
			},
			new SDLMTEdgeStoreAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge store", "sdl mt edge download" }
			},
			new SDLMTEdgeTranslationServerAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge", "enterprise translation server", "enterprise translation server documentation", "enterprise translation server website" }
			},
			new SDLMTEdgeOfficialWebAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge official", "sdl mt edge official web site", "sdl mt edge official documentation" }
			},
			new SDLMTEdgeLanguagePairsAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge language pairs" }
			},
			new SDLMTEdgeCommunitySupportAction
			{
				Keywords = new[] { "sdlmtedge", "sdl mt edge community", "sdl mt edge support" }
			}
		};
	}
}