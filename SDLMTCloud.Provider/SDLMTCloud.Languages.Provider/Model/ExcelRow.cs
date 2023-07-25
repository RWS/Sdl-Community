using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Languages.Provider.Model
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
