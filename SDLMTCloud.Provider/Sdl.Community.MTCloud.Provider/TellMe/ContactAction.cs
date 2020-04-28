using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public  class ContactAction: AbstractTellMeAction
	{
		public override void Execute()
		{
			Process.Start("https://translate.sdlbeglobal.com/");
		}
		public ContactAction()
		{
			Name = "SDL Machine Translation Cloud user interface";
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLMachineTranslationCloud results";
		public override Icon Icon => PluginResources.global;
	}
}
