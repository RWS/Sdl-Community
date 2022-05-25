using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class SDLMTEdgeLanguagePairsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => Properties.WeaverEdgeResource.WeaverEdge_CategoryResults;
		public override Icon Icon => PluginResources.LanguagePairsIcon;

		public SDLMTEdgeLanguagePairsAction()
		{
			Name = Properties.WeaverEdgeResource.WeaverEdge_LanguagePairsActionName;
		}

		public override void Execute()
		{
			Process.Start("https://www.sdl.com/products-and-solutions/translation/sdl-machine-translation/available-languages");
		}
	}
}
