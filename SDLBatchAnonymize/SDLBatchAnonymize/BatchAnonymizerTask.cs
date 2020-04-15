using System.Linq;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.Service;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
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
		private FileBasedProject _currentProject;
		private BatchAnonymizerSettings _generalSettings;
		private UserNameService _userNameService;
		private ResourceOriginsService _resourceOriginsService;

		protected override void OnInitializeTask()
		{
			_generalSettings = GetSetting<BatchAnonymizerSettings>();
			_resourceOriginsService = new ResourceOriginsService();
			_userNameService = new UserNameService();

			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_currentProject = projectsController.CurrentProject ?? projectsController.SelectedProjects.FirstOrDefault();
			var projectInfo = _currentProject?.GetProjectInfo();
			if (projectInfo is null) return;
			var backupService = new BackupService();
			
			backupService.BackupProject(projectInfo.LocalProjectFolder, projectInfo.Name);
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var settings = _generalSettings.UseGeneral ? _generalSettings : GetAnonymizationSettings(projectFile.Language);
		
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerProcessor(settings, _userNameService, _resourceOriginsService)));
		}

		private BatchAnonymizerSettings GetAnonymizationSettings(Language targetLanguage)
		{
			var settingsForLanguageBundle = _currentProject.GetSettings(targetLanguage);
			return settingsForLanguageBundle.GetSettingsGroup<BatchAnonymizerSettings>();
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}
}
