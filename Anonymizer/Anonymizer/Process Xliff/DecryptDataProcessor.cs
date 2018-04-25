using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.projectAnonymizer.Process_Xliff
{
	public class DecryptDataProcessor : AbstractBilingualContentProcessor
	{
		private readonly string _encryptionKey;
		public DecryptDataProcessor(string encryptionKey)
		{
			_encryptionKey = encryptionKey;
		}
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }

			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var decryptVisitor = new DecryptSegmentVisitor(_encryptionKey);
				decryptVisitor.DecryptText(segmentPair.Source, ItemFactory, PropertiesFactory);
				if (segmentPair.Target != null)
				{
					decryptVisitor.DecryptText(segmentPair.Target, ItemFactory, PropertiesFactory);
				}
			}
		}
	}
}
