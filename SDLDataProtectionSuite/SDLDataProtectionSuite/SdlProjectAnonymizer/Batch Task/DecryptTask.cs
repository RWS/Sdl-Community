using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	[AutomaticTask("SDPSDecryptTask",
		"Unprotect Data",
		"Unprotect data in preparation for saving the target files",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(AnonymizerSettings), typeof(DecryptSettingsPage))]
	public class DecryptTask : AbstractFileContentProcessingAutomaticTask
	{
		private AnonymizerSettings _settings;

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}

		protected override void OnInitializeTask()
		{
			_settings = GetSetting<AnonymizerSettings>();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			if (!AgreementMethods.UserAgreed())
			{
				CancelHelper.CancelTaskIfRequested(true);
			}

            if (!_settings.ShouldDeanonymize ?? false)
			{
				return;
			}

			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new DecryptDataProcessor(_settings)));

			var project = projectController.CurrentProject ?? projectController.SelectedProjects.ToList()[0];
			var projectFiles = project.GetTargetLanguageFiles();
			var unParsedProjectFiles = new List<ProjectFile>();

			foreach (var file in projectFiles)
			{
				if (TaskFiles.GetIds().Contains(file.Id))
				{
					continue;
				}

				unParsedProjectFiles.Add(file);
			}

			var editor = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocs = editor.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				Application.Current.Dispatcher.Invoke(() => { editor.Close(activeDoc); });
			}

			foreach (var file in unParsedProjectFiles)
			{
				var converter = DefaultFileTypeManager.CreateInstance(true).GetConverterToDefaultBilingual(file.LocalFilePath, file.LocalFilePath, null);
				var contentProcessor = new DecryptDataProcessor(_settings);
				converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter.Parse();
			}
		}

		public override void TaskComplete()
		{
			base.TaskComplete();

			_settings.HasBeenCheckedByControl = false;
		}
	}
}
