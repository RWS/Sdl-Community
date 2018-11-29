using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MtEnhancedProvider.MTEnhancedTellMe
{
	public class MTEnhancedHelpAction : AbstractTellMeAction
	{
		public MTEnhancedHelpAction()
		{
			Name = "MTEnhanced Provider help";
		}

		public override void Execute()
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/1271.microsoft-translator-credentials-for-mt-enhanced-app");
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Question;
	}
}
