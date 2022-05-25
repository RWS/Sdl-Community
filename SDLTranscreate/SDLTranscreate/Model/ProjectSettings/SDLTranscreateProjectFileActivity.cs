using System;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.Model.ProjectSettings
{
	public class SDLTranscreateProjectFileActivity
	{
		public SDLTranscreateProjectFileActivity()
		{
			Status = Enumerators.Status.None.ToString();
			Action = Enumerators.Action.None.ToString();
			WorkFlow = Enumerators.WorkFlow.None.ToString();
			Date = FormatDateTime(DateTime.MinValue);
		}
		public string ProjectFileId { get; set; }

		public string Status { get; set; }

		public string Action { get; set; }

		public string WorkFlow { get; set; }

		public string ActivityId { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public string Date { get; set; }

		public string Report { get; set; }
		
		public ConfirmationStatistics ConfirmationStatistics { get; set; }
		
		public TranslationOriginStatistics TranslationOriginStatistics { get; set; }

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
