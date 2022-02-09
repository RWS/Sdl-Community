using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.MTCloud.Provider.Service.RateIt
{
	public class SegmentRetriever : AbstractBilingualContentProcessor
	{
		public List<ISegment> Segments { get; set; } = new();

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) return;
			Segments.AddRange(paragraphUnit.SegmentPairs.Select(sp => sp.Source));
		}
	}
}