using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Core.Settings;
using Sdl.Community.TQA.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.Community.TQA.BatchTask;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Internal;


namespace Sdl.Community.TQA
{
	public partial class MainWindow : Form
	{

		private FileBasedProject CurrentProject { get; }
		private TQAReportingTask ReportingTask { get; }


		public MainWindow(ProjectsController controller)
		{
			InitializeComponent();
			CurrentProject = controller.CurrentProject;
			ReportingTask = new TQAReportingTask(CurrentProject);
			SetupWindow();
			SetupTQA();
		}

		private void SetupWindow()
		{
			txtProjectName.Text = PluginResources.NoActiveProjectSelected;
			txtStandardUsed.Text = PluginResources.TQAProfileStandardEmpty;
			StartButton.Enabled = false;
			//	LanguageSelector.Enabled = false;
		}
		private void SetupTQA()
		{
			if (CurrentProject != null)
			{
				var selectedProjectInfo = CurrentProject.GetProjectInfo();
				SetupProjectInfo(selectedProjectInfo);
				SetupTargetLanguages(selectedProjectInfo);
				SetupTQAStandardProfile();
				SetupReportOutputFile();
#warning 'Used in debug to see the objects available. It should be removed at the end'
				//AddEventFlags();
			}
		}

		//private void AddEventFlags()
		//{
		//	var filesController = SdlTradosStudio.Application.GetController<FilesController>();
		//	var editorController = SdlTradosStudio.Application.GetController<EditorController>();
		//	var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
		//	//filesController.CurrentSelectedLanguage
		//	//editorController.ActivationChanged
		//}

		private void SetupProjectInfo(ProjectInfo projectInfo)
		{
			var currentProjectName = projectInfo.Name;
			txtProjectName.Text = string.Format(PluginResources.CurrentlyWorkingOnProject, currentProjectName);
		}

		private void SetupTargetLanguages(ProjectInfo projectInfo)
		{
			object[] targetLanguages = projectInfo.TargetLanguages.Select(l => l.DisplayName).ToArray();
			LanguageSelector.Items.AddRange(targetLanguages);
			if (targetLanguages.Length > 0)
				LanguageSelector.SelectedItem = targetLanguages[0];
		}


		private void SetupTQAStandardProfile()
		{
			SetupStandardProfileUIControls();
		}

		private void SetupStandardProfileUIControls()
		{
			txtStandardUsed.Text = ReportingTask.GetCurrentTQStandardDescription();
			StartButton.Enabled = ReportingTask.CurrentStandardTypeIsSetAndSupported();
			SetupQualityCombobox();
		}

		private void SetupReportOutputFile()
		{
			outputSaveDialog.FileName = ReportingTask.GetReportOutputFile(string.Empty);
			outputSaveDialog.InitialDirectory = !string.IsNullOrEmpty(Path.GetDirectoryName(outputSaveDialog.FileName)) ? Path.GetDirectoryName(outputSaveDialog.FileName) : Path.GetTempPath();
		}

		private void SetupQualityCombobox()
		{
			QualityCombo.Items.Clear();
			QualityCombo.Items.AddRange(ReportingTask.GetQualitiesForTQAStandard().ToArray());
			if (QualityCombo.Items.Count > 0)
			{
				var tqaSettingQuality = ReportingTask.GetTQAQualityFromBatchTaskSettings();
				if (string.IsNullOrEmpty(tqaSettingQuality))
					QualityCombo.SelectedItem = (string)QualityCombo.Items[0];
				else
					QualityCombo.SelectedItem = tqaSettingQuality;

			}

		}

		private void RunTQAReport_ButtonClick(object sender, EventArgs e)
		{
			if (CurrentProject != null)
			{
				if (outputSaveDialog.ShowDialog() == DialogResult.OK)
				{
					//aici trebuie chemat TQAReportingTask care va chema TQAReportGenerationService
					var filesToBeProcessed = CurrentProject
						.GetTargetLanguageFiles(CurrentProject.GetProjectInfo().TargetLanguages
							.Single(l => l.DisplayName == LanguageSelector.SelectedItem.ToString())).ToList();
					ReportingTask.UserReportFileName = outputSaveDialog.FileName;
					ReportingTask.ExecuteTQATaskForProjectFiles(filesToBeProcessed, QualityCombo.SelectedItem.ToString());
				}
				else
				{
					MessageBox.Show(PluginResources.MsgTQAOperationAborted, PluginResources.MsgAborted,
						MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					Close();
				}
			}
			else
			{
				MessageBox.Show(PluginResources.MsgActivateOneProject, string.Empty, MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				Close();
			}

		}

	}

}

