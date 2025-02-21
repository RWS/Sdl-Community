using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Service;

namespace XLIFF.Manager.UnitTests.Common
{
	public class TestDataUtil
	{
		private readonly ImageService _imageService;

		public TestDataUtil(ImageService imageService)
		{
			_imageService = imageService;
		}

		public List<Project> GetDefaultTestProjectData()
		{
			var customer1 = new Customer
			{
				Name = "Avengers",
				Email = "Tror@gods.it",
				Id = Guid.NewGuid().ToString()
			};

			var customer2 = new Customer
			{
				Name = "Travel Guides",
				Email = "Travel@Guides.it",
				Id = Guid.NewGuid().ToString()
			};

			var projects = new List<Project>();
			projects.Add(GetProject(customer1, "Project 0", new CultureInfo("en-US"), new List<CultureInfo> { new CultureInfo("it-IT"), new CultureInfo("de-DE") }));
			projects.Add(GetProject(customer1, "Project 66", new CultureInfo("en-US"), new List<CultureInfo> { new CultureInfo("it-IT"), new CultureInfo("de-DE"), new CultureInfo("fr-FR") }));
			projects.Add(GetProject(customer2, "Project 2", new CultureInfo("en-US"), new List<CultureInfo> { new CultureInfo("de-DE") }));

			return projects;
		}

		private Project GetProject(Customer customer, string projectName, CultureInfo sourceLanguage, List<CultureInfo> targetLanguages)
		{
			var project = new Project
			{
				Customer = customer,
				Created = DateTime.Now.Subtract(new TimeSpan(10, 0, 0, 0, 0)),
				DueDate = DateTime.Now.AddDays(10),
				Id = Guid.NewGuid().ToString(),
				Name = projectName,
				Path = projectName,
				ProjectType = "RWS Project",
				ProjectFiles = new List<ProjectFile>()
			};

			var sourceLanguageInfo = new LanguageInfo
			{
				CultureInfo = sourceLanguage,
				Image = _imageService.GetImage(sourceLanguage.Name)
			};
			project.SourceLanguage = sourceLanguageInfo;

			project.TargetLanguages = new List<LanguageInfo>();
			foreach (var targetLanguage in targetLanguages)
			{
				var targetLanguageInfo = new LanguageInfo
				{
					CultureInfo = targetLanguage,
					Image = _imageService.GetImage(targetLanguage.Name)
				};
				project.TargetLanguages.Add(targetLanguageInfo);
			}

			foreach (var targetLanguage in project.TargetLanguages)
			{
				project.ProjectFiles.Add(GetProjectFileAction(project, targetLanguage,
					Enumerators.Action.Export, DateTime.Now.Subtract(new TimeSpan(5, 0, 0, 0, 0))));
				project.ProjectFiles.Add(GetProjectFileAction(project, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
				project.ProjectFiles.Add(GetProjectFileAction(project, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
				project.ProjectFiles.Add(GetProjectFileAction(project, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
				project.ProjectFiles.Add(GetProjectFileAction(project, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
			}

			return project;
		}

		private ProjectFile GetProjectFileAction(Project project, LanguageInfo targetLanguage, Enumerators.Action action, DateTime dateTime)
		{
			var projectFile = new ProjectFile
			{
				ProjectId = project.Id,
				Action = action,
				Date = dateTime,
				FileId = Guid.NewGuid().ToString(),
				Name = project.Name + ">File " + project.ProjectFiles.Count,
				Path = "\\Project File Path\\" + project.ProjectFiles.Count,
				TargetLanguage = targetLanguage.CultureInfo.Name,
				Project = project
			};

			if (action == Enumerators.Action.Export)
			{
				projectFile.ProjectFileActivities.Add(
					GetProjectFileActivity(projectFile, Enumerators.Action.Export, Enumerators.Status.Success,
						projectFile.Date));
			}

			if (action == Enumerators.Action.Import)
			{
				projectFile.ProjectFileActivities.Add(
					GetProjectFileActivity(projectFile, Enumerators.Action.Export, Enumerators.Status.Success,
						projectFile.Date.Subtract(new TimeSpan(1, 0, 0, 0, 0))));

				projectFile.ProjectFileActivities.Add(
					GetProjectFileActivity(projectFile, Enumerators.Action.Import, Enumerators.Status.Error,
						projectFile.Date.Subtract(new TimeSpan(0, 0, 2, 0, 0))));

				projectFile.ProjectFileActivities.Add(
					GetProjectFileActivity(projectFile, Enumerators.Action.Import, Enumerators.Status.Success,
						projectFile.Date));
			}

			projectFile.XliffFilePath = "\\XLIFF File\\" + (projectFile.ProjectFileActivities.Count - 1);

			return projectFile;
		}

		private ProjectFileActivity GetProjectFileActivity(ProjectFile projectFile,
			Enumerators.Action action, Enumerators.Status status, DateTime dateTime)
		{
			var projectFileActivity = new ProjectFileActivity
			{
				ProjectFileId = projectFile.FileId,
				Action = action,
				Status = status,
				ActivityId = Guid.NewGuid().ToString(),
				Name = projectFile.Name + ">XLIFF File " + projectFile.ProjectFileActivities.Count,
				Path = "\\XLIFF File Path\\" + projectFile.ProjectFileActivities.Count,
				Date = dateTime != DateTime.MinValue ? dateTime : GetRamdomDate(projectFile.Project.Created),
				Report = status.ToString(),
				ProjectFile = projectFile
			};

			return projectFileActivity;
		}

		private static DateTime GetRamdomDate(DateTime start)
		{
			var gen = new Random();
			var range = (DateTime.Today - start).Days;
			return start.AddDays(gen.Next(range));
		}
	}
}
