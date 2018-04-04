using Sdl.Community.Anonymizer.Process_Xliff;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Anonymizer
{
	[AutomaticTask("GDPR Task",
				   "Anonymize data",
				   "Anonymize personal data batch task",
				   GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(AnonymizerSettings), typeof(AnonymizerSettingsPage))]
	public class AnonymizerTask : AbstractFileContentProcessingAutomaticTask
	{
		private AnonymizerSettings _settings;
		protected override void OnInitializeTask()
		{
			_settings = GetSetting<AnonymizerSettings>();
		}

		//metoda asta se apeleaza pentru fiecare fisier selectat la pasul anterior anterior
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			//In here you should add your custom bilingual processor to the file converter

			var worker = new Worker(_settings);
			worker.GeneratePreviewFiles(projectFile.LocalFilePath, multiFileConverter);

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
