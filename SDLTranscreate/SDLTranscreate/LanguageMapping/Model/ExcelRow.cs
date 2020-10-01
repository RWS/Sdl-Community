using System.Collections.Generic;

namespace Sdl.Community.Transcreate.LanguageMapping.Model
{
	public class ExcelRow
	{
		public ExcelRow()
		{
			Cells = new List<ExcelCell>();
		}

		public int Index { get; set; }


		public List<ExcelCell> Cells;
	}
}
