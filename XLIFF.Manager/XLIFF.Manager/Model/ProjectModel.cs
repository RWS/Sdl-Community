using System;
using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectModel : BaseModel, ICloneable
	{
		public ProjectModel()
		{
			TargetLanguages = new List<LanguageInfo>();
			ProjectFileModels = new List<ProjectFileModel>();
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public CustomerModel Customer { get; set; }

		public string AbsoluteUri { get; set; }

		public DateTime DueDate { get; set; }

		public DateTime Created { get; set; }

		public string ProjectType { get; set; }

		public LanguageInfo SourceLanguage { get; set; }

		public List<LanguageInfo> TargetLanguages { get; set; }

		public List<ProjectFileModel> ProjectFileModels { get; set; }
		public object Clone()
		{
			var model = new ProjectModel
			{
				Id = Id,
				AbsoluteUri = AbsoluteUri,
				Name = Name,
				Created = Created,
				Customer = Customer?.Clone() as CustomerModel,
				DueDate = DueDate,
				Path = Path,
				ProjectType = ProjectType,
				SourceLanguage = SourceLanguage.Clone() as LanguageInfo
			};

			foreach (var languageInfo in model.TargetLanguages)
			{
				model.TargetLanguages.Add(languageInfo.Clone() as LanguageInfo);
			}
			foreach (var projectFileModel in model.ProjectFileModels)
			{
				model.ProjectFileModels.Add(projectFileModel.Clone() as ProjectFileModel);
			}

			return model;
		}
	}
}
