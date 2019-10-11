using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	public  class BeGlobalContactAction: AbstractTellMeAction
	{
		public override void Execute()
		{
			Process.Start("https://translate.sdlbeglobal.com/");
		}
		public BeGlobalContactAction()
		{
			Name = "SDL Machine Translation Cloud user interface";
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLMachineTranslationCloud results";
		public override Icon Icon => PluginResources.global;
	}
}
