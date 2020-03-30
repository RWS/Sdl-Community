using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public class StoreAction: AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download SDLMachineTranslationCloud from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdl-beglobal-nmt/941/");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLMachineTranslationCloud results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}
