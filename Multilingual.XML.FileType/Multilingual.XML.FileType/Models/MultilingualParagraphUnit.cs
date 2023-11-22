using System.Collections.Generic;

namespace Multilingual.XML.FileType.Models
{
	public class MultilingualParagraphUnit
	{
		public MultilingualParagraphUnit()
		{
			ParagraphUnitInfos = new List<ParagraphUnitInfo>();
		}
		
		public int ParagraphUnitIndex { get; set; }

		public List<ParagraphUnitInfo> ParagraphUnitInfos { get; set; }
	}
}
