using System;
using System.Collections.Generic;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.Interfaces
{
	public interface IProject
	{
		bool IsSelected { get; set; }

		bool IsExpanded { get; set; }

		string Id { get; set; }

		string Name { get; set; }

		string Path { get; set; }

		Customer Customer { get; set; }

		DateTime DueDate { get; set; }

		string DueDateToString { get; }

		DateTime Created { get; set; }

		string CreatedToString { get; }

		string ProjectType { get; set; }

		LanguageInfo SourceLanguage { get; set; }

		List<LanguageInfo> TargetLanguages { get; set; }

		List<ProjectFile> ProjectFiles { get; set; }

		List<BackTranslationProject> BackTranslationProjects { get; set; }
	}
}
