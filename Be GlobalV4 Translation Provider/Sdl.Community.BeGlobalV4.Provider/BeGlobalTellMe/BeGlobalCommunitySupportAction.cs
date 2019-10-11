using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BeGlobalV4.Provider.BeGlobalTellMe
{
	public class BeGlobalCommunitySupportAction : AbstractTellMeAction
	{
		public BeGlobalCommunitySupportAction()
		{
			Name = "SDL Community AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLMachineTranslationCloud results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}

