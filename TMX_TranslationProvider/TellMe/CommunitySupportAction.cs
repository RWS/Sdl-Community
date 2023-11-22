using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace TMX_TranslationProvider.Tellme
{
	public class TmxProviderCommunitySupportAction : AbstractTellMeAction
	{
		public TmxProviderCommunitySupportAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override string Category => "TmxProvider results";

		public override Icon Icon => PluginResources.ForumIcon;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f/rws-appstore");
		}
	}
}