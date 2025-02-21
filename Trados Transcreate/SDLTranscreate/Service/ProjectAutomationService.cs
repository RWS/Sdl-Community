using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using Reports.Viewer.Api.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.Model;
using Trados.Transcreate.Model.ProjectSettings;
using AnalysisBand = Trados.Transcreate.Model.AnalysisBand;
using ProjectFile = Trados.Transcreate.Model.ProjectFile;

namespace Trados.Transcreate.Service
{
	public class ProjectAutomationService
	{
		private readonly ImageService _imageService;
		private readonly TranscreateViewController _controller;
		private readonly ProjectsController _projectsController;
		private readonly CustomerProvider _customerProvider;
		private readonly StudioVersionService _studioVersionService;

		public ProjectAutomationService(ImageService imageService, TranscreateViewController controller, ProjectsController projectsController,
			CustomerProvider customerProvider, StudioVersionService studioVersionService)
		{
			_imageService = imageService;
			_controller = controller;
			_projectsController = projectsController;
			_customerProvider = customerProvider;
			_studioVersionService = studioVersionService;
		}

		public void ActivateProject(FileBasedProject project)
		{
			if (project == null)
			{
				return;
			}

			var projectId = project.GetProjectInfo().Id.ToString();
			var selectedProjectId = _projectsController.CurrentProject?.GetProjectInfo().Id.ToString();
			if (projectId != selectedProjectId)
			{
				if (CanActivateFileBasedProject())
				{
					var activateProjectMethod = _projectsController.GetType().GetMethod("ActivateProject",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					activateProjectMethod?.Invoke(_projectsController, new object[] { project });
				}
				else
				{
					var internalProjectType = typeof(FileBasedProject).GetProperty("InternalProject",
						BindingFlags.NonPublic | BindingFlags.Instance);
					var projectInstance = internalProjectType?.GetValue(project);

					var activateProjectMethod = _projectsController.GetType().GetMethod("ActivateProject",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					activateProjectMethod?.Invoke(_projectsController, new[] { projectInstance });
				}
			}

			Dispatcher.CurrentDispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		/// <summary>
		/// Work around to ensure the project files have a segment pair container (e.g. segmented)
		/// </summary>
		/// <param name="project"></param>
		public async Task<bool> RunPretranslationWithoutTm(FileBasedProject project)
		{
			var success = true;
			try
			{
				var projectInfo = project.GetProjectInfo();
				var translationProviderConfigurationClone = project.GetTranslationProviderConfiguration();
				var translationProviderConfiguration = project.GetTranslationProviderConfiguration();

				var languageTranslationProviderConfigurations =
					new Dictionary<Language, TranslationProviderConfiguration>();
				foreach (var language in projectInfo.TargetLanguages)
				{
					var languageTranslationProviderConfigurationClone =
						project.GetTranslationProviderConfiguration(language);
					languageTranslationProviderConfigurations.Add(language,
						languageTranslationProviderConfigurationClone);

					var languageTranslationProviderConfiguration =
						project.GetTranslationProviderConfiguration(language);
					languageTranslationProviderConfiguration.Entries = new List<TranslationProviderCascadeEntry>();
					project.UpdateTranslationProviderConfiguration(language, languageTranslationProviderConfiguration);
				}

				// temporarily remove tm resources, prior to performing a pretranslation task
				translationProviderConfiguration.Entries = new List<TranslationProviderCascadeEntry>();
				project.UpdateTranslationProviderConfiguration(translationProviderConfiguration);

				foreach (var language in project.GetProjectInfo().TargetLanguages)
				{
					var targetGuids = GetProjectFileGuids(project.GetTargetLanguageFiles(language));
					project.RunAutomaticTask(
						targetGuids.ToArray(),
						AutomaticTaskTemplateIds.PreTranslateFiles
					);
				}

				// add back the tm resources
				project.UpdateTranslationProviderConfiguration(translationProviderConfigurationClone);
				foreach (var language in projectInfo.TargetLanguages)
				{
					project.UpdateTranslationProviderConfiguration(language,
						languageTranslationProviderConfigurations[language]);
				}

				project.UpdateProject(projectInfo);
				project.Save();
			}
			catch
			{
				success = false;
			}

			return await System.Threading.Tasks.Task.FromResult(success);
		}

		public void RemoveLastReportOfType(string groupType)
		{
			var task = System.Threading.Tasks.Task.Run(async () => await _controller.ReportsController.GetReports(true));
			System.Threading.Tasks.Task.WaitAll(task);

			var report = task.Result?.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Group == groupType);
			if (report != null)
			{
				_controller.ReportsController.RemoveReports(new List<Guid> { report.Id });
			}
		}

		public void RemoveAllReports()
		{
			var task = System.Threading.Tasks.Task.Run(async () => await _controller.ReportsController.GetReports(true));
			System.Threading.Tasks.Task.WaitAll(task);

			if (task.Result != null)
			{
				_controller.ReportsController.RemoveReports(task.Result.Select(a => a.Id).ToList());
			}
		}

		public FileBasedProject CreateTranscreateProject(FileBasedProject project, string iconPath,
			List<ProjectFile> projectFiles, string projectNameSuffix)
		{
			if (string.IsNullOrEmpty(projectNameSuffix))
			{
				throw new Exception(PluginResources.Warning_Message_ProjectNameSuffixCannotBeNull);
			}

			var projectInfo = project.GetProjectInfo();
			var projectReference = new ProjectReference(project.FilePath);

			var newProjectInfo = new ProjectInfo
			{
				Name = projectInfo.Name + "-" + projectNameSuffix,
				Description = projectInfo.Description,
				LocalProjectFolder = projectInfo.LocalProjectFolder + "-" + projectNameSuffix,
				SourceLanguage = projectInfo.SourceLanguage,
				TargetLanguages = projectInfo.TargetLanguages,
				DueDate = projectInfo.DueDate,
				ProjectOrigin = Constants.ProjectOrigin_TranscreateProject,
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
			newProject.Save();

			newProject.RunAutomaticTask(
			   newProject.GetSourceLanguageFiles().GetIds(),
			   AutomaticTaskTemplateIds.Scan
		   );

			var sourceGuids = GetProjectFileGuids(newProject.GetSourceLanguageFiles());
			if (sourceGuids.Count > 0)
			{
				newProject.RunAutomaticTask(
					sourceGuids.ToArray(),
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat);

				newProject.RunAutomaticTask(
					sourceGuids.ToArray(),
					AutomaticTaskTemplateIds.CopyToTargetLanguages);
			}

			newProject.Save();

			return newProject;
		}

		public async Task<FileBasedProject> CreateBackTranslationProject(FileBasedProject project, string localProjectFolder,
			string targetLanguage, string iconPath,
			List<SourceFile> sourceFiles, string projectNameSuffix)
		{
			if (string.IsNullOrEmpty(projectNameSuffix))
			{
				throw new Exception(PluginResources.Warning_Message_ProjectNameSuffixCannotBeNull);
			}

			var projectInfo = project.GetProjectInfo();

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
				Name = projectInfo.Name + "-" + projectNameSuffix + "-" + targetLanguage,
				Description = projectInfo.Description,
				LocalProjectFolder = localProjectFolder,
				SourceLanguage = newSourceLanguage,
				TargetLanguages = new Language[] { newTargetLanguage },
				DueDate = projectInfo.DueDate,
				ProjectOrigin = Constants.ProjectOrigin_BackTranslationProject,
				IconPath = iconPath
			};

			var newProject = new FileBasedProject(newProjectInfo, projectReference);
			foreach (var sourceFile in sourceFiles)
			{
				if (!string.IsNullOrEmpty(sourceFile.FilePath) &&
					File.Exists(sourceFile.FilePath))
				{
					newProject.AddFiles(new[] { sourceFile.FilePath }, sourceFile.FolderPathInProject);
				}
			}
			newProject.Save();

			UpdateProjectSettingsBundle(newProject, null, null, null);

			// Remove any TMs that don't correspond to the language directions of the project
			UpdateTmConfiguration(newProject);

			var languageFileIds = newProject.GetSourceLanguageFiles().GetIds();

			newProject.RunAutomaticTask(
				languageFileIds,
				AutomaticTaskTemplateIds.Scan);

			var sourceGuids = GetProjectFileGuids(newProject.GetSourceLanguageFiles());
			if (sourceGuids.Count > 0)
			{
				newProject.RunAutomaticTask(
					sourceGuids.ToArray(),
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat);

				newProject.RunAutomaticTask(
					sourceGuids.ToArray(),
					AutomaticTaskTemplateIds.CopyToTargetLanguages);
			}

			newProject.Save();
			return await System.Threading.Tasks.Task.FromResult(newProject);
		}

		public void UpdateProjectSettingsBundle(FileBasedProject project, IReadOnlyCollection<Report> reports,
			IReadOnlyCollection<SDLTranscreateProjectFile> projectFiles, IReadOnlyCollection<SDLTranscreateBackProject> backProjects)
		{
			var settingsBundle = project.GetSettings();

			var reportsViewerBundle = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();
			reportsViewerBundle.ReportsJson.Value = JsonConvert.SerializeObject(reports ?? new List<Report>());
			project.UpdateSettings(reportsViewerBundle.SettingsBundle);

			var transcreateBundle = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();
			transcreateBundle.ProjectFilesJson.Value = JsonConvert.SerializeObject(projectFiles ?? new List<SDLTranscreateProjectFile>());
			project.UpdateSettings(transcreateBundle.SettingsBundle);

			var backTranslationBundle = settingsBundle.GetSettingsGroup<SDLTranscreateBackProjects>();
			backTranslationBundle.BackProjectsJson.Value = JsonConvert.SerializeObject(backProjects ?? new List<SDLTranscreateBackProject>());
			project.UpdateSettings(backTranslationBundle.SettingsBundle);

			project.Save();
		}

		public List<Report> GetProjectReports(FileBasedProject project)
		{
			if (project != null)
			{
				var settingsBundle = project.GetSettings();
				var reportsViewerBundle = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();
				return DeserializeReports(reportsViewerBundle.ReportsJson.Value);
			}

			return new List<Report>();
		}

		public void UpdateProjectIcon(FileBasedProject project, string iconPath)
		{
			var projectInfo = project?.GetProjectInfo();
			if (projectInfo != null && 
			    string.Compare(projectInfo.IconPath, iconPath, StringComparison.CurrentCultureIgnoreCase) != 0)
			{
				projectInfo.IconPath = iconPath;
				project.UpdateProject(projectInfo);
				project.Save();
			}
		}

		public string GetTranscreateIconPath(Common.PathInfo pathInfo)
		{
			if (pathInfo == null)
			{
				return null;
			}
			
			if (!File.Exists(pathInfo.ProjectIconFilePath))
			{
				using (var fs = new FileStream(pathInfo.ProjectIconFilePath, FileMode.Create))
				{
					PluginResources.sdl_transcreate_view.Save(fs);
				}
			}

			return pathInfo.ProjectIconFilePath;
		}

		public string GetBackTranslationIconPath(Common.PathInfo pathInfo)
		{
			if (pathInfo == null)
			{
				return null;
			}

			if (!File.Exists(pathInfo.BackTranslationIconFilePath))
			{
				using (var fs = new FileStream(pathInfo.BackTranslationIconFilePath, FileMode.Create))
				{
					PluginResources.back_translation_small.Save(fs);
				}
			}

			return pathInfo.BackTranslationIconFilePath;
		}

		private bool CanActivateFileBasedProject()
		{
			var studioVersion = _studioVersionService.GetStudioVersion();
			if (studioVersion != null)
			{
				var version = studioVersion.ExecutableVersion;
				if (version.Major < 16 && version.Minor < 1 && version.Build < 4)
				{
					return false;
				}
			}

			return true;
		}

		private List<Report> DeserializeReports(string value)
		{
			try
			{
				var reports = JsonConvert.DeserializeObject<List<Report>>(value);
				return reports?.ToList() ?? new List<Report>();
			}
			catch
			{
				// catch all; ignore
			}

			return new List<Report>();
		}

		public Interfaces.IProject GetProject(FileBasedProject selectedProject, IReadOnlyCollection<string> selectedFileIds, List<ProjectFile> projectFiles = null)
		{
			if (selectedProject == null)
			{
				return null;
			}

			var projectInfo = selectedProject.GetProjectInfo();

			Interfaces.IProject project = IsBackTranslationProject(projectInfo.ProjectOrigin)
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

			var existingProject = IsBackTranslationProject(projectInfo.ProjectOrigin)
				? GetBackTranslationProject(projectInfo.Id.ToString(), out _)
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

				foreach (var existingProjectBackTranslationProject in existingProject.BackTranslationProjects)
				{
					project.BackTranslationProjects.Add(existingProjectBackTranslationProject.Clone() as BackTranslationProject);
				}
			}
			else
			{
				project.ProjectFiles = GetProjectFiles(selectedProject, project, selectedFileIds);
			}

			AssignProjectFileXliffData(project, projectFiles);

			return project;
		}

		public Interfaces.IProject GetBackTranslationProject(string projectId, out Interfaces.IProject parentProject)
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

		public bool IsBackTranslationProject(string projectOrigin)
		{
			return string.Compare(projectOrigin, Constants.ProjectOrigin_BackTranslationProject,
				StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		private void UpdateTmConfiguration(FileBasedProject project)
		{
			if (project == null)
			{
				return;
			}

			try
			{
				var projectInfo = project.GetProjectInfo();
				var translationProviderConfiguration = project.GetTranslationProviderConfiguration();
				var cascadeEntries = new List<TranslationProviderCascadeEntry>();
				foreach (var cascadeEntry in translationProviderConfiguration.Entries)
				{
					var uri = cascadeEntry.MainTranslationProvider.Uri;
					if (FileBasedTranslationMemory.IsFileBasedTranslationMemory(uri))
					{
						var filePath = FileBasedTranslationMemory.GetFileBasedTranslationMemoryFilePath(uri);
						var memory = new FileBasedTranslationMemory(filePath);

						foreach (var languageDirection in memory.SupportedLanguageDirections)
						{
							if (IsSame(languageDirection?.SourceCulture.Name, projectInfo.SourceLanguage.CultureInfo.Name, true) &&
								LanguageExistsIn(projectInfo.TargetLanguages.ToList(), languageDirection?.TargetCulture.Name, true))
							{
								cascadeEntries.Add(cascadeEntry);
							}
						}
					}
					else
					{
						if (string.Compare(cascadeEntry.MainTranslationProvider.Uri.Scheme, "anytm.sdltm.file", StringComparison.CurrentCultureIgnoreCase) == 0)
						{
							var filePath = cascadeEntry.MainTranslationProvider.Uri.LocalPath;
							if (File.Exists(filePath))
							{
								var memory = new FileBasedTranslationMemory(filePath);
								foreach (var languageDirection in memory.SupportedLanguageDirections)
								{
									if ((IsSame(projectInfo.SourceLanguage.CultureInfo.Name, languageDirection?.SourceCulture.Name, true) &&
										LanguageExistsIn(projectInfo.TargetLanguages.ToList(), languageDirection?.TargetCulture.Name, true))
										||
										IsSame(projectInfo.SourceLanguage.CultureInfo.Name, languageDirection?.TargetCulture.Name, true) &&
										LanguageExistsIn(projectInfo.TargetLanguages.ToList(), languageDirection?.SourceCulture.Name, true))
									{
										cascadeEntries.Add(cascadeEntry);
									}
								}
							}
						}
						else
						{
							cascadeEntries.Add(cascadeEntry);
						}
					}
				}

				translationProviderConfiguration.Entries = cascadeEntries;
				project.UpdateTranslationProviderConfiguration(translationProviderConfiguration);
				project.Save();
			}
			catch
			{
				// catch all; ignore
			}
		}

		private bool IsSame(string value1, string value2, bool ignoreCase)
		{
			return ignoreCase
				? string.Compare(value1, value2, StringComparison.CurrentCultureIgnoreCase) == 0
				: string.CompareOrdinal(value1, value2) == 0;
		}

		private bool LanguageExistsIn(List<Language> values, string value, bool ignoreCase)
		{
			return ignoreCase
				? values.Exists(a => string.Compare(a.CultureInfo.Name, value, StringComparison.CurrentCultureIgnoreCase) == 0)
				: values.Exists(a => string.CompareOrdinal(a.CultureInfo.Name, value) == 0);
		}


		private List<Guid> GetProjectFileGuids(Sdl.ProjectAutomation.Core.ProjectFile[] projectFiles)
		{
			var fileGuids = new List<Guid>();
			foreach (var projectFile in projectFiles)
			{
				if (projectFile.Role == FileRole.Translatable)
				{
					fileGuids.Add(projectFile.Id);
				}
			}

			return fileGuids;
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

		private ProjectFile GetProjectFile(Interfaces.IProject project, Sdl.ProjectAutomation.Core.ProjectFile projectFile,
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
