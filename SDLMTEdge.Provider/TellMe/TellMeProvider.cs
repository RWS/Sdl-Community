using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => WeaverEdgeResource.WeaverEdge_TellMeProviderName;

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "language", "weaver", "edge", "community", "support", "wiki" }
			},
			new Action
			{
				//Old Keywords = new[] { "sdlmtedge", "sdl mt edge", "sdl mt edge documentation" }
				Keywords = new[] { WeaverEdgeResource.WeaverEdge_TellMeActions, WeaverEdgeResource.WeaverEdge_TellMe_EdgeActionKey, WeaverEdgeResource.WeaverEdge_TellMe_EdgeActionOperation }
			},
			new StoreAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge store", "sdl mt edge download" }
					Keywords = new[] { WeaverEdgeResource.WeaverEdge_TellMeActions, WeaverEdgeResource.WeaverEdge_TellMe_EdgeStoreActionKey, WeaverEdgeResource.WeaverEdge_TellMe_EdgeStoreActionOperation }
			},
			new TranslationServerAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge", "enterprise translation server", "enterprise translation server documentation", "enterprise translation server website" }
				Keywords = new[] { WeaverEdgeResource.WeaverEdge_TellMeActions, "language weaver edge", WeaverEdgeResource.WeaverEdge_TellMe_EdgeTranslationServerActionKey, WeaverEdgeResource.WeaverEdge_TellMe_EdgeTranslationServerActionOperationDoc, WeaverEdgeResource.WeaverEdge_TellMe_EdgeTranslationServerActionOperationWebServer }
			},
			new OfficialWebAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge official", "sdl mt edge official web site", "sdl mt edge official documentation" }
				Keywords = new[] { WeaverEdgeResource.WeaverEdge_TellMeActions, WeaverEdgeResource.WeaverEdge_TellMe_EdgeOfficialWebActionKey,WeaverEdgeResource.WeaverEdge_TellMe_EdgeOfficialWebActionOperationWebServers,WeaverEdgeResource.WeaverEdge_TellMe_EdgeOfficialWebActionOperationDoc}
			},
			new LanguagePairsAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge language pairs" }
				Keywords = new[] { WeaverEdgeResource.WeaverEdge_TellMeActions, WeaverEdgeResource.WeaverEdge_TellMe_EdgeLanguagePairsActionOperation }
			},
			new CommunitySupportAction
			{
				//old Keywords = new[] { "sdlmtedge", "sdl mt edge community", "sdl mt edge support" }
				Keywords = new[] { WeaverEdgeResource.WeaverEdge_TellMeActions, WeaverEdgeResource.WeaverEdge_TellMe_EdgeCommunitySupportActionKey, WeaverEdgeResource.WeaverEdge_TellMe_EdgeCommunitySupportActionOperation }
			}
		};
	}
}