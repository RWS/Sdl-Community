using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public class StoreAction: AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = $"Download {PluginResources.SDLMTCloud_Provider_Name} from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/9");
		}

		public override bool IsAvailable => true;
		public override string Category => $"{PluginResources.SDLMTCloud_Provider_Name} results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}
