using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFile : BaseModel, IDisposable, ICloneable
	{
		private DateTime _date;
		private bool _selected;
		private string _path;
		private string _importedFilePath;
		private Enumerators.Status _status;

		public ProjectFile()
		{
			ProjectFileActivities = new List<ProjectFileActivity>();			
		}

		public string ProjectId { get; set; }

		public Project ProjectModel { get; set; }

		public List<ProjectFileActivity> ProjectFileActivities { get; set; }

		public Enumerators.Action Action { get; set; }

		public string FileType { get; set; }

		public bool Selected
		{
			get => _selected;
			set
			{
				if (_selected == value)
				{
					return;
				}

				_selected = value;
				OnPropertyChanged(nameof(Selected));
			}
		}

		public Enumerators.Status Status
		{
			get => _status;
			set
			{
				if (_status == value)
				{
					return;
				}

				_status = value;
				OnPropertyChanged(nameof(Status));
			}
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path
		{
			get => string.IsNullOrEmpty(_path) ? "\\" : _path;
			set
			{
				if (_path == value)
				{
					return;
				}

				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public string Location { get; set; }

		public string XliffFilePath { get; set; }

		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged(nameof(Date));
				OnPropertyChanged(nameof(DateToString));
			}
		}

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

		public LanguageInfo TargetLanguage { get; set; }

		public string ShortMessage { get; set; }

		public string Details { get; set; }

		public void Dispose()
		{
		}

		public object Clone()
		{
			var projectFile = new ProjectFile
			{
				ProjectId = ProjectId,
				Id = Id.Clone() as string,
				Name = Name.Clone() as string,
				Action = Action,
				Date = Date,
				Status = Status,
				Path = Path.Clone() as string,
				Selected = Selected,
				TargetLanguage = TargetLanguage.Clone() as LanguageInfo,
				XliffFilePath = XliffFilePath.Clone() as string,
				Details = Details?.Clone() as string,
				FileType = FileType.Clone() as string
			};

			foreach (var projectFileActivity in ProjectFileActivities)
			{
				projectFile.ProjectFileActivities.Add(projectFileActivity.Clone() as ProjectFileActivity);
			}

			return projectFile;
		}
	}
}
