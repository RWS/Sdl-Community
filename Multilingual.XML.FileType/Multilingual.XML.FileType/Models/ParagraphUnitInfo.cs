using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.XML.FileType.Models
{
	public class ParagraphUnitInfo
	{
		public string ParagraphUnitId { get; set; }

		public int ParagraphUnitIndex { get; set; }

		public string FileId { get; set; }

		public bool IsCDATA { get; set; }

		public IParagraphUnit ParagraphUnit { get; set; }

		public List<SegmentPairInfo> SegmentPairs { get; set; }
	}
}
