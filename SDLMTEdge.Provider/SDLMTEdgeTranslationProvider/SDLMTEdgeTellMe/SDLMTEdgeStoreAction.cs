using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class SDLMTEdgeStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDL MT Edge results";
		public override Icon Icon => PluginResources.Download;

		public SDLMTEdgeStoreAction()
		{
			Name = "Download SDL MT Edge provider from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdl-ets/843/");
		}
	}
}