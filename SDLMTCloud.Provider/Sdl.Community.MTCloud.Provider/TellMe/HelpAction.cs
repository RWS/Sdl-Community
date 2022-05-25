using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTCloud.Provider.TellMe
{
	public class HelpAction: AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = $"{PluginResources.SDLMTCloud_Provider_Name} wiki page";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3356");
		}

		public override bool IsAvailable => true;
		public override string Category => $"{PluginResources.SDLMTCloud_Provider_Name} results";
		public override Icon Icon => PluginResources.Question;
	}
}
