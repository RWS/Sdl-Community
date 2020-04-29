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
			{
				return;
			}

			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			var segmentPairs = paragraphUnit.SegmentPairs.ToList();

			var decryptVisitor = new DecryptSegmentVisitor(ItemFactory, PropertiesFactory, _decryptSettings);

			if (segmentPairs.Count == 0 && paragraphUnit.Source != null)
			{
				decryptVisitor.DecryptText(paragraphUnit.Source);
			}
			else
			{
				foreach (var segmentPair in segmentPairs)
				{					
					decryptVisitor.DecryptText(segmentPair.Source);
					if (segmentPair.Target != null)
					{
						decryptVisitor.DecryptText(segmentPair.Target);
					}
				}
			}
		}
	}
}