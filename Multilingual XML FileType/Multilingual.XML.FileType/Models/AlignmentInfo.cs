using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.XML.FileType.Models
{
	public class AlignmentInfo
	{

		public enum AlignmentType
		{
			None,
			Added,
			Removed,
			Matched,
			LeftOnly
		}

		public string SortId { get; set; }

		public string SegmentId { get; set; }

		public AlignmentType Alignment { get; set; }

		public ISegmentPair SegmentPairLeft { get; set; }

		public ISegmentPair SegmentPairRight { get; set; }

	}
}
