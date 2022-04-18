using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class SDLMTEdgeAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => Properties.WeaverEdgeResource.WeaverEdge_CategoryResults;
		public override Icon Icon => PluginResources.ForumIcon;

		public SDLMTEdgeAction()
		{
			Name = "RWS official Weaver Edge documentation";
		}

		public override void Execute()
		{
			Process.Start("https://www.sdl.com/software-and-services/translation-software/machine-translation/");
		}
	}
}