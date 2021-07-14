using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TQA
{
    public partial class MainWindow : Form
    {
	    private readonly FileBasedProject _currentProject;

	    public MainWindow(ProjectsController controller)
        {
	        InitializeComponent();
	        _currentProject = controller.CurrentProject;
			if (_currentProject != null)
			{
				var selectedProjectInfo = _currentProject.GetProjectInfo();
		        var currentProjectName = selectedProjectInfo.Name;
		        ProjectNameLabel.Text = $"Currently working on: {currentProjectName}";
		        outputSaveDialog.FileName = "SDL-5-401-F001 Quality Evaluation Form_XXX_XXXXXX_XXX_XX";

		        var targetLanguages = selectedProjectInfo.TargetLanguages.Select(l => l.DisplayName).ToArray();
		        LanguageSelector.Items.AddRange(targetLanguages);
		        LanguageSelector.SelectedItem = targetLanguages[0];
		        QualityCombo.SelectedItem = (string) QualityCombo.Items[0];
	        }
	        else
	        {
		        ProjectNameLabel.Text = @"No active project";
	        }
	       
		}

        private void StartButton_Click(object sender, EventArgs e)
        {
	        if (_currentProject != null)
	        {
		        SaveActiveFile();

		        var tempPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
		        var tqaTask = _currentProject.RunAutomaticTask(_currentProject.GetTargetLanguageFiles(_currentProject.GetProjectInfo().TargetLanguages.Single(l => l.DisplayName == LanguageSelector.SelectedItem.ToString())).Select(f => f.Id).ToArray(), "Sdl.ProjectApi.AutomaticTasks.Feedback");

		        _currentProject.SaveTaskReportAs(tqaTask.Reports[0].Id, tempPath, ReportFormat.Xml);

		        var extractedData = DataConverter.ExtractFromXml(tempPath, (string)QualityCombo.SelectedItem);

		        if (outputSaveDialog.ShowDialog() == DialogResult.OK)
		        {
			        try
			        {
				        DataConverter.WriteExcel(outputSaveDialog.FileName, extractedData);
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
			        MessageBox.Show("TQA completed. Please find your report here: \n " + outputSaveDialog.FileName, @"Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}

