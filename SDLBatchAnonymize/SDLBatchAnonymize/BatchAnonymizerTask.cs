using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.Service;
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
		private readonly BackupService _backupService =new BackupService();
		
		protected override void OnInitializeTask()
		{
			var projectController =SdlTradosStudio.Application.GetController<ProjectsController>();
			var currentProject = projectController?.CurrentProject;
			var projectInfo = currentProject?.GetProjectInfo();
			if (projectInfo != null)
			{
				_backupService.BackupProject(projectInfo.LocalProjectFolder, projectInfo.Name);

			}
			_settings = GetSetting<BatchAnonymizerSettings>();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var usernameService = new UserNameService();
			var resourceOriginsService = new ResourceOriginsService();
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerProcessor(_settings,usernameService,resourceOriginsService)));
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}
}
