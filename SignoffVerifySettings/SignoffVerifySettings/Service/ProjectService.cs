using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Sdl.Community.SignoffVerifySettings.Helpers;
using Sdl.Community.SignoffVerifySettings.Model;
using Sdl.Community.Toolkit.Core;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using static System.Environment;

namespace Sdl.Community.SignoffVerifySettings.Service
{
	public class ProjectService
	{
		#region Private Fields
		private List<PhaseXmlNodeModel> _phaseXmlNodeModels = new List<PhaseXmlNodeModel>();
		private List<LanguageFileXmlNodeModel> _targetFiles = new List<LanguageFileXmlNodeModel>();
		private ProjectsController _projectsController = new ProjectsController();
		private XmlDocument _document = new XmlDocument();
		private readonly Utils _utils;
		#endregion

		#region Constructors
		public ProjectService()
		{
			_utils = new Utils();
			_projectsController = GetProjectController();			
			_document = _utils.LoadXmlDocument(_projectsController != null ? _projectsController.CurrentProject != null? _projectsController.CurrentProject.FilePath : null : null);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get current project information which will be displayed in the Signoff Verify Settings report based on the selected files
		/// </summary>
		/// <param name="taskFiles">selected files from the project based on which the batch task is running</param>
		public void GetCurrentProjectInformation(ProjectFile[] taskFiles)
		{			
			// Studio version
			var studioVersion = GetStudioVersion();
			var currentProject = GetProjectController().CurrentProject;
			if (currentProject != null)
			{
				var projectInfoReportModel = new ProjectInfoReportModel();
				var projectInfo = currentProject.GetProjectInfo();
				if (projectInfo != null)
				{
					// Project Name
					projectInfoReportModel.ProjectName = projectInfo.Name;
					// Language Pairs
					projectInfoReportModel.SourceLanguage = projectInfo.SourceLanguage;
					projectInfoReportModel.TargetLanguages = projectInfo.TargetLanguages.ToList();
				}
				_targetFiles = GetTargetFilesSettingsGuids(currentProject, taskFiles);
				GetSettingsBundleInformation(currentProject);
				var runAt = GetQAVerificationInfo(projectInfo);

				projectInfoReportModel.PhaseXmlNodeModels = _phaseXmlNodeModels;
				projectInfoReportModel.LanguageFileXmlNodeModels = _targetFiles;
				projectInfoReportModel.RunAt = runAt;
				GetMaterialsInfo(currentProject, projectInfoReportModel);
				projectInfoReportModel.QAVerificationSettingsModels = GetQASettings();
				projectInfoReportModel.NumberVerifierSettingsModel = GetNumberVerifierSettings();
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Get project controller
		/// </summary>
		/// <returns>information for the Project Controller</returns>
		private ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
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
		/// <param name="taskFiles">selected project files from where the information is retrieved</param>
		/// <returns></returns>
		private List<LanguageFileXmlNodeModel> GetTargetFilesSettingsGuids(FileBasedProject currentProject, ProjectFile[] taskFiles)
		{
			var langFileXMLNodeModels = new List<LanguageFileXmlNodeModel>();
			foreach (var taskFile in taskFiles)
			{
				var languageFileNode = _document.SelectSingleNode($"//LanguageFile[@Guid='{taskFile.Id}']");

				if (languageFileNode.Attributes.Count > 0)
				{
					var languageFileXmlNodeModel = new LanguageFileXmlNodeModel
					{
						LanguageFileGUID = languageFileNode.Attributes["Guid"].Value,
						SettingsBundleGuid = languageFileNode.Attributes["SettingsBundleGuid"].Value,
						LanguageCode = languageFileNode.Attributes["LanguageCode"].Value,
						TargetFileFullPath = taskFile.LocalFilePath
					};
					langFileXMLNodeModels.Add(languageFileXmlNodeModel);
				}
			}
			var sourceLangauge = currentProject.GetProjectInfo() != null ? currentProject.GetProjectInfo().SourceLanguage != null
					? currentProject.GetProjectInfo().SourceLanguage.CultureInfo != null
					? currentProject.GetProjectInfo().SourceLanguage.CultureInfo.Name
					: string.Empty : string.Empty : string.Empty;
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
			var settings = currentProject.GetSettings();

			//foreach target file get the phase information
			foreach (var targetFile in _targetFiles)
			{
				var settingsBundles = _document.SelectSingleNode($"//SettingsBundle[@Guid='{targetFile.SettingsBundleGuid}']");
				foreach(XmlNode settingBundle in settingsBundles)
				{
					foreach(XmlNode childNode in settingBundle.ChildNodes)
					{
						if (childNode.Attributes.Count > 0)
						{
							_utils.GetPhaseInformation(Constants.ReviewPhase, childNode, targetFile, _phaseXmlNodeModels);
							_utils.GetPhaseInformation(Constants.TranslationPhase, childNode, targetFile, _phaseXmlNodeModels);
							_utils.GetPhaseInformation(Constants.PreparationPhase, childNode, targetFile, _phaseXmlNodeModels);
							_utils.GetPhaseInformation(Constants.FinalisationPhase, childNode, targetFile, _phaseXmlNodeModels);
						}
					}
				}
			}
		}

		/// <summary>
		/// Get the QA Verification last DateTime (RunAt value) based on the report which is generated after the Verify Files batch task has ran.
		/// The information for each target file is stored in _targetFiles list and "RunAt" info for the project is returned by the method as string
		/// </summary>
		/// <param name="currentProject">current project selected</param>
		private string GetQAVerificationInfo(ProjectInfo projectInfo)
		{
			var runAt = string.Empty;
			var directoryInfo = new DirectoryInfo($@"{projectInfo.LocalProjectFolder}\Reports\");

			//get report info for each targetFile
			foreach (var targetFile in _targetFiles)
			{
				var fileInfo = directoryInfo
					.GetFiles()
					.OrderByDescending(f => f.LastWriteTime)
					.FirstOrDefault(n => n.Name.StartsWith($@"Verify Files {projectInfo.SourceLanguage.CultureInfo.Name}_{targetFile.LanguageCode}"));

				if (fileInfo != null)
				{
					var reportPath = fileInfo.FullName;
					if (File.Exists(reportPath))
					{
						var doc = _utils.LoadXmlDocument(reportPath);
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
									targetFile.RunAt = _utils.GetRunAtValue(doc);
								}
							}
						}
					}
				}
			}

			// get "RunAt" info from the last "Verify Files" report which is generated after running the "Verify Files" batch task on all files
			var allReportFilesInfo = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault(n => n.Name.StartsWith("Verify Files ("));
			if (allReportFilesInfo != null)
			{
				var reportPath = allReportFilesInfo.FullName;
				if (File.Exists(reportPath))
				{
					var doc = _utils.LoadXmlDocument(reportPath);
					runAt = _utils.GetRunAtValue(doc);
				}
			}
			return runAt;
		}

		/// <summary>
		/// Get the Materials information: Translation Memories, Termbases and if Regex expression have been used.
		/// Get also the QACheckerRanResult value which represents the QA Checker if has been run or not. 
		/// </summary>
		/// <param name="currentProject">current selected project</param>
		/// <param name="projectInfoReportModel">projectInfoReportModel used further to display the information into report</param>
		private void GetMaterialsInfo(FileBasedProject currentProject, ProjectInfoReportModel projectInfoReportModel)
		{
			// get RegExRules & CheckRexEx values
			projectInfoReportModel.RegexRules = _document.SelectSingleNode($"//SettingsGroup/Setting[@Id='RegExRules']") != null
				? _document.SelectSingleNode($"//SettingsGroup/Setting[@Id='RegExRules']").FirstChild != null 
				? _document.SelectSingleNode($"//SettingsGroup/Setting[@Id='RegExRules']").FirstChild.Value 
				: string.Empty : string.Empty;
			projectInfoReportModel.CheckRegexRules = _document.SelectSingleNode($"//SettingsGroup/Setting[@Id='CheckRegEx']") != null
				? _document.SelectSingleNode($"//SettingsGroup/Setting[@Id='CheckRegEx']").FirstChild != null
				? _document.SelectSingleNode($"//SettingsGroup/Setting[@Id='CheckRegEx']").FirstChild.Value
				: string.Empty : string.Empty;

			// get translation memories & termbases used
			projectInfoReportModel.TranslationMemories = currentProject.GetTranslationProviderConfiguration().Entries;
			projectInfoReportModel.Termbases = currentProject.GetTermbaseConfiguration().Termbases;

			// the below value represent the QA Checker if has been run or not (if has been run, then the 'Verification Report' exists in .sdlproj)
			projectInfoReportModel.QACheckerRanResult = _document.SelectSingleNode($"//Reports/Report[@Name='Verification Report']") != null
				? "QA Checker had executed"
				: "QA Checker did not executed";
		}

		/// <summary>
		/// Get all the QA Settings applied
		/// </summary>
		/// <returns>list of QA settings which are applied to project</returns>
		private List<QAVerificationSettingsModel> GetQASettings()
		{
			var qaVerificationSettingsModels = new List<QAVerificationSettingsModel>();
			var qaVerificationSettings = _document.SelectSingleNode($"//SettingsGroup[@Id='QAVerificationSettings']");
			foreach(XmlNode qaVerifcaitionSetting in qaVerificationSettings)
			{
				var qaVerificationSettingsModel = new QAVerificationSettingsModel
				{
					Name = qaVerifcaitionSetting.Attributes.Count > 0 ? qaVerifcaitionSetting.Attributes["Id"].Value : string.Empty,
					Value = qaVerifcaitionSetting.FirstChild != null ? qaVerifcaitionSetting.FirstChild.Value : string.Empty
				};
				qaVerificationSettingsModels.Add(qaVerificationSettingsModel);
			}
			return qaVerificationSettingsModels;
		}

		/// <summary>
		/// Get information related to date and file on which NumberVerifier had been executed
		/// </summary>
		/// <returns>NumberVerifierSettingsModel</returns>
		private NumberVerifierSettingsModel GetNumberVerifierSettings()
		{
			var directoryPath = $@"{GetFolderPath(SpecialFolder.ApplicationData)}\SDL Community\NumberVerifier";
			var jsonPath = $@"{directoryPath}\NumberVerifierSettings.json";

			if (File.Exists(jsonPath))
			{
				using (var r = new StreamReader(jsonPath))
				{
					var json = r.ReadToEnd();
					return JsonConvert.DeserializeObject<NumberVerifierSettingsModel>(json);
				}
			}
			return new NumberVerifierSettingsModel();
		}
		#endregion
	}
}