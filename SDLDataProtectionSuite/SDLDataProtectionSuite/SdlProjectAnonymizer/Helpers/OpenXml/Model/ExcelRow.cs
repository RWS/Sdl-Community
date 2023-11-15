using System.Collections.Generic;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml.Model
{
	public class ExcelRow
	{
		public ExcelRow()
		{
			Cells = new List<ExcelCell>();
		}

		public uint Index { get; set; }


		public List<ExcelCell> Cells;
	}
}
