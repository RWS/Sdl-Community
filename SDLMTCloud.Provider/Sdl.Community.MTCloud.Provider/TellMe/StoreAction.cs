using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = $"Download {PluginResources.SDLMTCloud_Provider_Name} from AppStore";
		}

		public override string Category => $"{PluginResources.SDLMTCloud_Provider_Name} results";

		public override Icon Icon => PluginResources.TellMe1;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/language-weaver-cloud/941/");
		}
	}
}