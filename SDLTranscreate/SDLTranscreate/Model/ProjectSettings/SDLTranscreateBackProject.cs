using System;
using System.Collections.Generic;

namespace Trados.Transcreate.Model.ProjectSettings
{
	public class SDLTranscreateBackProject
	{
		public SDLTranscreateBackProject()
		{
			Created = FormatDateTime(DateTime.MinValue);
			DueDate = FormatDateTime(DateTime.MaxValue);
			ProjectFiles = new List<SDLTranscreateProjectFile>();
			TargetLanguages = new List<string>();
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public Customer Customer { get; set; }

		public string Created { get; set; }

		public string DueDate { get; set; }

		public string ProjectType { get; set; }

		public string SourceLanguage { get; set; }

		public List<string> TargetLanguages { get; set; }

		public List<SDLTranscreateProjectFile> ProjectFiles { get; set; }

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
