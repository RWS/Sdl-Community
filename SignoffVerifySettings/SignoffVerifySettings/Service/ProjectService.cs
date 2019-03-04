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
				GetSettingsBundleInformation(targetFiles, currentProject);
			}			
		}

		// Get the opened Studio version
		private string GetStudioVersion()
		{
			var studioVersion = new Studio().GetStudioVersion();
			return studioVersion != null ? studioVersion.Version : string.Empty;
		}

		// Get Target Language File tag attributes from the .sdlproj xml document 
		private List<LanguageFileXmlNodeModel> GetTargetFilesSettingsGuids(FileBasedProject currentProject)
		{
			var doc = new XmlDocument();
			doc.Load(currentProject.FilePath);
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

		// Get Settings Bundle for the target files from the .sdlproj xml document using the Settings Bundle Guids
		private void GetSettingsBundleInformation(List<LanguageFileXmlNodeModel> targetFiles, FileBasedProject currentProject)
		{
			var doc = new XmlDocument();
			doc.Load(currentProject.FilePath);
			var settings = currentProject.GetSettings();
			var phaseXmlNodeModels = new List<PhaseXmlNodeModel>();

			//foreach target file check if exists one specific settingsBundle 
			foreach (var targetFile in targetFiles)
			{
				var settingsBundles = doc.SelectSingleNode($"//SettingsBundle[@Guid='{targetFile.SettingsBundleGuid}']");
				foreach(XmlNode settingBundle in settingsBundles)
				{
					foreach(XmlNode childNode in settingBundle.ChildNodes)
					{
						if(childNode.Attributes.Count > 0)
						{
							var phaseXmlNodeModel = new PhaseXmlNodeModel();
							//To do: move the below code in a generic method (GetPhaseInformation()) in Utils.cs, because it needs to be used also for the other phases
							if (childNode.Attributes["Id"].Value.Equals(Constants.ReviewPhase))
							{
								foreach (XmlNode node in childNode.ChildNodes)
								{
									if (node.Attributes["Id"].Value.Equals(Constants.IsCurrentAssignment))
									{
										// get info for the current assignement of the phase
										phaseXmlNodeModel.PhaseName = Constants.ReviewPhase;
										phaseXmlNodeModel.IsCurrentAssignment = node.InnerText;
										phaseXmlNodeModel.TargetFileGuid = targetFile.LanguageFileGUID;										
									}
								}

								// get the number of assignees
								var assigneesNumber = childNode.LastChild != null
									? childNode.LastChild != null
									? childNode.LastChild.LastChild != null
									? childNode.LastChild.LastChild.ChildNodes.Count > 0
									? childNode.LastChild.LastChild.ChildNodes.Count
									: 0 : 0 : 0 : 0;

								phaseXmlNodeModel.AssigneesNumber = assigneesNumber;
								phaseXmlNodeModels.Add(phaseXmlNodeModel);
							}
						}
					}
				}
			}
		}
	}
}