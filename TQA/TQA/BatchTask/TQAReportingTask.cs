using System;
using NLog;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Core.Settings;
using Sdl.Community.TQA.Model;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Sdl.Community.TQA.Services;
using Sdl.Core.Globalization;
using Sdl.Core.Settings.Implementation.Xml;
using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;




namespace Sdl.Community.TQA.BatchTask
{

	[AutomaticTask("TQAReporting",
	"Plugin_Name",
	"Plugin_Description",
	GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(TQAReportingSettings), typeof(TQAReportingSettingsPage))]
	public class TQAReportingTask : AbstractFileContentProcessingAutomaticTask
	{
		private const string suffixFileNameSeparator = "_";
		private AutomaticTask tqaAutomaticTask;
		private readonly string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
		private readonly Logger TQALogger = LogManager.GetCurrentClassLogger();
		private AssessmentCategories TQACategories { get; set; }
		private TQStandardType CurrentTQStandardType { get; set; }
		private ISettingsBundle ProjectSettings { get; set; }

		private List<ProjectFile> ProcessedProjectFiles { get; set; }
		private IProject CurrentProject { get; }
		internal string UserReportFileName { get; set; }

		public TQAReportingTask(FileBasedProject currentProject)
		{
			CurrentProject = currentProject;
			ProjectSettings = CurrentProject.GetSettings();
			TQACategories = GetAssessmentCategories(ProjectSettings);
			CurrentTQStandardType = GetTQSStandardBasedOnCurrentImportedTemplate();
			ProcessedProjectFiles = new List<ProjectFile>();
			CheckTQAReportingSettings(ProjectSettings);
		}

		public TQAReportingTask() : base()
		{

		}

		public override void TaskComplete()
		{
			base.TaskComplete();
			//GenerateTQAReports();
		}

		internal string GetReportOutputFile(string forLanguage)
		{//to be refactored
			var reportOutputFileName = string.Empty;
			if (string.IsNullOrEmpty(UserReportFileName))
				reportOutputFileName = string.Concat(TQStandardsFactory.GetReportOutputFilenameForTQStandard(CurrentTQStandardType),
					forLanguage, TQStandardsFactory.reportingFileExtension);
			else if (string.IsNullOrEmpty(forLanguage))
				reportOutputFileName = UserReportFileName;
			else if (UserReportFileName.Contains("."))
			{
				var fileNameComponents = UserReportFileName.Split('.').ToList();
				if (fileNameComponents.Last().Equals(TQStandardsFactory.excelFormatFile))
				{
					var indexOfExt = fileNameComponents.IndexOf(TQStandardsFactory.excelFormatFile);
					fileNameComponents.Insert(indexOfExt, forLanguage);
					fileNameComponents[indexOfExt + 1] = TQStandardsFactory.reportingFileExtension;
				}

				reportOutputFileName = string.Join("", fileNameComponents);
			}

			if (string.IsNullOrEmpty(Path.GetDirectoryName(reportOutputFileName)))
			{
				var tqaReportingSettings = ProjectSettings.GetSettingsGroup("TQAReportingSettings");
				var reportOutputFolder = tqaReportingSettings.GetSetting<string>("TQAReportOutputLocation").Value;//(tqaReportingSettings as TQAReportingSettings)?.TQAReportOutputLocation;
				if (string.IsNullOrEmpty(reportOutputFolder))
					reportOutputFolder = TQAReportingSettings.GetReportingOutputFolder();
				if (!Directory.Exists(reportOutputFolder))
					Directory.CreateDirectory(reportOutputFolder);
				reportOutputFileName = Path.Combine(reportOutputFolder, reportOutputFileName);
			}

			return reportOutputFileName;
		}
		internal bool CurrentStandardTypeIsSetAndSupported()
		{
			return CurrentTQStandardType == TQStandardType.tqsJ2450 || CurrentTQStandardType == TQStandardType.tqsMQM;
		}
		internal List<string> GetQualitiesForTQAStandard()
		{
			return TQStandardsFactory.GetTQSQualities(CurrentTQStandardType);
		}
		internal string GetCurrentTQStandardDescription()
		{
			return TQStandardsFactory.GetCurrentTQStandardDescription(CurrentTQStandardType);
		}
		internal List<Guid> GetCategoriesIds()
		{
			return TQACategories.Select(cat => cat.Id).Distinct().ToList();
		}
		internal List<string> GetCategoriesNames()
		{
			return TQACategories.Select(category => category.Name).Distinct().ToList();
		}

