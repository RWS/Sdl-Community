using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sdl.Community.SignoffVerifySettings.Helpers;
using Sdl.Community.SignoffVerifySettings.Model;
using Sdl.Community.Toolkit.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SignoffVerifySettings.Service
{
	public class ProjectService
	{
		private List<PhaseXmlNodeModel> _phaseXmlNodeModels = new List<PhaseXmlNodeModel>();

		/// <summary>
		/// Get project controller
		/// </summary>
		/// <returns>information for the Project Controller</returns>
		public ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		/// <summary>
		/// Get current project information which will be displayed in the Signoff Verify Settings report
		/// </summary>
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
				GetSettingsBundleInformation(targetFiles, currentProject);
			}			
		}

		/// <summary>
		/// Get the opened Studio version
		/// </summary>
		/// <returns></returns>
		private string GetStudioVersion()
		{
			var studioVersion = new Studio().GetStudioVersion();
			return studioVersion != null ? studioVersion.Version : string.Empty;
		}

		/// <summary>
		/// Get Target Language File tag attributes from the .sdlproj xml document 
		/// </summary>
		/// <param name="currentProject">current project selected</param>
		/// <returns></returns>
		private List<LanguageFileXmlNodeModel> GetTargetFilesSettingsGuids(FileBasedProject currentProject)
		{
			var doc = Utils.LoadXmlDocument(currentProject);
			var languageFileElements = doc.GetElementsByTagName("LanguageFile");
			var langFileXMLNodeModels = new List<LanguageFileXmlNodeModel>();
			foreach (XmlNode elem in languageFileElements)
			{
				if (elem.Attributes.Count > 0)
				{
					var languageFileXmlNodeModel = new LanguageFileXmlNodeModel
					{
						LanguageFileGUID = elem.Attributes["Guid"].Value,
						SettingsBundleGuid = elem.Attributes["SettingsBundleGuid"].Value,
						LanguageCode = elem.Attributes["LanguageCode"].Value
					};
					langFileXMLNodeModels.Add(languageFileXmlNodeModel);
				}
			}
			var sourceLangauge = currentProject.GetProjectInfo() != null ? currentProject.GetProjectInfo().SourceLanguage != null
				? currentProject.GetProjectInfo().SourceLanguage.CultureInfo != null
				? currentProject.GetProjectInfo().SourceLanguage.CultureInfo.Name
				:string.Empty :string.Empty : string.Empty;

			var targetFiles = langFileXMLNodeModels.Where(l => l.LanguageCode != sourceLangauge).ToList();
			return targetFiles;
		}

		/// <summary>
		/// Get Settings Bundle information for the target files from the .sdlproj xml document using the Settings Bundle Guids
		/// </summary>
		/// <param name="targetFiles">target files</param>
		/// <param name="currentProject">current project selected</param>
		private void GetSettingsBundleInformation(List<LanguageFileXmlNodeModel> targetFiles, FileBasedProject currentProject)
		{
			var doc = Utils.LoadXmlDocument(currentProject);
			var settings = currentProject.GetSettings();

			//foreach target file get the phase information
			foreach (var targetFile in targetFiles)
			{
				var settingsBundles = doc.SelectSingleNode($"//SettingsBundle[@Guid='{targetFile.SettingsBundleGuid}']");
				foreach(XmlNode settingBundle in settingsBundles)
				{
					foreach(XmlNode childNode in settingBundle.ChildNodes)
					{
						if (childNode.Attributes.Count > 0)
						{
							Utils.GetPhaseInformation(Constants.ReviewPhase, childNode, targetFile, _phaseXmlNodeModels);
							Utils.GetPhaseInformation(Constants.TranslationPhase, childNode, targetFile, _phaseXmlNodeModels);
							Utils.GetPhaseInformation(Constants.PreparationPhase, childNode, targetFile, _phaseXmlNodeModels);
							Utils.GetPhaseInformation(Constants.FinalisationPhase, childNode, targetFile, _phaseXmlNodeModels);
						}
					}
				}
			}
		}

		/// <summary>
		/// Get the QA Verification information based on the report which is generated after the Verify Files batch task ran.
		/// </summary>
		/// <param name="currentProject">current project selected</param>
		private void GetQAVerificationInfo(FileBasedProject currentProject)
		{
			var doc = Utils.LoadXmlDocument(currentProject);
		}
	}
}