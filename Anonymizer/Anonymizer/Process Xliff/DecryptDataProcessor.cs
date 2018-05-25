using System.Linq;
using Sdl.Community.projectAnonymizer.Batch_Task;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.projectAnonymizer.Process_Xliff
{
	public class DecryptDataProcessor : AbstractBilingualContentProcessor
	{
		private readonly DecryptSettings _decryptSettings;
		public DecryptDataProcessor(DecryptSettings decryptSettings)
		{
			_decryptSettings = decryptSettings;
		}
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }

			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var decryptVisitor = new DecryptSegmentVisitor(_decryptSettings);
				decryptVisitor.DecryptText(segmentPair.Source, ItemFactory, PropertiesFactory);
				if (segmentPair.Target != null)
				{
					decryptVisitor.DecryptText(segmentPair.Target, ItemFactory, PropertiesFactory);
				}
			}
		}
	}
}
