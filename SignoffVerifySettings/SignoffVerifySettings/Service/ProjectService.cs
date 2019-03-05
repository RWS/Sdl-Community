using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.SignoffVerifySettings.Helpers;
using Sdl.Community.SignoffVerifySettings.Model;
using Sdl.Community.Toolkit.Core;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SignoffVerifySettings.Service
{
	public class ProjectService
	{
		private List<PhaseXmlNodeModel> _phaseXmlNodeModels = new List<PhaseXmlNodeModel>();
		private List<LanguageFileXmlNodeModel> _targetFiles = new List<LanguageFileXmlNodeModel>();

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
				_targetFiles = GetTargetFilesSettingsGuids(currentProject);
				GetSettingsBundleInformation(currentProject);

				GetQAVerificationInfo(projectInfo);

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
			var doc = Utils.LoadXmlDocument(currentProject.FilePath);
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

			_targetFiles = langFileXMLNodeModels.Where(l => l.LanguageCode != sourceLangauge).ToList();
			return _targetFiles;
		}

		/// <summary>
		/// Get Settings Bundle information for the target files from the .sdlproj xml document using the Settings Bundle Guids
		/// </summary>
		/// <param name="targetFiles">target files</param>
		/// <param name="currentProject">current project selected</param>
		private void GetSettingsBundleInformation(FileBasedProject currentProject)
		{
			var doc = Utils.LoadXmlDocument(currentProject.FilePath);
			var settings = currentProject.GetSettings();

			//foreach target file get the phase information
			foreach (var targetFile in _targetFiles)
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
		private void GetQAVerificationInfo(ProjectInfo projectInfo)
		{
			//get report info for each targetFile
			foreach (var targetFile in _targetFiles)
			{				
				string reportPath = $@"{projectInfo.LocalProjectFolder}\Reports\Verify Files {projectInfo.SourceLanguage.CultureInfo.Name}_{targetFile.LanguageCode}.xml";
				if (File.Exists(reportPath))
				{
					var doc = Utils.LoadXmlDocument(reportPath);					
					var fileNodes = doc.GetElementsByTagName("file");
					foreach (XmlNode fileNode in fileNodes)
					{
						if (fileNode.Attributes.Count > 0)
						{
							// get the info only for those target files on which the 'Verify Files' batch task has been run.
							var reportFileGuid = fileNode.Attributes["guid"].Value;
							if (targetFile.LanguageFileGUID.Equals(reportFileGuid))
							{
								targetFile.FileName = fileNode.Attributes["name"].Value;

								// the first element is selected, because there is only one 'task' tag defined in the report structure
								var taskInfoNode = doc.GetElementsByTagName("taskInfo")[0];
								if (taskInfoNode.Attributes.Count > 0)
								{
									targetFile.RunAt = taskInfoNode.Attributes["runAt"].Value;
								}
							}
						}
					}
				}
			}

			// get info from the last "Verify Files" report which is generated after running the "Verify Files" batch task on all files

		}
	}
}