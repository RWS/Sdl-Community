using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Languages.Provider.Model
{
	public class ExcelRow
	{
		public List<ExcelCell> Cells;

		public ExcelRow()
		{
			Cells = new List<ExcelCell>();
		}

		public int Index { get; set; }
	}
}