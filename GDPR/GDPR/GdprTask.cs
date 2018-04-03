using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

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

	[Action("GDPR Action",
		Name = "Decrypt data",
		Description = "Deanonymize data which was previously anonymize by the batch task",
		Icon = "unlock"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class GdprDeanonymizeAction : AbstractAction
	{
		protected override void Execute()
		{
			
		}
	}
	[Action("Help GDPR Action",
		Name = "Help",
		Description = "Help",
		Icon = "question"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class GdprHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3199.gdpr");
		}
	}
}
