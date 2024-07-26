using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FailSafeTask.TellMe
{
	public class FailSafeTaskCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Fail Safe Task results";
		public override Icon Icon => PluginResources.Question;

		public FailSafeTaskCommunityWikiAction()
		{
			Name = "RWS Community File safe task plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/4038/fail-safe-task");
		}
	}
}