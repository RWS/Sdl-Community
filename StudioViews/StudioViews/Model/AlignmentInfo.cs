﻿using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Model
{
	public class AlignmentInfo
	{

		public enum AlignmentType
		{
			None,
			Added,
			Removed,
			Matched,
			LeftOnly,
			MarkupData
		}
		
		public string SortId { get; set; }
		
		public string SegmentId { get; set; }
		
		public AlignmentType Alignment { get; set; }

		public ISegmentPair SegmentPairLeft { get; set; }

		public ISegmentPair SegmentPairRight { get; set; }

		public IAbstractMarkupData MarkupData { get; set; }

	}
}
