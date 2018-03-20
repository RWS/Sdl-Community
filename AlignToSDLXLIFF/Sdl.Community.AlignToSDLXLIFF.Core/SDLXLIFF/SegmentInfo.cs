using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.Amgen.Core.SDLXLIFF
{
	public class SegmentInfo
	{		
        public string SegmentId { get; set; }

		public string ParagraphId { get; set; }

		public ISegmentPair SegmentPair { get; set; }

		public Segment SourceSegment { get; set; }

		public Segment TargetSegment { get; set; }
	}
}