using System.Collections.Generic;

namespace StudioViews.Model
{
	public class ParagraphInfo
	{
		public ParagraphInfo()
		{
			SegmentInfos = new List<SegmentInfo>();
			SegmentationMrks = new List<MrkInfo>();
			ContextDefinitions = new List<ParagraphUnitContext>();
		}

		public string ParagraphId { get; set; }

		public List<SegmentInfo> SegmentInfos { get; set; }

		public List<MrkInfo> SegmentationMrks { get; set; }

		public List<ParagraphUnitContext> ContextDefinitions { get; set; }
	}
}
