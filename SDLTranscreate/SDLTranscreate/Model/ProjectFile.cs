using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Interfaces;

namespace Trados.Transcreate.Model
{
	public class ProjectFile : BaseModel, IDisposable, ICloneable
	{
		private DateTime _date;
		private bool _selected;
		private string _path;
		private string _externalFilePath;
		private string _targetLanguage;
		private string _report;
		private Enumerators.Status _status;

		public ProjectFile()
		{
			Status = Enumerators.Status.None;
			Action = Enumerators.Action.None;
			WorkFlow = Enumerators.WorkFlow.None;
			ProjectFileActivities = new List<ProjectFileActivity>();
		}

		public string ProjectId { get; set; }

		[XmlIgnore]
		public IProject Project { get; set; }

		[XmlIgnore]
		public Xliff XliffData { get; set; }

		public ConfirmationStatistics ConfirmationStatistics { get; set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; set; }

		public List<ProjectFileActivity> ProjectFileActivities { get; set; }

		public Enumerators.Action Action { get; set; }

		public Enumerators.WorkFlow WorkFlow { get; set; }

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

		public string FileId { get; set; }

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

		public string ExternalFilePath
		{
			get => _externalFilePath;
			set
			{
				if (value == _externalFilePath)
				{
					return;
				}

				_externalFilePath = value;
				OnPropertyChanged(nameof(ExternalFilePath));
			}
		}

		public DateTime Date
		{
			get => _date;
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

		public string TargetLanguage
		{
			get => _targetLanguage;
			set
			{
				if (_targetLanguage == value)
				{
					return;
				}

				_targetLanguage = value;
				OnPropertyChanged(nameof(TargetLanguage));
				OnPropertyChanged(nameof(LanguageDirection));
			}
		}

		public string LanguageDirection => Project?.SourceLanguage?.CultureInfo?.Name + "," + TargetLanguage;

		public string ShortMessage { get; set; }

		public string Report
		{
			get => _report;
			set
			{
				if (_report == value)
				{
					return;
				}

				_report = value;
				OnPropertyChanged(nameof(Report));
			}
		}

		public void Dispose()
		{
		}

		public object Clone()
		{
			var projectFile = new ProjectFile
			{
				ProjectId = ProjectId,
				FileId = FileId,
				Name = Name,
				Action = Action,
				WorkFlow = WorkFlow,
				Status = Status,
				Date = new DateTime(Date.Ticks, DateTimeKind.Utc),
				Location = Location,
				Path = Path,
				Selected = Selected,
				TargetLanguage = TargetLanguage,
				ExternalFilePath = ExternalFilePath,
				Report = Report,
				FileType = FileType
			};

			if (ConfirmationStatistics != null)
			{
				projectFile.ConfirmationStatistics = ConfirmationStatistics.Clone() as ConfirmationStatistics;
			}

			if (TranslationOriginStatistics != null)
			{
				projectFile.TranslationOriginStatistics = TranslationOriginStatistics.Clone() as TranslationOriginStatistics;
			}

			foreach (var projectFileActivity in ProjectFileActivities)
			{
				if (projectFileActivity.Clone() is ProjectFileActivity projectFileActivityCloned)
				{
					projectFile.ProjectFileActivities.Add(projectFileActivityCloned);
				}
			}

			return projectFile;
		}
	}
}
