using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
        public override string Category => "Language Weaver Edge results";
        public override Icon Icon => PluginResources.Download;

		public StoreAction()
		{
			Name = "Language Weaver Edge results";

        }

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/40");
		}
	}
}