using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.AdaptiveMT
{
	[AutomaticTask("Sdl.Community.AdaptiveMt",
		"Adaptive MT training",
		"My_Custom_Batch_Task_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(AdaptiveMtBatchTaskSettings), typeof(AdaptiveMtBatchTaskSettingsPage))]
	class AdaptiveMtBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			
		}
	}
}
