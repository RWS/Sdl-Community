using System;
using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class Project : BaseModel, ICloneable
	{
		private Customer _customer;

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

		public List<ProjectFile> ProjectFiles { get; set; }

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
			var model = new Project
			{
				Id = Id,
				AbsoluteUri = AbsoluteUri,
				Name = Name,
				Created = Created,
				Customer = Customer?.Clone() as Customer,
				DueDate = DueDate,
				Path = Path,
				ProjectType = ProjectType,
				SourceLanguage = SourceLanguage.Clone() as LanguageInfo
			};

			foreach (var languageInfo in model.TargetLanguages)
			{
				model.TargetLanguages.Add(languageInfo.Clone() as LanguageInfo);
			}
			foreach (var projectFile in model.ProjectFiles)
			{
				model.ProjectFiles.Add(projectFile.Clone() as ProjectFile);
			}

			return model;
		}
	}
}
