using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLBatchAnonymize.TellMe
{
	public class SdlBatchAnonymizerWikiAction: AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDL Batch Anonymize results";
		public override Icon Icon => PluginResources.Question;

		public SdlBatchAnonymizerWikiAction()
		{
			Name = "SDL Batch Anonymizer wiki page";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5249/sdl-batch-anonymizer");
		}
	}
}
