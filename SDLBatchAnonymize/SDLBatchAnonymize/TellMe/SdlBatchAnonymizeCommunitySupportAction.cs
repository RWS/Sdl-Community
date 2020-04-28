using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLBatchAnonymize.TellMe
{
	public class SdlBatchAnonymizeCommunitySupportAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDL Batch Anonymize results";
		public override Icon Icon => PluginResources.ForumIcon;

		public SdlBatchAnonymizeCommunitySupportAction()
		{
			Name = "SDL Community AppStore forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}
	}
}
