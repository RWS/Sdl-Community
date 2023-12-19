namespace Multilingual.Excel.FileType.Providers.OpenXml.Model
{
	public class ExcelOptions
	{
		public int FirstRowIndex { get; set; }

		public bool FirstRowIndexIsHeading { get; set; }

		public bool ReadAllWorkSheets { get; set; }

		public bool ReadHyperlinks { get; set; }
	}
}
