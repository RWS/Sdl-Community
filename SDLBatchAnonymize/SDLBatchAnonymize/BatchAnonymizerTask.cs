using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.Service;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;


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

		protected override void OnInitializeTask()
		{
			_resourceOriginsService = new ResourceOriginsService();
			_userNameService = new UserNameService();
			
			var projectInfo = Project?.GetProjectInfo();
			if (projectInfo is null) return;
			var backupService = new BackupService();
			
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

			foreach (var targetLanguage in targetLanguages)
			{
				var languageSettings = GetAnonymizationSettings(targetLanguage);
				if (generalSettings.UseGeneral && generalSettings.ClearSettings)
				{
					var projectSettings = Project.GetSettings();
					RemoveSettings(projectSettings);
					Project.UpdateSettings(projectSettings);

				}
				else if (languageSettings.ClearSettings)
				{
					var projectSettings = Project.GetSettings(targetLanguage);
					RemoveSettings(projectSettings);
					Project.UpdateSettings(targetLanguage, projectSettings);
				}
			}
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
