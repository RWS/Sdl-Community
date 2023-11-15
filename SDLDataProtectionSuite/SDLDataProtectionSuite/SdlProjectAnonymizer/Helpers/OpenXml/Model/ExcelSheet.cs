using System.Collections.Generic;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml.Model
{
	public class ExcelSheet
	{
		public ExcelSheet()
		{
			Rows = new List<ExcelRow>();
		}

		public int Index { get; set; }

		public string Name { get; set; }


		public List<ExcelRow> Rows;
	}
}
