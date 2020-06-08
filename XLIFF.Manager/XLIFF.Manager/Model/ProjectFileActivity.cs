using System;
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

		/// <summary>
		/// ignore from xml;json
		/// </summary>
		public ProjectFile ProjectFile { get; set; }

		public string ProjectFileId { get; set; }

		public Enumerators.Status Status { get; set; }

		public Enumerators.Action Action { get; set; }	
		
		public bool Selected { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }

		public string DateToString
		{
			get
			{
				var value = Date != DateTime.MinValue
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

		public string Details { get; set; }		

		public void Dispose()
		{
		}

		public object Clone()
		{
			var projectFileActivity = new ProjectFileActivity
			{
				ProjectFileId = ProjectFile.Id,
				Action = Action,
				Status = Status,
				Id = Id,
				Name = Name,
				Date = new DateTime(Date.Year, Date.Month, Date.Day, Date.Hour,
					Date.Minute, Date.Second, Date.Millisecond, Date.Kind),
				Path = Path,
				Details = Details,
				Selected = Selected
			};
			
			return projectFileActivity;
		}
	}
}