		//this method will execute the Automatic task for all project files currently selected into FilesEditor
		//or for all Project files if the Run TQA reporting command it's executed against to the entire project
		internal void ExecuteTQATaskForProjectFiles(List<ProjectFile> aForFiles, string tqaReportingUserQuality)
		{
			if (tqaReportingUserQuality != null)
				ProjectSettings.GetSettingsGroup("TQAReportingSettings").GetSetting<string>("TQAReportingQuality").Value =
					tqaReportingUserQuality;
			var targetLanguages = aForFiles.Select(file => file.GetLanguageDirection().TargetLanguage).Distinct().ToList();

			//execute for each different target language from the files to be processed.
			foreach (var targetLanguage in targetLanguages)
			{
				var filesToBeProcessed = aForFiles.Where(file => file.GetLanguageDirection().TargetLanguage.Equals(targetLanguage)).ToList();
				if (ExecuteTQATaskOnFiles(filesToBeProcessed))
					GenerateTQAReports(GetReportFileNameSuffix(targetLanguages.Count > 1, targetLanguage));
			}
		}

		private string GetReportFileNameSuffix(bool multipleTargetLanguages, Language targetLanguage)
		{
			//we generate one suffixed report file for each target language just when: 
			//1. the project contains files with different target languages
			//2. the command is executed for specific user selected files from project and those have different target languages
			if (multipleTargetLanguages)
				//the file name will be suffixed by the target language CultureInfo name)
				return string.Concat(suffixFileNameSeparator, targetLanguage.CultureInfo.Name);
			return string.Empty;
		}
		protected override void OnInitializeTask()
		{

			base.OnInitializeTask();
			ProjectSettings = GetCurrentProject().GetSettings();
			TQACategories = GetAssessmentCategories(ProjectSettings);
			CurrentTQStandardType = GetTQSStandardBasedOnCurrentImportedTemplate();
			ProcessedProjectFiles = new List<ProjectFile>();
		}

		internal void CheckTQAReportingSettings(ISettingsBundle projectSettings)
		{
			var tqaReportingSettings = projectSettings.GetSettingsGroup("TQAReportingSettings");
			var reportingQualities = tqaReportingSettings.GetSetting<List<string>>("TQAReportingQualities");
			if (reportingQualities == null || reportingQualities.Value == null || !reportingQualities.Value.Any())
				AddTQAReportingSettingsGroupToProjectFile();
			else
			{
				var tqaQualities = GetQualitiesForTQAStandard();
				if (!reportingQualities.Value.All(quality =>
						tqaQualities.Contains(quality)))
					reportingQualities.Reset();
			}
		}

		private void AddTQAReportingSettingsGroupToProjectFile()
		{
			//can't be added in this way. TBD
			//var tqaReportSettings = new TQAReportingSettings();

			//tqaReportSettings.TQAReportingQualities.AddRange(GetQualitiesForTQAStandard());
			//tqaReportSettings.TQAReportingQuality = tqaReportSettings.TQAReportingQualities.Count > 0
			//	? tqaReportSettings.TQAReportingQualities[0]
			//	: "<unknown>";
			//tqaReportSettings.TQAReportOutputLocation = TQAReportingSettings.GetReportingOutputFolder();
		}

