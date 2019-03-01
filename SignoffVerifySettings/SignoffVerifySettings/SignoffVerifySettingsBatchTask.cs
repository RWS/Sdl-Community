using Sdl.Community.SignoffVerifySettings.Service;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.SignoffVerifySettings
{
	[AutomaticTask("SignoffVerifySettingsID", "Signoff Verify Settings", "Signoff Verify Settings", GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(SignoffVerifySettings), typeof(SignoffVerifySettingsPage))]
	class SignoffVerifySettingsBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void OnInitializeTask()
		{
			var projectService = new ProjectService();
			projectService.GetCurrentProjectInformation();
		}
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var fileConverter = new FileConverter();
			multiFileConverter.AddBilingualProcessor(fileConverter);
		}

		public override void TaskComplete()
		{
			base.TaskComplete();
		}		
	}
}