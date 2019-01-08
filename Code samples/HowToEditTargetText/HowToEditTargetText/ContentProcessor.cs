using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace HowToEditTargetText
{
	public class ContentProcessor : AbstractBilingualContentProcessor
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var segmentVisitor = new SegmentVisitor();
				if (segmentPair.Target != null)
				{
					segmentVisitor.AddText(segmentPair.Target);
				}  
			}
		}
	}
}
