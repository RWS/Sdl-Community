using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	public class FailSafeTaskCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Fail safe task results";
		public override Icon Icon => PluginResources.ForumIcon;

		public FailSafeTaskCommunityWikiAction()
		{
			Name = "SDL Community File safe task plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/4038.fail-safe-task");
		}
	}
}