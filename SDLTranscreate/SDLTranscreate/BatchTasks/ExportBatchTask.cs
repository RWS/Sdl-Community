using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.Transcreate.BatchTasks
{
	[AutomaticTask("Transcreate.Manager.BatchTasks.Export",
		"TranscreateManager_BatchTasks_Export_Name",
		"TranscreateManager_BatchTasks_Export_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	//[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(SDLTranscreateExportSettings), typeof(ExportSettingsPage))]
	public class ExportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			throw new System.NotImplementedException();
		}

		public override bool ShouldProcessFile(ProjectFile projectFile)
		{
			return false;
		}
	}
}
