using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	[AutomaticTask("XLIFF.Manager.BatchTasks.Import",
		"Import from XLIFF",
		"Import from XLIFF",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(ImportSettings), typeof(ImportSettingsPage))]
	public class ImportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void OnInitializeTask()
		{
			var importSettings = GetSetting<ImportSettings>();

			base.OnInitializeTask();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var exportSettings = GetSetting<ImportSettings>();
		}
	}
}
