using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class OfficialWebAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
        public override string Category => "Language Weaver Edge results";

        public override Icon Icon => PluginResources.ForumIcon;

        public OfficialWebAction()
		{
			Name = "Official Language Weaver Edge Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://docs.rws.com/791581/396549/sdl-ets-documentation/welcome-to-the-------------sdl-ets-------------8-1-x-documentation");
		}
	}
}