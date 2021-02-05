using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Trados.Transcreate.Common;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Actions
{
	public abstract class AbstractOpenFileInEditorAction : AbstractAction
	{
		private Controllers _controllers;

		internal void Setup()
		{
			Enabled = false;
			
			_controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			
			SetEnabled(_controllers.TranscreateController.GetSelectedProjectFiles());

			if (_controllers.ProjectsController != null)
			{
				_controllers.TranscreateController.ProjectFileSelectionChanged += TranscreateController_ProjectFileSelectionChanged;
			}
		}

		protected override void Execute()
		{
			// default editing mode
			OpenFile(EditingMode.Translation);
		}

		internal IStudioDocument OpenFile(EditingMode mode)
		{
			var selectedFiles = _controllers.TranscreateController.GetSelectedProjectFiles();
			if (selectedFiles.Count != 1)
			{
				Enabled = false;
				return null;
			}

			var project = _controllers.ProjectsController.GetAllProjects().FirstOrDefault(
				a => a.GetProjectInfo().Id.ToString() == selectedFiles[0].ProjectId);
			if (project == null)
			{
				return null;
			}

			var documents = _controllers.EditorController.GetDocuments();
			var document =
				documents?.FirstOrDefault(a => a.Files.FirstOrDefault()?.Id.ToString() == selectedFiles[0].FileId);

			if (document != null)
			{
				_controllers.EditorController.Activate();
				_controllers.EditorController.Activate(document);
				return document;
			}

			if (selectedFiles[0].Action == Enumerators.Action.Export || selectedFiles[0].Action == Enumerators.Action.ExportBackTranslation)
			{
				var activityfile = selectedFiles[0].ProjectFileActivities.OrderByDescending(a => a.Date)
					.FirstOrDefault(a => a.Action == Enumerators.Action.Export);

				var message1 = string.Format(PluginResources.Message_FileWasExportedOn, activityfile?.DateToString);
				var message2 =
					string.Format(PluginResources.Message_WarningTranslationsCanBeOverwrittenDuringImport,
						activityfile?.DateToString);
				var message3 = "Do you want to proceed?";

				var dr = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2 +
								Environment.NewLine + Environment.NewLine + message3,
					PluginResources.TranscreateManager_Name, MessageBoxButtons.YesNo,
					MessageBoxIcon.Question);

				if (dr != DialogResult.Yes)
				{
					return null;
				}

				_controllers.TranscreateController.OverrideEditorWarningMessage = true;
			}

			var projectFile = project.GetFile(Guid.Parse(selectedFiles[0].FileId));
			return _controllers.EditorController.Open(projectFile, mode);
		}

		private void SetEnabled(IReadOnlyCollection<ProjectFile> selectedFiles)
		{
			Enabled = selectedFiles?.Count == 1;
		}

		private void TranscreateController_ProjectFileSelectionChanged(object sender, CustomEventArgs.ProjectFileSelectionChangedEventArgs e)
		{
			SetEnabled(e.SelectedFiles);
		}
	}
}
