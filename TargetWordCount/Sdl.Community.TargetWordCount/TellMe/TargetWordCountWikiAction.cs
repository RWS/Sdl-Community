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
			Name = "SDL Community Target word count plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/4039.target-word-count");
		}
	}
}