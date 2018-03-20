﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TQA
{
    public partial class MainWindow : Form
    {
        private readonly ProjectsController _controller;

	    public MainWindow(ProjectsController controller)
        {
	        InitializeComponent();
            _controller = controller;

			if (_controller.CurrentProject != null)
	        {
		        var selectedProjectInfo = _controller.CurrentProject.GetProjectInfo();
				var currentProjectName = selectedProjectInfo.Name;
				ProjectNameLabel.Text = string.Format( "Currently working on: {0}",currentProjectName );
		        outputSaveDialog.FileName = "QRF-121_XXX_XXXXXX_XXX_XX";
				LanguageSelector.Items.AddRange(selectedProjectInfo.TargetLanguages.Select( l => l.DisplayName ).ToArray() );
	        }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            var tempPath = Path.Combine( Path.GetTempPath(), Path.GetTempFileName() );
            var tqaTask = _controller.CurrentProject.RunAutomaticTask( _controller.CurrentProject.GetTargetLanguageFiles( _controller.CurrentProject.GetProjectInfo().TargetLanguages.Single( l => l.DisplayName == LanguageSelector.SelectedItem.ToString() ) ).Select( f => f.Id ).ToArray(), "Sdl.ProjectApi.AutomaticTasks.Feedback" );

            _controller.CurrentProject.SaveTaskReportAs( tqaTask.Reports[0].Id, tempPath, ReportFormat.Xml );

	        var extractedData = DataConverter.ExtractFromXml(tempPath);

            if( outputSaveDialog.ShowDialog() == DialogResult.OK )
            {
				try
                {
                    DataConverter.WriteExcel( outputSaveDialog.FileName, extractedData );
                }
                catch(IOException  ex )
                {
                    MessageBox.Show( "Unable to open file:\n" + outputSaveDialog.FileName+"\n\n Please check if it not opened in another application and try again." );
                    return;
                }
                catch( Exception ex )
                {
                    MessageBox.Show( "Something went terribly wrong. Please send screenshot of this window to kpeka@sdl.com\n\n\n" + ex);
                    return;
                }
                MessageBox.Show( "TQA completed. Please find your report here: \n "+outputSaveDialog.FileName, @"Done", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                MessageBox.Show( @"Operation terminated by user!", @"Aborted", MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
            }
            Close();
        }
    }
}

