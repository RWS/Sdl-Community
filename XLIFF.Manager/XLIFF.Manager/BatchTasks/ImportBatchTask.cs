using System;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using ProjectFile = Sdl.ProjectAutomation.Core.ProjectFile;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	[AutomaticTask("XLIFF.Manager.BatchTasks.Import",
		"Import from XLIFF",
		"Import from XLIFF",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(XliffManagerImportSettings), typeof(ImportSettingsPage))]
	public class ImportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void OnInitializeTask()
		{
			throw new Exception("Not available!");
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{

		}

		public override bool ShouldProcessFile(ProjectFile projectFile)
		{
			return false;
		}
	}
}
