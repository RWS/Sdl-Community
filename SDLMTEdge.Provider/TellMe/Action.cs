using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class Action : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Language Weaver Edge results";

        public override Icon Icon => PluginResources.ForumIcon;

		public Action()
		{
			Name = "RWS official Language Weaver Edge documentation";
		}

		public override void Execute()
		{
			Process.Start("https://www.sdl.com/software-and-services/translation-software/machine-translation/");
		}
	}
}