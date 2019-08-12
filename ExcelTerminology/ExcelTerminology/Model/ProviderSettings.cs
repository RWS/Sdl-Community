using System;
using System.Globalization;
using System.IO;

namespace ExcelTerminology.Model
{
	public class ProviderSettings
	{
		public string SourceColumn { get; set; }
		public string TargetColumn { get; set; }
		public string ApprovedColumn { get; set; }
		public bool HasHeader { get; set; }
		public string TermFilePath { get; set; }
		public CultureInfo SourceLanguage { get; set; }
		public CultureInfo TargetLanguage { get; set; }
		public char Separator { get; set; }
		public string WorksheetName { get; set; }
		public bool IsReadOnly { get; set; }
		public Uri Uri { get; set; }

		public bool IsFileReady()
		{
			var result = true;
			try
			{
				using (new FileStream(TermFilePath, FileMode.OpenOrCreate))
				{
				}
			}
			catch (IOException)
			{
				result = false;
			}
			return result;
		}
	}
}