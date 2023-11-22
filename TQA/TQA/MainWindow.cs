using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Sdl.Community.TQA.Providers;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi.Internal;


namespace Sdl.Community.TQA
{
	public partial class MainWindow : Form
	{
		private readonly FileBasedProject _currentProject;
		private readonly TQAProfileType _tqaProfileType;
		private readonly ReportProvider _reportProvider;
		private readonly AbstractBindedController _controller;
		private readonly QualitiesProvider _qualitiesProvider;
		private readonly Logger _logger;

		public MainWindow(AbstractBindedController controller, ReportProvider reportProvider, CategoriesProvider categoriesProvider,
			QualitiesProvider qualitiesProvider, Logger logger)
		{
			InitializeComponent();

			_controller = controller;
			_reportProvider = reportProvider;
			_qualitiesProvider = qualitiesProvider;
			_logger = logger;

			_currentProject = GetProjectFromController() as FileBasedProject;

			var projectSettings = _currentProject?.GetSettings();

			_tqaProfileType = categoriesProvider.GetTQAProfileType(projectSettings);

			InitializeWindow();
		}

		private IProject GetProjectFromController()
		{
			if (_controller is ProjectsController projectsController)
			{
				return projectsController.CurrentProject ?? projectsController.SelectedProjects.FirstOrDefault();
			}
			if (_controller is FilesController filesController)
			{
				return filesController.CurrentProject;
			}
			if (_controller is EditorController editorController)
			{
				return editorController.ActiveDocument?.Project ?? editorController.GetDocuments()?.FirstOrDefault()?.Project;
			}

			return null;
		}

		private void InitializeWindow()
		{
			txtProjectName.Text = PluginResources.NoActiveProjectSelected;
			txtStandardUsed.Text = PluginResources.TQAProfileStandardEmpty;
			StartButton.Enabled = false;

			if (_currentProject != null)
			{
				var selectedProjectInfo = _currentProject.GetProjectInfo();
				SetupProjectInfo(selectedProjectInfo);
				SetupTargetLanguages(selectedProjectInfo);
				SetupTqaStandardProfile();
			}
		}

		private void SetupProjectInfo(ProjectInfo projectInfo)
		{
			var currentProjectName = projectInfo.Name;
			txtProjectName.Text = string.Format(PluginResources.CurrentlyWorkingOnProject, currentProjectName);
		}

		private void SetupTargetLanguages(ProjectInfo projectInfo)
		{
			LanguageSelector.Items.AddRange(projectInfo.TargetLanguages);
			SetLanguageSelection();
		}

		private void SetLanguageSelection()
		{
			if (_controller is ProjectsController projectsController)
			{
				LanguageSelector.SelectedItem = LanguageSelector.Items.Cast<Language>().FirstOrDefault();
				LanguageSelector.Enabled = true;
			}
			if (_controller is FilesController filesController)
			{
				SetSelectedLanguageItem(filesController.CurrentSelectedLanguage);
				LanguageSelector.Enabled = false;
			}
			if (_controller is EditorController editorController)
			{
				var selectedTargetLanguage = editorController.ActiveDocument?.ActiveFile?.Language ??
											 editorController.GetDocuments().FirstOrDefault()?.ActiveFile?.Language;
				SetSelectedLanguageItem(selectedTargetLanguage);
				LanguageSelector.Enabled = false;
			}
		}

		private void SetSelectedLanguageItem(Language selectedTargetLanguage)
		{
			foreach (var language in LanguageSelector.Items.Cast<Language>())
			{
				if (string.Compare(language.IsoAbbreviation,
						selectedTargetLanguage.IsoAbbreviation, StringComparison.OrdinalIgnoreCase) == 0)
				{
					LanguageSelector.SelectedItem = language;
					break;
				}
			}
		}

		private void SetupTqaStandardProfile()
		{
			txtStandardUsed.Text = PluginResources.Label_TQAProfile + " " + _reportProvider.GetProfileTypeName(_tqaProfileType);
			SetupQualityCombobox();

			IsValid();
		}

		private bool IsValid()
		{
			var isValidProfileType = _tqaProfileType == TQAProfileType.tqsJ2450 || _tqaProfileType == TQAProfileType.tqsMQM;
			var isValidFiles = GetProjectFiles().Any();

			var isValid = isValidProfileType && isValidFiles;
			StartButton.Enabled = isValid;

			return isValid;
		}

		private string GetLanguageId()
		{
			var language = LanguageSelector.SelectedItem as Language;
			return language?.CultureInfo?.Name;
		}

