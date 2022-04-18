using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	[TellMeProvider]
	public class SDLMTEdgeTellMeProvider : ITellMeProvider
	{
		public string Name => Properties.WeaverEdgeResource.WeaverEdge_TellMeProviderName;

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SDLMTEdgeAction
			{
				//Old Keywords = new[] { "sdlmtedge", "sdl mt edge", "sdl mt edge documentation" }
				Keywords = new[] { Properties.WeaverEdgeResource.WeaverEdge_TellMeActions, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeActionKey, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeActionOperation }

			},
			new SDLMTEdgeStoreAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge store", "sdl mt edge download" }
					Keywords = new[] { Properties.WeaverEdgeResource.WeaverEdge_TellMeActions, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeStoreActionKey, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeStoreActionOperation }
			},
			new SDLMTEdgeTranslationServerAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge", "enterprise translation server", "enterprise translation server documentation", "enterprise translation server website" }
				Keywords = new[] { Properties.WeaverEdgeResource.WeaverEdge_TellMeActions, "sdl mt edge",Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeTranslationServerActionKey, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeTranslationServerActionOperationDoc, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeTranslationServerActionOperationWebServer }
			},
			new SDLMTEdgeOfficialWebAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge official", "sdl mt edge official web site", "sdl mt edge official documentation" }
				Keywords = new[] { Properties.WeaverEdgeResource.WeaverEdge_TellMeActions, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeOfficialWebActionKey,Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeOfficialWebActionOperationWebServers,Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeOfficialWebActionOperationDoc}
			},
			new SDLMTEdgeLanguagePairsAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge language pairs" }
				Keywords = new[] { Properties.WeaverEdgeResource.WeaverEdge_TellMeActions, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeLanguagePairsActionOperation }
			},
			new SDLMTEdgeCommunitySupportAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge community", "sdl mt edge support" }
				Keywords = new[] { Properties.WeaverEdgeResource.WeaverEdge_TellMeActions, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeCommunitySupportActionKey, Properties.WeaverEdgeResource.WeaverEdge_TellMe_EdgeCommunitySupportActionOperation }
				
			}
		};
	}
}