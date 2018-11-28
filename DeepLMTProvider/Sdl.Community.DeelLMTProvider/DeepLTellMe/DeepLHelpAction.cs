using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLHelpAction : AbstractTellMeAction
	{
		public DeepLHelpAction()
		{
			Name = "DeepL help";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3266.deepl-mt-provider");
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";
		public override Icon Icon => PluginResources.Question;
	}
}
