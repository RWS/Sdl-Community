using System;
using System.Collections.Generic;
using CustomViewExample.Model;
using Sdl.Core.Globalization;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace CustomViewExample.Services
{
	public class ProjectsService
	{
		private readonly ProjectsController _projectsController;
		private readonly ImageService _imageService;

		public ProjectsService(ProjectsController projectsController, ImageService imageService)
		{
			_projectsController = projectsController;
			_imageService = imageService;
		}

		public List<CustomViewProject> GetProjects()
		{
			var projectModels = new List<CustomViewProject>();
			foreach (var project in _projectsController.GetAllProjects())
			{
				var projectInfo = project.GetProjectInfo();

				var projectModel = new CustomViewProject
				{
					Id = projectInfo.Id.ToString(),
					Name = projectInfo.Name,
					Description = projectInfo.Description,
					Created = projectInfo.CreatedAt,
					DueDate = projectInfo.DueDate ?? DateTime.MaxValue,
					ProjectType = projectInfo.ProjectType.ToString(),
					ProjectOrigin = projectInfo.ProjectOrigin,
					Path = projectInfo.LocalProjectFolder,
					SourceLanguage = GetLanguage(projectInfo.SourceLanguage),
					TargetLanguages = GetLanguages(projectInfo.TargetLanguages)
				};

				projectModels.Add(projectModel);
			}

			return projectModels;
		}

		private List<CustomViewLanguage> GetLanguages(IEnumerable<Language> languages)
		{
			var targetLanguages = new List<CustomViewLanguage>();
			foreach (var language in languages)
			{
				targetLanguages.Add(GetLanguage(language));
			}

			return targetLanguages;
		}

		private CustomViewLanguage GetLanguage(Language language)
		{
			return new CustomViewLanguage
			{
				CultureInfo = language.CultureInfo,
				Image = _imageService.GetImage(language.CultureInfo.Name)
			};
		}
	}
}
