using System;
using System.IO;
using System.Linq;
using Sdl.Community.Anonymizer.Helpers;
using Sdl.Community.Anonymizer.Process_Xliff;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;



namespace Sdl.Community.Anonymizer
{
	[AutomaticTask("Anonymizer Task",
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


		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var selectedPatternsFromGrid = _settings.RegexPatterns.Where(e => e.ShouldEnable).ToList();
			ProjectBackup.CreateProjectBackup(projectController.CurrentProject.FilePath);
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerPreProcessor(selectedPatternsFromGrid)));
		
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}

	[Action("Anonymizer Action",
		Name = "Decrypt data",
		Description = "Deanonymize data which was previously anonymize by the batch task",
		Icon = "unlock"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AnonymizerDeanonymizeAction : AbstractViewControllerAction<ProjectsController>
	{
		
		protected override void Execute()
		{
			var selectedProjects = Controller.SelectedProjects.ToList();
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			foreach (var project in selectedProjects)
			{
				var targetFiles = project.GetTargetLanguageFiles();
				foreach (var targetFile in targetFiles.ToList())
				{
					var converter =
						fileTypeManager.GetConverterToDefaultBilingual(targetFile.LocalFilePath, targetFile.LocalFilePath, null);
					converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new DecryptDataProcessor()));
					converter.Parse();
				}
				
			}
		}
	}
	[Action("Help Anonymizer Action",
		Name = "Help",
		Description = "Help",
		Icon = "question"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AnonymizerHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3199.gdpr");
		}
	}
}
