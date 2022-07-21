using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Core.Settings;
using Sdl.Community.TQA.Model;
using System.Collections.Generic;


namespace Sdl.Community.TQA
{
	public partial class MainWindow : Form
	{

		private FileBasedProject _currentProject;
		private AssessmentCategories TQACategories { get; set; }
		private TQStandardType CurrentTQStandardType { get; set; }

		public MainWindow(ProjectsController controller)
		{
			InitializeComponent();
			SetupWindow();
			SetupTQA(controller);
		}

		private void SetupWindow()
		{
			txtProjectName.Text = PluginResources.NoActiveProjectSelected;
			txtStandardUsed.Text = PluginResources.TQAProfileStandardEmpty;
			StartButton.Enabled = false;
		}
		private void SetupTQA(ProjectsController controller)
		{
			_currentProject = controller.CurrentProject;
			if (_currentProject != null)
			{
				var selectedProjectInfo = _currentProject.GetProjectInfo();
				SetupProjectInfo(selectedProjectInfo);
				SetupTargetLanguages(selectedProjectInfo);
				SetupTQAStandard(selectedProjectInfo);
				SetupReportOutputFile();
			}
		}

		private void SetupTargetLanguages(ProjectAutomation.Core.ProjectInfo projectInfo)
		{
			var targetLanguages = projectInfo.TargetLanguages.Select(l => l.DisplayName).ToArray();
			LanguageSelector.Items.AddRange(targetLanguages);
			LanguageSelector.SelectedItem = targetLanguages[0];
		}

		private void SetupProjectInfo(ProjectAutomation.Core.ProjectInfo projectInfo)
		{
			var currentProjectName = projectInfo.Name;
			txtProjectName.Text = string.Format(PluginResources.CurrentlyWorkingOnProject, currentProjectName);
		}

		private void SetupReportOutputFile()
		{
			outputSaveDialog.FileName = TQStandardsFactory.GetReportOutputFilenameForTQStandard(CurrentTQStandardType);
		}
		private void StartButton_Click(object sender, EventArgs e)
		{
			if (_currentProject != null)
			{
				SaveActiveFile();

				var tempPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
				var tqaTask = _currentProject.RunAutomaticTask(_currentProject.GetTargetLanguageFiles(_currentProject.GetProjectInfo().TargetLanguages.Single(l => l.DisplayName == LanguageSelector.SelectedItem.ToString())).Select(f => f.Id).ToArray(), "Sdl.ProjectApi.AutomaticTasks.Feedback");

				_currentProject.SaveTaskReportAs(tqaTask.Reports[0].Id, tempPath, Sdl.ProjectAutomation.Core.ReportFormat.Xml);

				var extractedData = DataConverter.ExtractFromXml(tempPath, (string)QualityCombo.SelectedItem);

				if (outputSaveDialog.ShowDialog() == DialogResult.OK)
				{
					try
					{
						DataConverter.WriteExcel(outputSaveDialog.FileName, extractedData, CurrentTQStandardType);
					}
					catch (IOException ex)
					{
						throw ex;
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString());
						return;
					}
					MessageBox.Show(PluginResources.MsgTQAProcessCompleted + outputSaveDialog.FileName, PluginResources.ReportGenerationFinished, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(@"Operation terminated by user!", @"Aborted", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				Close();
			}
			else
			{
				MessageBox.Show(@"Please activate one project", string.Empty, MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				Close();
			}
		}
		private static void SaveActiveFile()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeFile = editorController?.ActiveDocument;

			if (activeFile != null)
			{
				editorController.Save(activeFile); // if the file is not saved in the editor TQA changes does not appear in the report
			}
		}
		private void SetupTQAStandard(ProjectAutomation.Core.ProjectInfo projectInfo)
		{
			//var targetLanguage = selectedProjectInfo.TargetLanguages.Where(l => l.DisplayName == LanguageSelector.SelectedItem.ToString()).FirstOrDefault();
			ISettingsBundle settings = _currentProject.GetSettings();//later if it's needed we'll call it with targetLanguage as param. It is a request there, in this Epic, to make it to work with more target languages...
			SetupTQStandard(settings);
			SetupQualityCombobox();
		}

		private void SetupTQStandard(ISettingsBundle projectSettings)
		{
			TQACategories = GetAssessmentCategories(projectSettings);
			CurrentTQStandardType = GetTQSStandardBasedOnCurrentImportedTemplate();
			txtStandardUsed.Text = GetCurrentTQStandardDescription();
			StartButton.Enabled = CurrentStandardTypeIsSetAndSupported();
		}

		private bool CurrentStandardTypeIsSetAndSupported()
		{
			return CurrentTQStandardType == TQStandardType.tqsJ2450 || CurrentTQStandardType == TQStandardType.tqsMQM;
		}

		private void SetupQualityCombobox()
		{
			QualityCombo.Items.Clear();
			QualityCombo.Items.AddRange(GetQualitiesForTQAStandard().ToArray());
			if (QualityCombo.Items.Count>0)
		    	QualityCombo.SelectedItem = (string)QualityCombo.Items[0];
		}

		private List<string> GetQualitiesForTQAStandard()
		{
			return TQStandardsFactory.GetTQSQualities(CurrentTQStandardType);
		}

		private string GetCurrentTQStandardDescription()
		{
			return TQStandardsFactory.GetCurrentTQStandardDescription(CurrentTQStandardType);
		}

		private TQStandardType GetTQSStandardBasedOnCurrentImportedTemplate()
		{

			return TQSCategories.GetStandardBasedOnCategories(TQACategories);//GetCategoriesNames()
		}

		private List<Guid> GetCategoriesIds()
		{
			return TQACategories.Select(cat => cat.Id).Distinct().ToList();
		}
		private List<string> GetCategoriesNames()
		{
			return TQACategories.Select(category => category.Name).Distinct().ToList();
		}

		private AssessmentCategories GetAssessmentCategories(ISettingsBundle settingsBundle)
		{
			return GetCategorySettings<AssessmentCategories>(settingsBundle, "AssessmentCategories");
		}
		private static T GetCategorySettings<T>(ISettingsBundle settingsBundle, string setting) where T : new()
		{
			ISettingsGroup settingsGroup = settingsBundle.GetSettingsGroup("TranslationQualityAssessmentSettings");
			if (!settingsGroup.ContainsSetting(setting))
			{
				return new T();
			}

			return settingsGroup.GetSetting<T>(setting);
		}


	}

}

