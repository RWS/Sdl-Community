using System;
using System.Collections.Generic;

namespace CustomViewExample.Model
{
	public class CustomViewProject
	{
		public string Id { get; set; }
		
		public string Name { get; set; }

		public string Path { get; set; }
		
		public string Description { get; set; }

		public DateTime DueDate { get; set; }

		public string DueDateToString => GetDateTimeToString(DueDate);

		public DateTime Created { get; set; }

		public string CreatedToString => GetDateTimeToString(Created);

		public string ProjectType { get; set; }
		
		public string ProjectOrigin { get; set; }

		public CustomViewLanguage SourceLanguage { get; set; }

		public List<CustomViewLanguage> TargetLanguages { get; set; }

		private string GetDateTimeToString(DateTime dateTime)
		{
			var value = (dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
				? dateTime.Year
				  + "-" + dateTime.Month.ToString().PadLeft(2, '0')
				  + "-" + dateTime.Day.ToString().PadLeft(2, '0')
				  + " " + dateTime.Hour.ToString().PadLeft(2, '0')
				  + ":" + dateTime.Minute.ToString().PadLeft(2, '0')
				  + ":" + dateTime.Second.ToString().PadLeft(2, '0')
				: "[none]";
			return value;
		}
	}
}
