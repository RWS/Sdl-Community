using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NLog;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class ProjectService : IProjectService
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ProjectService()
		{
			ProjectController = GetProjectsController();
			ProjectsXmlPath = GetStudioProjectsXmlPath();
		}

		public string ProjectsXmlPath { get; set; }
		
		public ProjectsController ProjectController { get; set; }

		/// <summary>
		/// Add file path names to list (used to identify when user opens a single file project)
		/// </summary>
		/// <param name="studioProjectsPaths"></param>
		/// <returns></returns>
		public List<string> AddFilePaths(List<string> studioProjectsPaths)
		{
			if (studioProjectsPaths != null) return studioProjectsPaths.Select(path => Path.GetFileName(path)).ToList();

			studioProjectsPaths = new List<string>();
			return studioProjectsPaths.Select(path => Path.GetFileName(path)).ToList();
		}

		/// <summary>
		/// Creates project details for given project from xml file
		/// </summary>
		/// <param name="projNode"></param>
		/// <param name="isSingleFileProject"></param>
		/// <param name="reportsFolderPath"></param>
		/// <returns></returns>
		public ProjectDetails CreateProjectDetails(XmlNode projNode, bool isSingleFileProject, string reportsFolderPath)
		{
			var projectDetails = new ProjectDetails
			{
				ProjectLanguages = new Dictionary<string, bool>(),
				ShouldBeExported = false,
				ReportsFolderPath = reportsFolderPath
			};

			try
			{
				var selectSingleNode = projNode.SelectSingleNode("ProjectInfo");
				if (selectSingleNode?.Attributes != null)
				{
					projectDetails.ProjectName = selectSingleNode.Attributes["Name"].Value;
				}

				projectDetails = SetProjectFilePath(projNode, projectDetails);

				var projectStatus = GetProjectStatus(projectDetails.ProjectPath);
				projectDetails.Status = projectStatus;
				projectDetails.IsSingleFileProject = isSingleFileProject;

				ConfigureProjectLanguages(projectDetails);
			}
			catch (Exception ex)
			{
				_logger.Error($"CreateProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectDetails;
		}

		/// <summary>
		/// Get project information
		/// </summary>
		/// <param name="projectPath"></param>
		/// <returns></returns>
		public ProjectInfo GetProjectInfo(string projectPath)
		{
			var fileBasedProject = new FileBasedProject(projectPath);
			var projectInfo = fileBasedProject?.GetProjectInfo();

			return projectInfo;
		}

		public List<FileBasedProject> GetStudioProjects()
		{
			return ProjectController?.GetAllProjects()?.ToList();
		}

		public List<FileBasedProject> GetSelectedStudioProjects()
		{
			return ProjectController?.SelectedProjects.ToList();
		}

		/// <summary>
		/// Remove the languages corresponding to the single file project
		/// </summary>
		/// <param name="languagesDictionary"></param>
		/// <param name="languages"></param>
		public void RemoveSingleFileProjectLanguages(Dictionary<string, bool> languagesDictionary, BindingList<LanguageDetails> languages)
		{
			try
			{
				// remove also the language corresponding to the single file project, when the "Is single file project" option is unchecked.
				if (languagesDictionary.Count <= languages.Count) return;

				var languagesToRemove = languagesDictionary.Where(item => !languages.Any(a => a.LanguageName.Equals(item.Key))).ToList();
				foreach (var languageToRemove in languagesToRemove)
				{
					languagesDictionary.Remove(languageToRemove.Key);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"RemoveSingleFileProjectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public void SetProjectLanguages(List<ProjectDetails> projectDetails, bool isChecked, string languageName)
		{
			foreach (var project in projectDetails)
			{
				if (project.ProjectLanguages != null)
				{
					var language = project.ProjectLanguages.FirstOrDefault(l => l.Key.Equals(languageName));
					if (language.Key != null)
					{
						project.ProjectLanguages[language.Key] = isChecked;
					}
				}
			}
		}

		public void SetAllProjectDetails(List<ProjectDetails> allProjectDetails, ProjectDetails projectDetails)
		{
			if (!allProjectDetails.Any(p => p.ProjectName.Equals(projectDetails.ProjectName)))
			{
				allProjectDetails.Add(projectDetails);
			}
		}

		public void RemoveAllSingleProjects(List<ProjectDetails> allProjectDetails)
		{
			allProjectDetails.RemoveAll(x => x.IsSingleFileProject);
		}

		// Get the short Studio's version
		public virtual string GetInstalledStudioShortVersion()
		{
			var studioService = new StudioVersionService();
			return studioService.GetStudioVersion()?.ShortVersion;
		}

		public ProjectDetails GetExternalProjectDetails(string path, string reportFolderPath)
		{
			try
			{
				var fileBasedProject = new FileBasedProject(path);
				var projectInfo = fileBasedProject.GetProjectInfo();

				var projectDetails = new ProjectDetails
				{
					ProjectName = projectInfo?.Name,
					ProjectPath = projectInfo?.Uri.LocalPath,
					Status = GetInternalProjectStatus(fileBasedProject),
					ProjectLanguages = new Dictionary<string, bool>(),
					ShouldBeExported = true,
					ReportsFolderPath = reportFolderPath
				};

				foreach (var language in projectInfo?.TargetLanguages)
				{
					projectDetails.ProjectLanguages.Add(language.DisplayName, true);
				}

				ProjectController?.Close(fileBasedProject);

				return projectDetails;
			}
			catch (Exception ex)
			{
				_logger.Error($"GetExternalProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}
			return new ProjectDetails();
		}

		// Configure the project languages using project details
		private void ConfigureProjectLanguages(ProjectDetails projectDetails)
		{
			try
			{
				var doc = new XmlDocument();
				doc.Load(projectDetails.ProjectPath);
				var projectLanguages = LoadLanguageDirections(doc);

				SetProjectLanguages(projectDetails, projectLanguages);
			}
			catch (Exception ex)
			{
				_logger.Error($"ConfigureProjectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private ProjectsController GetProjectsController()
		{
			return SdlTradosStudio.Application?.GetController<ProjectsController>();
		}

		// Get the path of Studio projects's xml file
		private string GetStudioProjectsXmlPath()
		{
			try
			{
				var shortStudioVersion = GetInstalledStudioShortVersion();
				if (string.IsNullOrEmpty(shortStudioVersion)) return string.Empty;
				var projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $@"Studio {shortStudioVersion}\Projects\projects.xml");
				return projectsPath;
			}
			catch (Exception ex)
			{
				_logger.Error($"GetStudioProjectsPath method: {ex.Message}\n {ex.StackTrace}");
			}

			return string.Empty;
		}

		// Load language directions from the xml document
		private Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc)
		{
			var languages = new Dictionary<string, LanguageDirection>();
			try
			{
				var languagesDirectionNode = doc.SelectNodes("/Project/LanguageDirections/LanguageDirection");

				if (languagesDirectionNode == null) return languages;
				foreach (var item in languagesDirectionNode)
				{
					var node = (XmlNode)item;
					if (node.Attributes == null) continue;
					var lang = new LanguageDirection
					{
						Guid = node.Attributes["Guid"].Value,
						TargetLang = CultureInfo.GetCultureInfo(node.Attributes["TargetLanguageCode"].Value)
					};
					if (!languages.ContainsKey(lang.Guid))
					{
						languages.Add(lang.Guid, lang);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"LoadLanguageDirections method: {ex.Message}\n {ex.StackTrace}");
			}

			return languages;
		}

		// Set the project file path
		private ProjectDetails SetProjectFilePath(XmlNode projectNode, ProjectDetails projectDetails)
		{
			try
			{
				if (projectNode.Attributes == null) return projectDetails;
				var projectFilePath = projectNode.Attributes["ProjectFilePath"]?.Value;
				if (!string.IsNullOrEmpty(projectFilePath))
				{
					if (Path.IsPathRooted(projectFilePath))
					{
						projectDetails.ProjectPath = projectFilePath; //location outside standard project place
					}
					else
					{
						var projectXmlPath = Path.GetDirectoryName(ProjectsXmlPath);
						if (!string.IsNullOrEmpty(projectXmlPath))
						{
							projectDetails.ProjectPath = Path.Combine(projectXmlPath, projectFilePath);
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"SetProjectFilePath method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectDetails;
		}

		// Set project languages
		private void SetProjectLanguages(ProjectDetails project, Dictionary<string, LanguageDirection> languages)
		{
			try
			{
				foreach (var language in languages)
				{
					project?.ProjectLanguages?.Add(language.Value.TargetLang.EnglishName, false);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"SetProjectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private string GetInternalProjectStatus(FileBasedProject studioProject)
		{
			try
			{
				var internalProjPropertyInfo = studioProject?.GetType().GetProperty("InternalProject", BindingFlags.NonPublic | BindingFlags.Instance);

				if (internalProjPropertyInfo == null)
				{
					return string.Empty;
				}
				var internalProjMethodInfo = internalProjPropertyInfo.GetGetMethod(true);

				dynamic internalProject = internalProjMethodInfo.Invoke(studioProject, null);
				var statusProperty = internalProject.GetType().GetProperty("Status");
				var projectStatus = statusProperty.GetValue(internalProject, null).ToString();
				return projectStatus;
			}
			catch (Exception ex)
			{
				_logger.Error($"GetInternalProjectStatus method: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
		}

		private string GetProjectStatus(string projectPath)
		{
			var projectStatus = string.Empty;
			try
			{
				var studioProject = GetStudioProjects()?.FirstOrDefault(p => p.FilePath.Equals(projectPath));

				if (studioProject != null)
				{
					projectStatus = GetInternalProjectStatus(studioProject);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"GetProjectStatus method: {ex.Message}\n {ex.StackTrace}");

			}
			return projectStatus;
		}
	}
}