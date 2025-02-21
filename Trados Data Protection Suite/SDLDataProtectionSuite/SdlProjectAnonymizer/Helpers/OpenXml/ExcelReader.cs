using DocumentFormat.OpenXml.Spreadsheet;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml
{
	public class ExcelReader
	{
		public ExcelReader(SharedStringTable sharedStringTable) => SharedStringTable = sharedStringTable;

		public SharedStringTable SharedStringTable { get; set; }

		public string GetCellText(string id) =>
					string.IsNullOrWhiteSpace(id) ? null : SharedStringTable?.ChildElements[int.Parse(id)].InnerText ?? id;
	}
}