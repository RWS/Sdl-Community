using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class LanguagePairsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
        public override string Category => "Language Weaver Edge results";
        public override Icon Icon => PluginResources.LanguagePairsIcon;

		public LanguagePairsAction()
		{
			Name = "Language Weaver Edge Language Pairs";
        }

		public override void Execute()
		{
			Process.Start("https://www.rws.com/language-weaver/supported-languages/");
		}
	}
}