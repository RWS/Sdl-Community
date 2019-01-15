using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace ETSTranslationProvider.ETSTellMe
{
	public class ETSCommunitySupportAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "ETS results";
		public override Icon Icon => PluginResources.ForumIcon;

		public ETSCommunitySupportAction()
		{
			Name = "SDL Community AppStore forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3450.ets-mt-provider");
		}
	}
}