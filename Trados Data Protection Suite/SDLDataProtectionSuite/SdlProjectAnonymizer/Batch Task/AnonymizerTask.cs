using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Constants = Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.Constants;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	[AutomaticTask("SDPSAnonymizerTask",
				   "Protect Data",
				   "Protect data during the project, with or without encryption",
				   GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(AnonymizerSettings), typeof(AnonymizerSettingsPage))]
	public class AnonymizerTask : AbstractFileContentProcessingAutomaticTask
	{
		private List<string> _ignoredFiles;
		private RestOfFilesParser _restOfFilesParser;
		private AnonymizerSettings _settings;

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}

		public override void TaskComplete()
		{
			if (_ignoredFiles?.Count > 0)
			{
				MessageBox.Show(string.Format(PluginResources.FilesIgnoredByParser, string.Join(Environment.NewLine, _ignoredFiles.ToArray())), PluginResources.SDLDataProtectionSuite);
			}
			base.TaskComplete();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			if (!AgreementMethods.UserAgreed())
			{
				CancelHelper.CancelTaskIfRequested(true);
			}

			if (!_settings.ShouldAnonymize ?? false)
			{
				return;
			}

			var selectedPatternsFromGrid = _settings.RegexPatterns.Where(e => e.ShouldEnable).ToList();

			var key = _settings.EncryptionKey == "<dummy-encryption-key>" ? "" : AnonymizeData.DecryptData(_settings.EncryptionKey, Constants.Key);
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerPreProcessor(selectedPatternsFromGrid, key, _settings.EncryptionState.HasFlag(State.PatternsEncrypted))));

			_restOfFilesParser.ParseRestOfFiles(
				Project,
				TaskFiles,
				new AnonymizerPreProcessor(selectedPatternsFromGrid, key, _settings.EncryptionState.HasFlag(State.PatternsEncrypted)),
				out _ignoredFiles);
		}

		protected override void OnInitializeTask()
		{
			ProjectBackup.CreateProjectBackup((Project as FileBasedProject)?.FilePath);

			_restOfFilesParser = new RestOfFilesParser();
			_settings = GetSetting<AnonymizerSettings>();
		}
	}
}