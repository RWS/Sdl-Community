using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	public class BeGlobalStoreAction: AbstractTellMeAction
	{
		public BeGlobalStoreAction()
		{
			Name = "Download MachineTranslationCloud from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdl-beglobal-nmt/941/");
		}

		public override bool IsAvailable => true;
		public override string Category => "MachineTranslationCloud results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}
