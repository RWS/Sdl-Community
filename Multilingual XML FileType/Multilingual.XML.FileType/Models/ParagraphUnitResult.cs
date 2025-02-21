using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.XML.FileType.Models
{
	public class ParagraphUnitResult
	{
		public IParagraphUnit Paragraph { get; set; }

		public int ExcludedSegments { get; set; }

		public int UpdatedSegments { get; set; }
	}
}
