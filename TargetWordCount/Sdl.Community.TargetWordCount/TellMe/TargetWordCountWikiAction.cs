using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	public class TargetWordCountWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Target word count results";
		public override Icon Icon => PluginResources.ForumIcon;

		public TargetWordCountWikiAction()
		{
			Name = "RWS Community Target word count plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/4039/target-word-count");
		}
	}
}