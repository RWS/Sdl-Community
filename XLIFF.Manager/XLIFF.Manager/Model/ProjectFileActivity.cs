using System;
using System.Xml.Serialization;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActivity: BaseModel, IDisposable, ICloneable
	{	
		public ProjectFileActivity()
		{			
			Status = Enumerators.Status.None;
			Action = Enumerators.Action.None;
		}

		[XmlIgnore]
		public ProjectFile ProjectFile { get; set; }

		public ConfirmationStatistics ConfirmationStatistics { get; set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; set; }

		public string ProjectFileId { get; set; }

		public Enumerators.Status Status { get; set; }

		public Enumerators.Action Action { get; set; }	
		
		public bool Selected { get; set; }

		public string ActivityId { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }

		public string DateToString
		{
			get
			{
				var value = (Date != DateTime.MinValue && Date != DateTime.MaxValue)
					? Date.Year
					  + "-" + Date.Month.ToString().PadLeft(2, '0')
					  + "-" + Date.Day.ToString().PadLeft(2, '0')
					  + " " + Date.Hour.ToString().PadLeft(2, '0')
					  + ":" + Date.Minute.ToString().PadLeft(2, '0')
					  + ":" + Date.Second.ToString().PadLeft(2, '0')
					: "[none]";

				return value;
			}
		}

		public string Report { get; set; }		

		public void Dispose()
		{
		}

		public object Clone()
		{
			var projectFileActivity = new ProjectFileActivity
			{
				ProjectFileId = ProjectFileId,
				Action = Action,
				Status = Status,
				ActivityId = ActivityId,
				Name = Name,
				Date = new DateTime(Date.Ticks, DateTimeKind.Utc),
				Path = Path,
				Report = Report,
				Selected = Selected
			};

			if (ConfirmationStatistics != null)
			{
				projectFileActivity.ConfirmationStatistics = ConfirmationStatistics.Clone() as ConfirmationStatistics;
			}

			if (TranslationOriginStatistics != null)
			{
				projectFileActivity.TranslationOriginStatistics = TranslationOriginStatistics.Clone() as TranslationOriginStatistics;
			}

			return projectFileActivity;
		}
	}
}
