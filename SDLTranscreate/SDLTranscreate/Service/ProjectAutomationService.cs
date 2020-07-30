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
		private string _projectNameSuffix = "-Transcreate";
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

		public FileBasedProject CreateTranscreateProject(FileBasedProject project,
			List<ProjectFile> projectFiles, string projectNameSuffix = null)
		{
			if (!string.IsNullOrEmpty(projectNameSuffix))
			{
				_projectNameSuffix = projectNameSuffix;
			}

			var projectInfo = project.GetProjectInfo();

			var projectReference = new ProjectReference(project.FilePath);
			var newProjectInfo = new ProjectInfo
			{
				Name = projectInfo.Name + _projectNameSuffix,
				Description = projectInfo.Description,
				LocalProjectFolder = projectInfo.LocalProjectFolder + _projectNameSuffix,
				SourceLanguage = projectInfo.SourceLanguage,
				TargetLanguages = projectInfo.TargetLanguages,
				DueDate = projectInfo.DueDate
			};

			var newProject = new FileBasedProject(newProjectInfo, projectReference);
			foreach (var contextProjectFile in projectFiles)
			{
				if (!string.IsNullOrEmpty(contextProjectFile.XliffFilePath) &&
					File.Exists(contextProjectFile.XliffFilePath))
				{
					newProject.AddFiles(new[] {contextProjectFile.XliffFilePath}, contextProjectFile.Path);
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

			newProject.Save();
			return newProject;
		}

		public Project GetProject(FileBasedProject selectedProject, 
			IReadOnlyCollection<string> selectedFileIds, List<ProjectFile> projectFiles = null)
		{
			if (selectedProject == null)
			{
				return null;
			}

			var projectInfo = selectedProject.GetProjectInfo();

			var project = new Project
			{
				Id = projectInfo.Id.ToString(),
				Name = projectInfo.Name,
				AbsoluteUri = projectInfo.Uri.AbsoluteUri,
				Customer = _customerProvider.GetProjectCustomer(selectedProject),
				Created = projectInfo.CreatedAt.ToUniversalTime(),
				DueDate = projectInfo.DueDate?.ToUniversalTime() ?? DateTime.MaxValue,
				Path = projectInfo.LocalProjectFolder,
				SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
				TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
				ProjectType = GetProjectType(selectedProject)
			};

			var existingProject = _controller.GetProjects().FirstOrDefault(a => a.Id == projectInfo.Id.ToString());
			if (existingProject != null)
			{
				foreach (var projectFile in existingProject.ProjectFiles)
				{
					if (projectFile.Clone() is ProjectFile clonedProjectFile)
					{
						clonedProjectFile.Project = project;
						clonedProjectFile.Location = GeFullPath(project.Path, clonedProjectFile.Location);
						clonedProjectFile.Report = GeFullPath(project.Path, clonedProjectFile.Report);
						clonedProjectFile.XliffFilePath = GeFullPath(project.Path, clonedProjectFile.XliffFilePath);
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

		private void AssignProjectFileXliffData(Project project, IEnumerable<ProjectFile> projectfiles)
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
					targetFile.XliffFilePath = projectFile.XliffFilePath;
					targetFile.ConfirmationStatistics = projectFile.ConfirmationStatistics;
					targetFile.TranslationOriginStatistics = projectFile.TranslationOriginStatistics;
					targetFile.Selected = true;
				}
			}
		}

		private ProjectFile GetProjectFile(Project project, ProjectAutomation.Core.ProjectFile projectFile,
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
				Status = Enumerators.Status.Ready,
				Date = DateTime.MinValue,
				TargetLanguage = projectFile.Language.CultureInfo.Name,
				Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.Id.ToString()),
				FileType = projectFile.FileTypeId,
				Project = project
			};

			return projectFileModel;
		}

		private List<ProjectFile> GetProjectFiles(IProject project, Project projectModel, IReadOnlyCollection<string> selectedFileIds)
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
