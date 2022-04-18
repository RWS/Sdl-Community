using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class SDLMTEdgeStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => Properties.WeaverEdgeResource.WeaverEdge_CategoryResults;
		public override Icon Icon => PluginResources.Download;

		public SDLMTEdgeStoreAction()
		{
			Name = Properties.WeaverEdgeResource.WeaverEdge_StoreAction;
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdl-ets/843/");
		}
	}
}