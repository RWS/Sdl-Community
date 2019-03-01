using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sdl.Community.SignoffVerifySettings.Model;
using Sdl.Community.Toolkit.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SignoffVerifySettings.Service
{
	public class ProjectService
	{
		public ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		public void GetCurrentProjectInformation()
		{
			// Studio version
			var studioVersion = GetStudioVersion();

			var currentProject = GetProjectController().CurrentProject;
			if (currentProject != null)
			{
				var projectInfo = currentProject.GetProjectInfo();
				if (projectInfo != null)
				{
					// Project Name
					var projectName = projectInfo.Name;
					// Language Pairs
					var sourceLanguage = projectInfo.SourceLanguage;
					var targetLanguages = projectInfo.TargetLanguages;
				}
				var targetFiles = GetTargetFilesSettingsGuids(currentProject);
			}			
		}

		// Get the opened Studio version
		private string GetStudioVersion()
		{
			var studioVersion = new Studio().GetStudioVersion();
			return studioVersion != null ? studioVersion.Version : string.Empty;
		}

		// Get Target Language File tag attributes from the .sdlproj xml document 
		private List<LanguageFileXMLNodeModel> GetTargetFilesSettingsGuids(FileBasedProject currentProject)
		{
			var doc = new XmlDocument();
			doc.Load(currentProject.FilePath);
			var languageFileElements = doc.GetElementsByTagName("LanguageFile");
			var langFileXMLNodeModels = new List<LanguageFileXMLNodeModel>();
			foreach (XmlNode elem in languageFileElements)
			{
				var languageFileXMLNodeModel = new LanguageFileXMLNodeModel
				{
					LanguageFileGUID = elem.Attributes["Guid"].Value,
					SettingsBundleGuid = elem.Attributes["SettingsBundleGuid"].Value,
					LanguageCode = elem.Attributes["LanguageCode"].Value
				};
				langFileXMLNodeModels.Add(languageFileXMLNodeModel);
			}
			var sourceLangauge = currentProject.GetProjectInfo().SourceLanguage.CultureInfo.Name;
			var targetFiles = langFileXMLNodeModels.Where(l => l.LanguageCode != sourceLangauge).ToList();
			return targetFiles;
		}

		// Get Settings Bundle for the target files from the .sdlproj xml document using the Settings Bundle Guids
		private void GetSettingsBundleInformation()
		{

		}
	}
}