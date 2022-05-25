using System;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using ProjectFile = Sdl.ProjectAutomation.Core.ProjectFile;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	[AutomaticTask("XLIFF.Manager.BatchTasks.Import",
		"XLIFFManager_BatchTasks_Import_Name",
		"XLIFFManager_BatchTasks_Import_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	//[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(XliffManagerImportSettings), typeof(ImportSettingsPage))]
	public class ImportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void OnInitializeTask()
		{
			throw new Exception(PluginResources.XLIFFManager_BatchTasks_Import_Description);
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
