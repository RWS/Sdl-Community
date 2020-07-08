using System;
using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class Project : BaseModel, ICloneable
	{
		private Customer _customer;
		private List<ProjectFile> _projectFiles;

		public Project()
		{
			TargetLanguages = new List<LanguageInfo>();
			ProjectFiles = new List<ProjectFile>();
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }	

		public Customer Customer
		{
			get
			{
				return _customer ?? (_customer = new Customer
				{
					Name = "[no client]"
				});
			}
			set
			{
				_customer = value;
				OnPropertyChanged(nameof(Customer));
			}
		}

		public string AbsoluteUri { get; set; }

		public DateTime DueDate { get; set; }

		public string DueDateToString => GetDateTimeToString(DueDate);

		public DateTime Created { get; set; }

		public string CreatedToString => GetDateTimeToString(Created);

		public string ProjectType { get; set; }

		public LanguageInfo SourceLanguage { get; set; }

		public List<LanguageInfo> TargetLanguages { get; set; }

		public List<ProjectFile> ProjectFiles
		{
			get => _projectFiles;
			set
			{
				_projectFiles = value;
				OnPropertyChanged(nameof(ProjectFiles));
			}
		}

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

		public object Clone()
		{
			var project = new Project
			{
				Id = Id,
				AbsoluteUri = AbsoluteUri,
				Name = Name,
				Customer = Customer?.Clone() as Customer,
				Created = new DateTime(Created.Ticks, DateTimeKind.Utc),
				DueDate = new DateTime(DueDate.Ticks, DateTimeKind.Utc),
				Path = Path,
				ProjectType = ProjectType,
				SourceLanguage = SourceLanguage.Clone() as LanguageInfo
			};

			foreach (var languageInfo in TargetLanguages)
			{
				project.TargetLanguages.Add(languageInfo.Clone() as LanguageInfo);
			}

			foreach (var projectFile in ProjectFiles)
			{
				if (projectFile.Clone() is ProjectFile projectFileCloned)
				{
					projectFileCloned.Project = project;
					project.ProjectFiles.Add(projectFileCloned);
				}
			}

			return project;
		}
	}
}
