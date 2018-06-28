using System.IO;
using OfficeOpenXml;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
{
	public static class ExcelFile
	{
		public static ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}
	}
}
