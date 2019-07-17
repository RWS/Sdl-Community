using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.NumberVerifier.TellMe
{
	public class NumVerifierCommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Number Verifier results";
		public override Icon Icon => PluginResources.ForumIcon;

		public NumVerifierCommunityWikiAction()
		{
			Name = "SDL Community Number Verifier plugin wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/4723/number-verifier");
		}
	}
}