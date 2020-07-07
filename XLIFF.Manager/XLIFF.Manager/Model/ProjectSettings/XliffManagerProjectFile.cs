using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model.ProjectSettings
{
	public class XliffManagerProjectFile
	{
		public XliffManagerProjectFile()
		{
			Status = Enumerators.Status.None.ToString();
			Action = Enumerators.Action.None.ToString();
			Date = FormatDateTime(DateTime.MinValue);
			Activities = new List<XliffManagerProjectFileActivity>();
		}

		public List<XliffManagerProjectFileActivity> Activities { get; set; }
		
		public ConfirmationStatistics ConfirmationStatistics { get; set; }
		
		public TranslationOriginStatistics TranslationOriginStatistics { get; set; }

		public string Status { get; set; }

		public string Action { get; set; }

		public string FileId { get; set; }

		public string TargetLanguage { get; set; }

		public string FileType { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public string Location { get; set; }

		public string Date { get; set; }

		public string Report { get; set; }

		public string XliffFilePath { get; set; }

		public string ShortMessage { get; set; }
	
		private static string FormatDateTime(DateTime dateTime)
		{
			var value = dateTime.Year
						+ "-" + dateTime.Month.ToString().PadLeft(2, '0')
						+ "-" + dateTime.Day.ToString().PadLeft(2, '0')
						+ "T" + dateTime.Hour.ToString().PadLeft(2, '0')
						+ ":" + dateTime.Minute.ToString().PadLeft(2, '0')
						+ ":" + dateTime.Second.ToString().PadLeft(2, '0')
						+ "." + dateTime.Millisecond.ToString().PadLeft(2, '0')
						+ "Z";

			return value;
		}
	}
}
