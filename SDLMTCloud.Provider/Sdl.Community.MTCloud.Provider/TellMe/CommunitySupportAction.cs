using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public class CommunitySupportAction : AbstractTellMeAction
	{
		public CommunitySupportAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override string Category => $"{PluginResources.SDLMTCloud_Provider_Name} results";

		public override Icon Icon => PluginResources.ForumIcon;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}
	}
}