using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLBatchAnonymize.TellMe
{
	public class SdlBatchAnonymizerWikiAction: AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Trados Batch Anonymize results";
		public override Icon Icon => PluginResources.Question;

		public SdlBatchAnonymizerWikiAction()
		{
			Name = "Trados Batch Anonymizer wiki page";
		}
		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5249/trados-batch-anonymizer");
		}
	}
}
