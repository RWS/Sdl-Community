using System.Collections.Generic;

namespace Sdl.Community.StudioViews.Model
{
	public class ParagraphInfo
	{
		public ParagraphInfo()
		{
			SegmentIdInfos = new List<SegmentIdInfo>();
			SegmentationMrks = new List<MrkInfo>();
			ContextDefinitions = new List<ParagraphUnitContext>();
		}

		public string ParagraphId { get; set; }

		public List<SegmentIdInfo> SegmentIdInfos { get; set; }

		public List<MrkInfo> SegmentationMrks { get; set; }

		public List<ParagraphUnitContext> ContextDefinitions { get; set; }
	}
}
