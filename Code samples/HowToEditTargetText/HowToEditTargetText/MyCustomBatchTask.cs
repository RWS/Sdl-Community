using Sdl.ProjectAutomation.AutomaticTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace HowToEditTargetText
{
	[AutomaticTask("My_Custom_Batch_Task_ID",
				   "My_Custom_Batch_Task_Name",
				   "My_Custom_Batch_Task_Description",
				   //[TODO] You can change the file type according to your needs
				   GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	//[TODO] You can change the file type according to your needs
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(MyCustomBatchTaskSettings), typeof(MyCustomBatchTaskSettingsPage))]
	class MyCustomBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			//In here you should add your custom bilingual processor to the file converter
			multiFileConverter.AddBilingualProcessor(new ContentProcessor()); 
		}
		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}
}