		//execute the automatic task against to FilesEditor current selected file or all project files 
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			SaveActiveFile();
			var selectedFiles = GetSelectedFilesFromFilesControllerOrTask();
			if (selectedFiles.Count > 1)
				ExecuteTQATaskForProjectFiles(selectedFiles, null);
			else
				ExecuteTQATaskOnFile(projectFile);

		}

		private void ExecuteTQATaskOnFile(ProjectFile projectFile)
		{
			var projectFiles = new List<ProjectFile> { projectFile };
			if (ExecuteTQATaskOnFiles(projectFiles))
				GenerateTQAReports(string.Empty);
		}
		private bool ExecuteTQATaskOnFiles(List<ProjectFile> projectFiles)
		{
			//avoiding to be executed twice from ConfigureConverter until I'll see why 
			if (projectFiles.Select(file => file.Name + file.Id).ToList().TrueForAll(fileIdAndName => ProcessedProjectFiles.Select(fileProc => fileProc.Name + fileProc.Id).ToList().Contains(fileIdAndName)))
				return false;
			var projectFilesIds = projectFiles.Select(file => file.Id).ToArray();
			tqaAutomaticTask = GetCurrentProject().RunAutomaticTask(projectFilesIds, "Sdl.ProjectApi.AutomaticTasks.Feedback");
			if (tqaAutomaticTask == null || tqaAutomaticTask.Status != TaskStatus.Completed)
				TQALogger.Error($"{tqaAutomaticTask?.Messages?.FirstOrDefault()?.Message}\n " + PluginResources.MsgTQATaskNotRunCorrectly);
			ProcessedProjectFiles.AddRange(projectFiles);
			return true;
		}

		private IProject GetCurrentProject()
		{
			return CurrentProject ?? Project;
		}
		private List<ProjectFile> GetSelectedFilesFromFilesControllerOrTask()
		{
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();

			if (TaskFiles.Length > 1)
				return TaskFiles.ToList();

			return filesController?.SelectedFiles.ToList();
		}


#warning 'Neimplementata trebuie sa extraga din setarile Proiectului'
		internal string GetTQAQualityFromBatchTaskSettings()
		{
			var tqaReportingSettings = ProjectSettings.GetSettingsGroup("TQAReportingSettings");
			return tqaReportingSettings.GetSetting<string>("TQAReportingQuality").Value;
		}
		private static void SaveActiveFile()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeFile = editorController?.ActiveDocument;

			if (activeFile != null)
			{
				editorController.Save(activeFile); // if the file is not saved in the editor TQA changes does not appear in the report
			}
		}

		private bool GenerateTQAReports(string forLanguage)
		{
			GetCurrentProject().SaveTaskReportAs(tqaAutomaticTask.Reports[0].Id, tempFileName, ReportFormat.Xml);
			var extractedData = TQAReportGenerationService.ExtractFromXml(tempFileName, GetTQAQualityFromBatchTaskSettings());

			try
			{
				var reportFileName = GetReportOutputFile(forLanguage);
				TQAReportGenerationService.WriteExcel(reportFileName, extractedData, CurrentTQStandardType);
				MessageBox.Show(string.Format(PluginResources.MsgTQAProcessCompleted, string.Join("\n", reportFileName)), PluginResources.ReportGenerationFinished, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return true;
			}

			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return false;
			}
		}

		private TQStandardType GetTQSStandardBasedOnCurrentImportedTemplate()
		{
			return TQSCategories.GetStandardBasedOnCategories(TQACategories);//GetCategoriesNames()
		}



		private static AssessmentCategories GetAssessmentCategories(ISettingsBundle settingsBundle)
		{
			return GetCategorySettings<AssessmentCategories>(settingsBundle, "AssessmentCategories");
		}
		private static T GetCategorySettings<T>(ISettingsBundle settingsBundle, string setting) where T : new()
		{
			var settingsGroup = settingsBundle.GetSettingsGroup("TranslationQualityAssessmentSettings");
			return !settingsGroup.ContainsSetting(setting) ? new T() : settingsGroup.GetSetting<T>(setting);
		}






	}
}