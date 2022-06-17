using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Model
{
	public class ParagraphUnitResult
	{
		public IParagraphUnit Paragraph { get; set; }
		
		public int ExcludedSegments { get; set; }

		public int UpdatedSegments { get; set; }
	}
}
