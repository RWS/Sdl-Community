using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Models;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Community.projectAnonymizer.Ui;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Constants = Sdl.Community.projectAnonymizer.Helpers.Constants;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			CreateAcceptFile();
			var acceptWindow = new AcceptWindow();
			if (!AgreementMethods.UserAgreed())
			{
				acceptWindow.ShowDialog();
			}
		}

		private void CreateAcceptFile()
		{
			if (!Directory.Exists(Constants.AcceptFolderPath))
			{
				Directory.CreateDirectory(Constants.AcceptFolderPath);
			}

			if (File.Exists(Constants.AcceptFilePath)) return;
			var file = File.Create(Constants.AcceptFilePath);
			file.Close();
			var accept = new Agreement
			{
				Accept = false
			};
			File.WriteAllText(Constants.AcceptFilePath, JsonConvert.SerializeObject(accept));
		}
	}

	[AutomaticTask("Anonymizer Task",
				   "Protect Data",
				   "Protect data during the project, with or without encryption",
				   GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(AnonymizerSettings), typeof(AnonymizerSettingsPage))]
	public class AnonymizerTask : AbstractFileContentProcessingAutomaticTask
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

		private void ParseRestOfFiles(ProjectsController projectController, List<RegexPattern> selectedPatternsFromGrid, string key)
		{
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
		}

		private List<ProjectFile> GetUnparsedFiles(ProjectsController projectController)
		{
			var projectFiles = projectController.CurrentProject.GetTargetLanguageFiles();
			var unParsedProjectFiles = new List<ProjectFile>();

			foreach (var file in projectFiles)
			{
				if (TaskFiles.GetIds().Contains(file.Id))
				{
					continue;
				}
				unParsedProjectFiles.Add(file);
			}

			return unParsedProjectFiles;
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
	}

	//Decrypt  task
	[AutomaticTask("Decrypt Task",
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
			if (!_settings.ShouldDeanonymize ?? false)
				return;

			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new DecryptDataProcessor(_settings)));

			var projectFiles = projectController.CurrentProject.GetTargetLanguageFiles();
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