using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Model
{
	public class SegmentPairInfo
	{
		public string FileId { get; set; }
		
		public string ParagraphUnitId { get; set; }
		
		public string SegmentId { get; set; }
		
		public IParagraphUnit ParagraphUnit { get; set; }
		
		public ISegmentPair SegmentPair { get; set; }
		
		public Toolkit.LanguagePlatform.Models.WordCounts SourceWordCounts { get; set; }
	}
}
