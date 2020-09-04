using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.Transcreate.BatchTasks
{
	[AutomaticTask("Transcreate.BatchTasks.Convert",
		"TranscreateManager_BatchTasks_Convert_Name",
		"TranscreateManager_BatchTasks_Convert_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = false)]
	//[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(SDLTranscreateConvertSettings), typeof(ConvertSettingsPage))]
	public class ConvertBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			
		}

		public override bool ShouldProcessFile(ProjectFile projectFile)
		{
			return false;
		}
	}
}
