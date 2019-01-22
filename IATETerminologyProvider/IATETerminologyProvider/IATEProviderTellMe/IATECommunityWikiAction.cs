using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace IATETerminologyProvider.IATEProviderTellMe
{
	public class IATECommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.ForumIcon;

		public IATECommunityWikiAction()
		{
			Name = "SDL Community IATE plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3390.iate-terminology-provider");
		}
	}
}