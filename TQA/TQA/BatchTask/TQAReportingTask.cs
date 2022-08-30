using System;
using NLog;
using System.Linq;
using System.Collections.Generic;
using Sdl.Community.TQA.Providers;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.TQA.BatchTask
{
	[AutomaticTask(Constants.TQAReporting_BatchTask_Id,
		"Plugin_Name",
		"Plugin_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(TQAReportingSettings), typeof(TQAReportingSettingsPage))]
	public class TQAReportingTask : AbstractFileContentProcessingAutomaticTask
	{
		private Logger _logger;

		private TQAProfileType _tqaProfileType;
		private ReportProvider _reportProvider;
		private CategoriesProvider _categoriesProvider;
		private QualitiesProvider _qualitiesProvider;

		private List<ProjectFile> _projectFiles;

		protected override void OnInitializeTask()
		{
			base.OnInitializeTask();

			_logger = LogManager.GetCurrentClassLogger();
			_reportProvider = new ReportProvider(_logger);
			_categoriesProvider = new CategoriesProvider();
			_qualitiesProvider = new QualitiesProvider();
			_projectFiles = new List<ProjectFile>();

			var projectSettings = Project.GetSettings();
			_tqaProfileType = _categoriesProvider.GetTQAProfileType(projectSettings);
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			if (!IsValidProfileType())
			{
				return;
			}
			if (!_projectFiles.Exists(a => a.Id == projectFile.Id))
			{
				_projectFiles.Add(projectFile);
			}
		}


		public override void TaskComplete()
		{
			if (!IsValidProfileType())
			{
				throw new Exception(string.Format(PluginResources.Message_Invalid, _reportProvider.GetProfileTypeName(_tqaProfileType)));
			}

			Execute(_projectFiles);

			_projectFiles.Clear();

			base.TaskComplete();
		}

		private bool IsValidProfileType()
		{
			return _tqaProfileType == TQAProfileType.tqsJ2450 || _tqaProfileType == TQAProfileType.tqsMQM;
		}

		private void Execute(IReadOnlyCollection<ProjectFile> projectFiles)
		{
			var localProjectFolder = Project.GetProjectInfo().LocalProjectFolder;
			var defaultReportsDirectory = _reportProvider.GetReportDirectory(localProjectFolder);
			var defaultReportFileName = _reportProvider.GetReportDefaultOutputFilename(_tqaProfileType);

			var quality = _qualitiesProvider.GetQuality(Project);
			var fileBasedProject = Project as FileBasedProject;

			foreach (var language in Project.GetProjectInfo().TargetLanguages)
			{
				var reportFilePath = _reportProvider.GetDefaultReportFileFullPath(defaultReportsDirectory, defaultReportFileName, language.CultureInfo.Name);
				var languageProjectFiles = projectFiles.Where(a => a.Language.CultureInfo.Name == language.CultureInfo.Name).ToList();

				if (languageProjectFiles.Any())
				{
					var projectFilesIds = languageProjectFiles.Select(file => file.Id).ToArray();

					var tqaAutomaticTask = Project.RunAutomaticTask(projectFilesIds, Constants.AutomatedTasks_Feedback_Id);
					if (tqaAutomaticTask == null || tqaAutomaticTask.Status != TaskStatus.Completed)
					{
						_logger.Error($"{tqaAutomaticTask?.Messages?.FirstOrDefault()?.Message}\n " + PluginResources.MsgTQATaskNotRunCorrectly);
					}

					_reportProvider.GenerateReport(fileBasedProject, tqaAutomaticTask, _tqaProfileType, reportFilePath, quality);
				}
			}
		}
	}
}