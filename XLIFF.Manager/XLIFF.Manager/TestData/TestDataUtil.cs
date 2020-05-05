using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.TestData
{
	public class TestDataUtil
	{
		public List<ProjectModel> GetTestProjectData()
		{			
			var projectModels = new List<ProjectModel>();
			projectModels.Add(GetProject("Avengers", "Project 0", new CultureInfo("en-US"), new List<CultureInfo> { new CultureInfo("it-IT"), new CultureInfo("de-DE") }));
			projectModels.Add(GetProject("Avengers", "Project 66", new CultureInfo("en-US"), new List<CultureInfo> { new CultureInfo("it-IT"), new CultureInfo("de-DE"), new CultureInfo("fr-FR") }));
			projectModels.Add(GetProject("Travel Guides", "Project 2", new CultureInfo("en-US"), new List<CultureInfo> { new CultureInfo("de-DE") }));

			return projectModels;
		}

		private ProjectModel GetProject(string clientName, string projectName, CultureInfo sourceLanguage, List<CultureInfo> targetLanguages)
		{
			var projectModel = new ProjectModel
			{
				ClientName = clientName,
				Created = DateTime.Now.Subtract(new TimeSpan(10, 0, 0, 0, 0)),
				DueDate = DateTime.Now.AddDays(10),
				Id = Guid.NewGuid().ToString(),
				Name = projectName,
				Path = projectName,
				ProjectType = "SDL Project",
				SourceLanguage = sourceLanguage,
				TargetLanguages = targetLanguages,
				ProjectFileActionModels = new List<ProjectFileActionModel>()
			};

			foreach (var targetLanguage in targetLanguages)
			{
				projectModel.ProjectFileActionModels.Add(GetProjectFileAction(projectModel, targetLanguage,
					Enumerators.Action.Export, DateTime.Now.Subtract(new TimeSpan(5, 0, 0, 0, 0))));
				projectModel.ProjectFileActionModels.Add(GetProjectFileAction(projectModel, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
				projectModel.ProjectFileActionModels.Add(GetProjectFileAction(projectModel, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
				projectModel.ProjectFileActionModels.Add(GetProjectFileAction(projectModel, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
				projectModel.ProjectFileActionModels.Add(GetProjectFileAction(projectModel, targetLanguage,
					Enumerators.Action.Import, DateTime.Now));
			}

			return projectModel;
		}

		private ProjectFileActionModel GetProjectFileAction(ProjectModel projectModel, CultureInfo targetLanguage, Enumerators.Action action, DateTime dateTime)
		{
			var projectFileActionModel = new ProjectFileActionModel(projectModel)
			{
				Action = action,
				Date = dateTime,
				Id = Guid.NewGuid().ToString(),
				Name = projectModel.Name + ">File " + projectModel.ProjectFileActionModels.Count,
				Path = "\\File Path\\" + projectModel.ProjectFileActionModels.Count,
				TargetLanguage = targetLanguage
			};

			if (action == Enumerators.Action.Export)
			{
				projectFileActionModel.ProjectFileActivityModels.Add(
					GetProjectFileActivity(projectFileActionModel, Enumerators.Action.Export, Enumerators.Status.Success,
						projectFileActionModel.Date));
			}

			if (action == Enumerators.Action.Import)
			{
				projectFileActionModel.ProjectFileActivityModels.Add(
					GetProjectFileActivity(projectFileActionModel, Enumerators.Action.Export, Enumerators.Status.Success,
						projectFileActionModel.Date.Subtract(new TimeSpan(1, 0, 0, 0, 0))));

				projectFileActionModel.ProjectFileActivityModels.Add(
					GetProjectFileActivity(projectFileActionModel, Enumerators.Action.Import, Enumerators.Status.Error,
						projectFileActionModel.Date.Subtract(new TimeSpan(0, 0, 2, 0, 0))));

				projectFileActionModel.ProjectFileActivityModels.Add(
					GetProjectFileActivity(projectFileActionModel, Enumerators.Action.Import, Enumerators.Status.Success,
						projectFileActionModel.Date));
			}

			return projectFileActionModel;
		}

		private ProjectFileActivityModel GetProjectFileActivity(ProjectFileActionModel projectFileActionModel,
			Enumerators.Action action, Enumerators.Status status, DateTime dateTime)
		{
			var projectFileActivityModel = new ProjectFileActivityModel(projectFileActionModel)
			{
				Action = action,
				Status = status,
				Id = Guid.NewGuid().ToString(),
				Name = projectFileActionModel.Name + ">Activity File " + projectFileActionModel.ProjectFileActivityModels.Count,
				Path = "\\File Path\\" + projectFileActionModel.ProjectFileActivityModels.Count,
				Date = dateTime != DateTime.MinValue ? dateTime : GetRamdomDate(projectFileActionModel.ProjectModel.Created),
				Message = status.ToString()
			};

			return projectFileActivityModel;
		}

		private static DateTime GetRamdomDate(DateTime start)
		{
			var gen = new Random();
			var range = (DateTime.Today - start).Days;
			return start.AddDays(gen.Next(range));
		}
	}
}
