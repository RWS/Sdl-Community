using System;
using System.Collections.Generic;
using Trados.Transcreate.Interfaces;

namespace Trados.Transcreate.Model
{
	public class Project : BaseModel, ICloneable, IProject
	{
		private Customer _customer;
		private List<ProjectFile> _projectFiles;
		private List<BackTranslationProject> _backTranslationProjects;
		private bool _isSelected;
		private bool _isExpanded;

		public Project()
		{
			TargetLanguages = new List<LanguageInfo>();
			ProjectFiles = new List<ProjectFile>();
			BackTranslationProjects = new List<BackTranslationProject>();
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected == value)
				{
					return;
				}

				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public bool IsExpanded
		{
			get => _isExpanded;
			set
			{
				if (_isExpanded == value)
				{
					return;
				}

				_isExpanded = value;
				OnPropertyChanged(nameof(IsExpanded));
			}
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

		public List<BackTranslationProject> BackTranslationProjects
		{
			get => _backTranslationProjects;
			set
			{
				_backTranslationProjects = value;
				OnPropertyChanged(nameof(BackTranslationProjects));
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
			var project = this is BackTranslationProject ? new BackTranslationProject() : new Project();

			project.Id = Id;
			project.Name = Name;
			project.Customer = Customer?.Clone() as Customer;
			project.Created = new DateTime(Created.Ticks, DateTimeKind.Utc);
			project.DueDate = new DateTime(DueDate.Ticks, DateTimeKind.Utc);
			project.Path = Path;
			project.ProjectType = ProjectType;
			project.SourceLanguage = SourceLanguage.Clone() as LanguageInfo;

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

			foreach (var backTranslationProject in BackTranslationProjects)
			{
				project.BackTranslationProjects.Add(backTranslationProject.Clone() as BackTranslationProject);
			}

			return project;
		}
	}
}
