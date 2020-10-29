using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.Model;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using AnalysisBand = Sdl.Community.Transcreate.Model.AnalysisBand;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;

namespace Sdl.Community.Transcreate.Service
{
	public class ProjectAutomationService
	{
		private string _projectNameSuffix;
		private readonly ImageService _imageService;
		private readonly TranscreateViewController _controller;
		private readonly CustomerProvider _customerProvider;

		public ProjectAutomationService(ImageService imageService, TranscreateViewController controller,
			CustomerProvider customerProvider)
		{
			_imageService = imageService;
			_controller = controller;
			_customerProvider = customerProvider;
		}

		public FileBasedProject CreateTranscreateProject(FileBasedProject project, string iconPath,
			List<ProjectFile> projectFiles, string projectNameSuffix)
		{
			if (string.IsNullOrEmpty(projectNameSuffix))
			{
				throw new Exception("The project name suffix cannot be null!");
			}

			_projectNameSuffix = projectNameSuffix;

			var projectInfo = project.GetProjectInfo();

			var projectReference = new ProjectReference(project.FilePath);
			var newProjectInfo = new ProjectInfo
			{
				Name = projectInfo.Name + "-" + _projectNameSuffix,
				Description = projectInfo.Description,
				LocalProjectFolder = projectInfo.LocalProjectFolder + "-" + _projectNameSuffix,
				SourceLanguage = projectInfo.SourceLanguage,
				TargetLanguages = projectInfo.TargetLanguages,
				DueDate = projectInfo.DueDate,
				ProjectOrigin = "Transcreate Project",
				IconPath = iconPath,
			};

			var newProject = new FileBasedProject(newProjectInfo, projectReference);
			foreach (var contextProjectFile in projectFiles)
			{
				if (!string.IsNullOrEmpty(contextProjectFile.ExternalFilePath) &&
					File.Exists(contextProjectFile.ExternalFilePath))
				{
					newProject.AddFiles(new[] { contextProjectFile.ExternalFilePath }, contextProjectFile.Path);
				}
			}

			var sourceLanguageFiles = newProject.GetSourceLanguageFiles();
			var scanResult = newProject.RunAutomaticTask(
				sourceLanguageFiles.GetIds(),
				AutomaticTaskTemplateIds.Scan
			);

			sourceLanguageFiles = newProject.GetSourceLanguageFiles();
			foreach (var projectFile in sourceLanguageFiles)
			{
				if (projectFile.Role == FileRole.Translatable)
				{
					Guid[] currentFileId = { projectFile.Id };
					var convertTask = newProject.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.ConvertToTranslatableFormat
					);

					var copyTask = newProject.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.CopyToTargetLanguages
					);
				}
			}

			var targetGuids = new List<Guid>();
			var targetLanguageFiles = newProject.GetTargetLanguageFiles();
			foreach (var projectFile in targetLanguageFiles)
			{
				if (projectFile.Role == FileRole.Translatable)
				{
					targetGuids.Add(projectFile.Id);
				}
			}

			var pretranslate = newProject.RunAutomaticTask(
				targetGuids.ToArray(),
				AutomaticTaskTemplateIds.PreTranslateFiles
			);

			var analyTask = newProject.RunAutomaticTask(
				targetGuids.ToArray(),
				AutomaticTaskTemplateIds.AnalyzeFiles
			);

