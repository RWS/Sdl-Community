using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	public class TargetWordCountCommunityForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.TellMe_Forum;

		public TargetWordCountCommunityForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}