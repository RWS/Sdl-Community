using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace ProjectController
{
	public class FileProcessor: AbstractBilingualContentProcessor
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var originType = segmentPair?.Properties?.TranslationOrigin?.OriginType;

				if (!string.IsNullOrEmpty(originType))
				{
					if (originType.Equals("nmt") || originType.Equals("at"))
					{
						segmentPair.Properties.TranslationOrigin.OriginType = "interactive";
						segmentPair.Properties.TranslationOrigin.OriginSystem = string.Empty;
					}
				}

				var originBeforeAdaptation = segmentPair?.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginType;
				if (!string.IsNullOrEmpty(originBeforeAdaptation))
				{
					if (originBeforeAdaptation.Equals("nmt") || originBeforeAdaptation.Equals("at"))
					{
						segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginType = "interactive";
						segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem = string.Empty;
					}
				}
			}
		}
	}
}
