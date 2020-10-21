using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class SDLMTEdgeTranslationServerAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDL MT Edge results";
		public override Icon Icon => PluginResources.ForumIcon;

		public SDLMTEdgeTranslationServerAction()
		{
			Name = "SDL Enterprise translation server";
		}

		public override void Execute()
		{
			Process.Start("https://www.sdl.com/software-and-services/translation-software/machine-translation/enterprise-translation-server.html");
		}
	}
}