		private void SetupQualityCombobox()
		{
			QualityCombo.Items.Clear();
			QualityCombo.Items.AddRange(_qualitiesProvider.GetQualities(_tqaProfileType).ToArray());

			if (QualityCombo.Items.Count > 0)
			{
				QualityCombo.SelectedItem = (string)QualityCombo.Items[0];
			}
		}

		private void RunTQAReport_ButtonClick(object sender, EventArgs e)
		{
			if (_currentProject != null)
			{
				try
				{
					StartButton.Enabled = false;
					label_progress.Text = PluginResources.ProgressMessage_GeneratingTQAReport;
					Enabled = false;

					Execute();
				}
				finally
				{
					StartButton.Enabled = true;
					label_progress.Text = PluginResources.ProgressMessage_Ready;
					Enabled = true;
				}
			}
			else
			{
				MessageBox.Show(PluginResources.MsgActivateOneProject, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
				Close();
			}
		}

		private void Execute()
		{
			var projectFileIds = GetProjectFiles().Select(a => a.Id).ToArray();
			if (!projectFileIds.Any())
			{
				MessageBox.Show(PluginResources.ProgressMessage_NoProjectFilesFound, PluginResources.MsgAborted,
					MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				SaveProject();

				var localProjectFolder = _currentProject.GetProjectInfo().LocalProjectFolder;
				if (!Directory.Exists(localProjectFolder))
				{
					Directory.CreateDirectory(localProjectFolder);
				}

				var defaultReportsDirectory = _reportProvider.GetReportDirectory(localProjectFolder);
				var defaultReportFileName = _reportProvider.GetReportDefaultOutputFilename(_tqaProfileType);
				var languageId = GetLanguageId();

				var reportFilePath =
					_reportProvider.GetDefaultReportFileFullPath(defaultReportsDirectory, defaultReportFileName,
						languageId);

				outputSaveDialog.FileName = Path.GetFileName(reportFilePath);
				outputSaveDialog.InitialDirectory = defaultReportsDirectory;

				if (outputSaveDialog.ShowDialog() == DialogResult.OK)
				{
					var updatedReportFilePath = outputSaveDialog.FileName;

					_qualitiesProvider.UpdateQualityValue(_currentProject,
						QualityCombo.SelectedItem.ToString());

					var tqaAutomaticTask =
						_currentProject.RunAutomaticTask(projectFileIds, Constants.AutomatedTasks_Feedback_Id);
					if (tqaAutomaticTask == null || tqaAutomaticTask.Status != TaskStatus.Completed)
					{
						_logger.Error($"{tqaAutomaticTask?.Messages?.FirstOrDefault()?.Message}\n " +
									  PluginResources.MsgTQATaskNotRunCorrectly);
					}

					var success = _reportProvider.GenerateReport(_currentProject, tqaAutomaticTask, _tqaProfileType,
						updatedReportFilePath, QualityCombo.SelectedItem.ToString());
					if (success)
					{
						MessageBox.Show(
							string.Format(PluginResources.MsgTQAProcessCompleted,
								string.Join("\n", updatedReportFilePath)), PluginResources.ReportGenerationFinished,
							MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}
		}

		private void SaveProject()
		{
			var editorController = _controller as EditorController ?? SdlTradosStudio.Application.GetController<EditorController>();
			if (editorController != null)
			{
				foreach (var document in editorController.GetDocuments())
				{
					editorController.Save(document);
				}
			}

			_currentProject.Save();
		}

		private IEnumerable<ProjectFile> GetProjectFiles()
		{
			var projectFiles = new List<ProjectFile>();

			if (_controller is ProjectsController projectsController)
			{
				var project = projectsController.CurrentProject ?? projectsController.SelectedProjects.FirstOrDefault();
				var language = LanguageSelector.SelectedItem as Language;

				var targetProjectFiles = project.GetTargetLanguageFiles(language);
				foreach (var targetProjectFile in targetProjectFiles)
				{
					if (targetProjectFile.Role == FileRole.Translatable)
					{
						projectFiles.Add(targetProjectFile);
					}
				}
			}

			if (_controller is FilesController filesController)
			{
				foreach (var targetProjectFile in filesController.SelectedFiles)
				{
					if (targetProjectFile.Role == FileRole.Translatable)
					{
						projectFiles.Add(targetProjectFile);
					}
				}
			}

			if (_controller is EditorController editorController)
			{
				foreach (var document in editorController.GetDocuments())
				{
					foreach (var documentFile in document.Files)
					{
						projectFiles.Add(documentFile);
					}
				}
			}

			return projectFiles;
		}

		private void LanguageSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			IsValid();
		}
	}
}

