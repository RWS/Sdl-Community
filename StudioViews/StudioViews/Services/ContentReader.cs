using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace StudioViews.Services
{
	public class ContentReader : AbstractBilingualContentProcessor
	{
		public ContentReader()
		{
			ParagraphUnits = new List<IParagraphUnit>();
		}

		public List<IParagraphUnit> ParagraphUnits { get; }

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			ParagraphUnits.Add(paragraphUnit);
		}
	}
}
