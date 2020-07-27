﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
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
		private bool _restOfFilesParsed;
		private AnonymizerSettings _settings;

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
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

			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var selectedPatternsFromGrid = _settings.RegexPatterns.Where(e => e.ShouldEnable).ToList();
			if (projectController.CurrentProject != null)
			{
				ProjectBackup.CreateProjectBackup(projectController.CurrentProject.FilePath);
			}

			var key = _settings.EncryptionKey == "<dummy-encryption-key>" ? "" : AnonymizeData.DecryptData(_settings.EncryptionKey, Constants.Key);
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerPreProcessor(selectedPatternsFromGrid, key, _settings.EncryptionState.HasFlag(State.PatternsEncrypted))));

			ParseRestOfFiles(projectController, selectedPatternsFromGrid, key);
		}

		protected override void OnInitializeTask()
		{
			_settings = GetSetting<AnonymizerSettings>();
		}

		private static void CloseOpenDocuments()
		{
			var editor = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocs = editor.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				Application.Current.Dispatcher.Invoke(() => { editor.Close(activeDoc); });
			}
		}

		private List<ProjectFile> GetUnparsedFiles(ProjectsController projectController)
		{
			var project = projectController.CurrentProject ?? projectController.SelectedProjects.ToList()[0];
			var projectFiles = project.GetTargetLanguageFiles();

			var taskFilesIds = TaskFiles.GetIds();
			var unparsedFiles = projectFiles.Where(file => !taskFilesIds.Contains(file.Id)).ToList();

			return unparsedFiles;
		}

		private void ParseRestOfFiles(ProjectsController projectController, List<RegexPattern> selectedPatternsFromGrid, string key)
		{
			if (_restOfFilesParsed) return;

			var unParsedProjectFiles = GetUnparsedFiles(projectController);

			CloseOpenDocuments();

			foreach (var file in unParsedProjectFiles)
			{
				var converter = DefaultFileTypeManager.CreateInstance(true)
					.GetConverterToDefaultBilingual(file.LocalFilePath, file.LocalFilePath, null);
				var contentProcessor = new AnonymizerPreProcessor(selectedPatternsFromGrid, key,
					_settings.EncryptionState.HasFlag(State.PatternsEncrypted));

				converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter.Parse();
			}

			_restOfFilesParsed = true;
		}
	}
}