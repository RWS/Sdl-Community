using System;
using System.Collections.Generic;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
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
		private readonly RestOfFilesParser _restOfFilesParser = new RestOfFilesParser();
		private List<string> _ignoredFiles;
		private AnonymizerSettings _settings;

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}

		public override void TaskComplete()
		{
			if (_ignoredFiles.Count > 0)
			{
				MessageBox.Show(string.Format(PluginResources.FilesIgnoredByParser, string.Join(Environment.NewLine, _ignoredFiles.ToArray())), PluginResources.IgnoredFiles);
			}
			base.TaskComplete();
			_settings.HasBeenCheckedByControl = false;
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

			_restOfFilesParser.ParseRestOfFiles(projectController, TaskFiles, new DecryptDataProcessor(_settings), out _ignoredFiles);
		}

		protected override void OnInitializeTask()
		{
			_settings = GetSetting<AnonymizerSettings>();
		}
	}
}