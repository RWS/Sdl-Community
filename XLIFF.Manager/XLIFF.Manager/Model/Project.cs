using System;
using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class Project : BaseModel, ICloneable
	{
		public Project()
		{
			TargetLanguages = new List<LanguageInfo>();
			ProjectFiles = new List<ProjectFile>();
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public Customer Customer { get; set; }

		public string AbsoluteUri { get; set; }

		public DateTime DueDate { get; set; }

		public DateTime Created { get; set; }

		public string ProjectType { get; set; }

		public LanguageInfo SourceLanguage { get; set; }

		public List<LanguageInfo> TargetLanguages { get; set; }

		public List<ProjectFile> ProjectFiles { get; set; }

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
			foreach (var projectFileModel in model.ProjectFiles)
			{
				model.ProjectFiles.Add(projectFileModel.Clone() as ProjectFile);
			}

			return model;
		}
	}
}
