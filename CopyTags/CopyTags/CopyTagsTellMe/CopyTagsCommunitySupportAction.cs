using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsCommunitySupportAction : AbstractTellMeAction
	{
		public CopyTagsCommunitySupportAction()
		{
			Name = "SDL Community AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;
		public override string Category => "CopyTags results";
		public override Icon Icon => PluginResources.ForumIcon;
	}
}