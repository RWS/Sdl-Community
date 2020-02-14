using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	public class RapidAddTermCommunitySupportAction:AbstractTellMeAction
	{
		public RapidAddTermCommunitySupportAction()
		{
			Name = "SDL Community AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}
