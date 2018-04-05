using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class AnonymizerPreProcessor: AbstractBilingualContentHandler
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure) { return; }
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				var source = segmentPair.Source;
				var cleanUp = new CleanUpHandler();
				var text =cleanUp.GetText(segmentPair.Source);
			}
		}
	}
}
