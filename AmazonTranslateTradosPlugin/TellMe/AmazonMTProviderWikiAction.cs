using System.Diagnostics;
using System.Drawing;
using Sdl.Community.AmazonTranslateProvider;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin.TellMe
{
	public class AmazonMTProviderWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Amazon Translate MT Provider results";
		public override Icon Icon => PluginResources.question;

		public AmazonMTProviderWikiAction()
		{
			Name = "SDL Community Amazon Translate MT Provider Wiki";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3315/amazon-translate-mt-provider");
		}
	}
}