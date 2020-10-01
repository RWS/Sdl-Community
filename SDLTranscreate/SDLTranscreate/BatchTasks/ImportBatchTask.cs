using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.Transcreate.BatchTasks
{
	[AutomaticTask("Transcreate.BatchTasks.Import",
		"TranscreateManager_BatchTasks_Import_Name",
		"TranscreateManager_BatchTasks_Import_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	//[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(SDLTranscreateImportSettings), typeof(ImportSettingsPage))]
	public class ImportBatchTask : AbstractFileContentProcessingAutomaticTask
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
