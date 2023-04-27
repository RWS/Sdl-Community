using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class TranslationServerAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Language Weaver Edge results";
		public override Icon Icon => PluginResources.ForumIcon;

		public TranslationServerAction()
		{
			Name = "RWS Enterprise translation server";
		}

		public override void Execute()
		{
			Process.Start("https://www.rws.com/language-weaver/");
		}
	}
}