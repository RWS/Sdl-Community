using System.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff
{
	public class DecryptDataProcessor : AbstractBilingualContentProcessor
	{
		private readonly AnonymizerSettings _decryptSettings;

		public DecryptDataProcessor(AnonymizerSettings decryptSettings)
		{
			_decryptSettings = decryptSettings;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (!(_decryptSettings.HasBeenCheckedByControl ?? false))
				return;

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