using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Helpers;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class StudioService : IStudioService
	{
		public string ProjectsXmlPath { get; set; }
		public static readonly Log Log = Log.Instance;

		public StudioService()
		{
			ProjectsXmlPath = GetStudioProjectsXmlPath();
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

		/// <summary>
		/// Set project details
		/// </summary>
		/// <param name="projects"></param>
		/// <param name="newProjectDetails"></param>
		/// <returns></returns>
		public BindingList<ProjectDetails> SetProjectDetails(List<ProjectDetails> projects, BindingList<ProjectDetails> newProjectDetails)
		{
			if (newProjectDetails != null)
			{
				foreach (var project in projects)
				{
					newProjectDetails.Add(project);
				}
			}
			return newProjectDetails;
		}
		
		/// <summary>
		/// Remove the languages corresponding to the single file project
		/// </summary>
		/// <param name="languagesDictionary"></param>
		/// <param name="languages"></param>
		public void RemoveSingleFileProjectLanguages(Dictionary<string,bool> languagesDictionary, BindingList<LanguageDetails> languages)
		{
			// remove also the language corresponding to the single file project, when the "Is single file project" option is unchecked.
			if (languagesDictionary.Count > languages.Count)
			{
				var languagesToRemove = languagesDictionary.Where(item => !languages.Any(a => a.LanguageName.Equals(item.Key))).ToList();

				foreach (var languageToRemove in languagesToRemove)
				{
					languagesDictionary.Remove(languageToRemove.Key);
				}
			}
		}

		/// <summary>
		/// Add file path names to list (used to identify when user opens a single file project)
		/// </summary>
		/// <param name="studioProjectsPaths"></param>
		/// <returns></returns>
		public List<string> AddFilePaths(List<string> studioProjectsPaths)
		{
			var filePathNames = new List<string>();
			if (studioProjectsPaths == null)
			{
				studioProjectsPaths = new List<string>();
			}
			foreach (var path in studioProjectsPaths)
			{
				filePathNames.Add(Path.GetFileName(path));
			}
			return filePathNames;
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
				PojectLanguages = new Dictionary<string, bool>(),
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

				var projectStatus = ProjectInformation.GetProjectStatus(projectDetails.ProjectPath);
				projectDetails.Status = projectStatus;
				projectDetails.IsSingleFileProject = isSingleFileProject;

				ConfigureProjectLanguages(projectDetails);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectDetails;
		}

		// Set the project file path
		private ProjectDetails SetProjectFilePath(XmlNode projectNode, ProjectDetails projectDetails)
		{
			if (projectNode.Attributes != null)
			{
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
			return projectDetails;
		}

		// Configure the project languages using project details
		private void ConfigureProjectLanguages(ProjectDetails projectDetails)
		{
			var doc = new XmlDocument();
			doc.Load(projectDetails.ProjectPath);
			var projectLanguages = LoadLanguageDirections(doc);

			SetProjectLanguages(projectDetails, projectLanguages);
		}

		// Set project languages
		private void SetProjectLanguages(ProjectDetails project, Dictionary<string, LanguageDirection> languages)
		{
			try
			{
				foreach (var language in languages)
				{
					project?.PojectLanguages?.Add(language.Value.TargetLang.EnglishName, false);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetProjectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
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
				Log.Logger.Error($"LoadLanguageDirections method: {ex.Message}\n {ex.StackTrace}");
			}
			return languages;
		}

		// Get the path of Studio projects's xml file
		private string GetStudioProjectsXmlPath()
		{
			try
			{
				var shortStudioVersion = GetInstalledStudioShortVersion();
				if (string.IsNullOrEmpty(shortStudioVersion))
				{
					return string.Empty;
				}

				var projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $@"Studio {shortStudioVersion}\Projects\projects.xml");
				return projectsPath;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"GetStudioProjectsPath method: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
		}

		// Get the short Studio's version
		private string GetInstalledStudioShortVersion()
		{
			var studioService = new StudioVersionService();
			return studioService.GetStudioVersion()?.ShortVersion;
		}
	}
}
