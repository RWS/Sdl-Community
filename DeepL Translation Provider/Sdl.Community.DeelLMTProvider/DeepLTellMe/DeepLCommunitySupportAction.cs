using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLCommunitySupportAction : AbstractTellMeAction
	{
		public DeepLCommunitySupportAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";

		public override Icon Icon => PluginResources.Question;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}