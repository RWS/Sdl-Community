using System.Windows;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.Service;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;


namespace Sdl.Community.SDLBatchAnonymize
{

	[AutomaticTask("SDL Batch Anonymizer",
		"SDL Batch Anonymizer",
		"Provides various options for anonymizing the files in a Studio project.",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(BatchAnonymizerSettings), typeof(BatchAnonymizerSettingsPage))]

	public class BatchAnonymizerTask : AbstractFileContentProcessingAutomaticTask
	{
		private BatchAnonymizerSettings _settings;
		private UserNameService _userNameService;
		private ResourceOriginsService _resourceOriginsService;
		private Window _batchTaskWindow;

		protected override void OnInitializeTask()
		{
			_resourceOriginsService = new ResourceOriginsService();
			_userNameService = new UserNameService();

			var projectInfo = Project?.GetProjectInfo();
			if (projectInfo is null) return;
			var backupService = new BackupService();

			Application.Current?.Dispatcher?.Invoke(() =>
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (!window.Title.Equals("Batch Processing")) continue;
					_batchTaskWindow = window;
					_batchTaskWindow.Closing += Window_Closing;
				}
			});

			backupService.BackupProject(projectInfo.LocalProjectFolder, projectInfo.Name);
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var generalSettings = GetSetting<BatchAnonymizerSettings>();

			_settings = generalSettings.UseGeneral ? generalSettings : GetAnonymizationSettings(projectFile.Language);

			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerProcessor(_settings, _userNameService, _resourceOriginsService)));
		}

		private BatchAnonymizerSettings GetAnonymizationSettings(Language targetLanguage)
		{
			var settingsForLanguageBundle = Project.GetSettings(targetLanguage);
			return settingsForLanguageBundle.GetSettingsGroup<BatchAnonymizerSettings>();
		}

		public override void TaskComplete()
		{
			var generalSettings = GetSetting<BatchAnonymizerSettings>();
			var targetLanguages = Project.GetProjectInfo().TargetLanguages;

			//Remove selected settings from sdlproj if the user selected clear settings
			foreach (var targetLanguage in targetLanguages)
			{
				var languageSettings = GetAnonymizationSettings(targetLanguage);
				if (generalSettings.ClearSettings)
				{
					var projectSettings = Project.GetSettings();
					RemoveSettings(projectSettings);
					Project.UpdateSettings(projectSettings);
				}
				if (languageSettings.ClearSettings)
				{
					var projectSettings = Project.GetSettings(targetLanguage);
					RemoveSettings(projectSettings);
					Project.UpdateSettings(targetLanguage, projectSettings);
				}
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var projectFilePath = Project.GetProjectInfo()?.Uri?.LocalPath;
			if (string.IsNullOrEmpty(projectFilePath)) return;

			var anonymizeProjService = new AnonymizeSdlProjService();
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var proj = projectController.CurrentProject;

			if (proj != null)
			{
				proj.Save();
				projectController.Close(proj);
				//Remove the comment and task template id any way
				anonymizeProjService.RemoveFileVersionComment(projectFilePath);
				anonymizeProjService.RemoveTemplateId(projectFilePath);
				projectController.Add(projectFilePath);
			}
			_batchTaskWindow.Closing -= Window_Closing;
		}

		private void RemoveSettings(ISettingsBundle projectSettingsBundle)
		{
			if (projectSettingsBundle is null) return;
			if (!projectSettingsBundle.ContainsSettingsGroup(nameof(BatchAnonymizerSettings))) return;
			projectSettingsBundle.RemoveSettingsGroup(nameof(BatchAnonymizerSettings));
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}
}