			newProject.Save();
			return newProject;
		}

		public FileBasedProject CreateBackTranslationProject(FileBasedProject project, string targetLanguage, string iconPath,
			List<string> projectFiles, string projectNameSuffix)
		{
			if (string.IsNullOrEmpty(projectNameSuffix))
			{
				throw new Exception("The project name suffix cannot be null!");
			}

			_projectNameSuffix = projectNameSuffix;

			var projectInfo = project.GetProjectInfo();
			
			var localProjectFolder = Path.Combine(projectInfo.LocalProjectFolder, "BackProjects", targetLanguage);

			var newSourceLanguage = projectInfo.TargetLanguages.FirstOrDefault(a =>
				string.Compare(a.CultureInfo.Name, targetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0);
			var newTargetLanguage = projectInfo.SourceLanguage;
			if (Directory.Exists(localProjectFolder))
			{
				Directory.Delete(localProjectFolder, true);
			}

			var projectReference = new ProjectReference(project.FilePath);
			var newProjectInfo = new ProjectInfo
			{
				Name = projectInfo.Name + "-" + _projectNameSuffix + "-" + targetLanguage,
				Description = projectInfo.Description,
				LocalProjectFolder = localProjectFolder,
				SourceLanguage = newSourceLanguage,
				TargetLanguages = new Language[] { newTargetLanguage },
				DueDate = projectInfo.DueDate,
				ProjectOrigin = "Back-Translation Project",
				IconPath = iconPath,
			};

			var newProject = new FileBasedProject(newProjectInfo, projectReference);
			foreach (var contextProjectFile in projectFiles)
			{
				if (!string.IsNullOrEmpty(contextProjectFile) &&
					File.Exists(contextProjectFile))
				{
					newProject.AddFiles(new[] { contextProjectFile }, string.Empty);
				}
			}

			var sourceLanguageFiles = newProject.GetSourceLanguageFiles();
			var scanResult = newProject.RunAutomaticTask(
				sourceLanguageFiles.GetIds(),
				AutomaticTaskTemplateIds.Scan
			);

			sourceLanguageFiles = newProject.GetSourceLanguageFiles();
			foreach (var projectFile in sourceLanguageFiles)
			{
				if (projectFile.Role == FileRole.Translatable)
				{
					Guid[] currentFileId = { projectFile.Id };
					var convertTask = newProject.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.ConvertToTranslatableFormat
					);

					var copyTask = newProject.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.CopyToTargetLanguages
					);
				}
			}

			var targetGuids = new List<Guid>();
			var targetLanguageFiles = newProject.GetTargetLanguageFiles();
			foreach (var projectFile in targetLanguageFiles)
			{
				if (projectFile.Role == FileRole.Translatable)
				{
					targetGuids.Add(projectFile.Id);
				}
			}

			var pretranslate = newProject.RunAutomaticTask(
				targetGuids.ToArray(),
				AutomaticTaskTemplateIds.PreTranslateFiles
			);

			var analyTask = newProject.RunAutomaticTask(
				targetGuids.ToArray(),
				AutomaticTaskTemplateIds.AnalyzeFiles
			);

			newProject.Save();
			return newProject;
		}

		public Interfaces.IProject GetProject(FileBasedProject selectedProject, IReadOnlyCollection<string> selectedFileIds, List<ProjectFile> projectFiles = null)
		{
			if (selectedProject == null)
			{
				return null;
			}

			var projectInfo = selectedProject.GetProjectInfo();

			Interfaces.IProject project = projectInfo.ProjectOrigin == "Back-Translation Project"
				? new BackTranslationProject()
				: new Project();


			project.Id = projectInfo.Id.ToString();
			project.Name = projectInfo.Name;
			project.Customer = _customerProvider.GetProjectCustomer(selectedProject);
			project.Created = projectInfo.CreatedAt.ToUniversalTime();
			project.DueDate = projectInfo.DueDate?.ToUniversalTime() ?? DateTime.MaxValue;
			project.Path = projectInfo.LocalProjectFolder;
			project.SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo);
			project.TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages);
			project.ProjectType = GetProjectType(selectedProject);

			var existingProject = projectInfo.ProjectOrigin == "Back-Translation Project"
				? GetBackTranslationProjectProject(projectInfo.Id.ToString(), out _)
				: _controller.GetProjects().FirstOrDefault(a => a.Id == projectInfo.Id.ToString());
			if (existingProject != null)
			{
				foreach (var projectFile in existingProject.ProjectFiles)
				{
					if (projectFile.Clone() is ProjectFile clonedProjectFile)
					{
						clonedProjectFile.Project = project;
						clonedProjectFile.Location = GeFullPath(project.Path, clonedProjectFile.Location);
						clonedProjectFile.Report = GeFullPath(project.Path, clonedProjectFile.Report);
						clonedProjectFile.ExternalFilePath = GeFullPath(project.Path, clonedProjectFile.ExternalFilePath);
						clonedProjectFile.Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.FileId.ToString());
						project.ProjectFiles.Add(clonedProjectFile);
					}
				}
			}
			else
			{
				project.ProjectFiles = GetProjectFiles(selectedProject, project, selectedFileIds);
			}

			AssignProjectFileXliffData(project, projectFiles);

			return project;
		}

		public Interfaces.IProject GetBackTranslationProjectProject(string projectId, out Interfaces.IProject parentProject)
		{
			parentProject = null;
			var projects = _controller.GetProjects();
			foreach (var project in projects)
			{
				var backTranslationProject =
					project.BackTranslationProjects.FirstOrDefault(a => a.Id == projectId);
				if (backTranslationProject != null)
				{
					parentProject = project;
					return backTranslationProject;
				}
			}

			return null;
		}

		public List<AnalysisBand> GetAnalysisBands(FileBasedProject project)
		{
			var regex = new Regex(@"(?<min>[\d]*)([^\d]*)(?<max>[\d]*)", RegexOptions.IgnoreCase);

			var analysisBands = new List<AnalysisBand>();
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				foreach (var analysisBand in internalDynamicProject.AnalysisBands)
				{
					Match match = regex.Match(analysisBand.ToString());
					if (match.Success)
					{
						var min = match.Groups["min"].Value;
						var max = match.Groups["max"].Value;
						analysisBands.Add(new AnalysisBand
						{
							MinimumMatchValue = Convert.ToInt32(min),
							MaximumMatchValue = Convert.ToInt32(max)
						});
					}
				}
			}

			return analysisBands;
		}

		private string GeFullPath(string projectPath, string path)
		{
			if (string.IsNullOrEmpty(path?.Trim('\\')))
			{
				return string.Empty;
			}

			return Path.Combine(projectPath.Trim('\\'), path.Trim('\\'));
		}

		private LanguageInfo GetLanguageInfo(CultureInfo cultureInfo)
		{
			var languageInfo = new LanguageInfo
			{
				CultureInfo = cultureInfo,
				Image = _imageService.GetImage(cultureInfo.Name)
			};

			return languageInfo;
		}

		private List<LanguageInfo> GetLanguageInfos(IEnumerable<Language> languages)
		{
			var targetLanguages = new List<LanguageInfo>();
			foreach (var targetLanguage in languages)
			{
				targetLanguages.Add(GetLanguageInfo(targetLanguage.CultureInfo));
			}

			return targetLanguages;
		}

		private string GetProjectType(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				return internalDynamicProject.ProjectType.ToString();
			}

			return null;
		}

		private void AssignProjectFileXliffData(Interfaces.IProject project, IEnumerable<ProjectFile> projectfiles)
		{
			if (projectfiles == null)
			{
				return;
			}

			foreach (var projectFile in projectfiles)
			{
				if (projectFile.XliffData == null)
				{
					continue;
				}

				var projectFileName = projectFile.Name.Substring(0, projectFile.Name.Length - ".sdlxliff".Length) + ".xliff.sdlxliff";

				var targetFile = project.ProjectFiles?.FirstOrDefault(a =>
					string.Compare(a.Name, projectFileName, StringComparison.CurrentCultureIgnoreCase) == 0
					&& string.Compare(a.Path, projectFile.Path, StringComparison.CurrentCultureIgnoreCase) == 0
					&& string.Compare(a.TargetLanguage, projectFile.TargetLanguage, StringComparison.CurrentCultureIgnoreCase) == 0);

				if (targetFile != null)
				{
					targetFile.XliffData = projectFile.XliffData;
					targetFile.ExternalFilePath = projectFile.ExternalFilePath;
					targetFile.ConfirmationStatistics = projectFile.ConfirmationStatistics;
					targetFile.TranslationOriginStatistics = projectFile.TranslationOriginStatistics;
					targetFile.Selected = true;
				}
			}
		}

		private ProjectFile GetProjectFile(Interfaces.IProject project, ProjectAutomation.Core.ProjectFile projectFile,
			IReadOnlyCollection<string> selectedFileIds)
		{
			var projectFileModel = new ProjectFile
			{
				ProjectId = project.Id,
				FileId = projectFile.Id.ToString(),
				Name = projectFile.Name,
				Path = projectFile.Folder,
				Location = projectFile.LocalFilePath,
				Action = Enumerators.Action.None,
				WorkFlow = Enumerators.WorkFlow.None,
				Status = Enumerators.Status.Ready,
				Date = DateTime.MinValue,
				TargetLanguage = projectFile.Language.CultureInfo.Name,
				Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.Id.ToString()),
				FileType = projectFile.FileTypeId,
				Project = project
			};

			return projectFileModel;
		}

		private List<ProjectFile> GetProjectFiles(IProject project, Interfaces.IProject projectModel, IReadOnlyCollection<string> selectedFileIds)
		{
			var projectInfo = project.GetProjectInfo();
			var projectFiles = new List<ProjectFile>();


			var sourceLanguageFiles = project.GetSourceLanguageFiles();
			foreach (var projectFile in sourceLanguageFiles)
			{
				if (projectFile.Role != FileRole.Translatable)
				{
					continue;
				}

				var projectFileModel = GetProjectFile(projectModel, projectFile, null);
				projectFiles.Add(projectFileModel);
			}

			foreach (var targetLanguage in projectInfo.TargetLanguages)
			{
				var languageFiles = project.GetTargetLanguageFiles(targetLanguage);
				foreach (var projectFile in languageFiles)
				{
					if (projectFile.Role != FileRole.Translatable)
					{
						continue;
					}

					var projectFileModel = GetProjectFile(projectModel, projectFile, selectedFileIds);
					projectFiles.Add(projectFileModel);
				}
			}

			return projectFiles;
		}
	}
}
