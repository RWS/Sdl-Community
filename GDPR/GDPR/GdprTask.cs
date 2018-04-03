using System;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.GDPR
{
	[AutomaticTask("GDPR Task",
				   "Anonymize data",
				   "Anonymize personal data batch task",
				   GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(GdprSettings), typeof(GdprSettingsPage))]
	public class GdprTask : AbstractFileContentProcessingAutomaticTask
	{
		protected override void OnInitializeTask()
		{
			base.OnInitializeTask();
		}
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			//In here you should add your custom bilingual processor to the file converter
		}
	}
}
