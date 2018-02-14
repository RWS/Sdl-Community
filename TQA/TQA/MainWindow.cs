using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;


namespace TQA
{
    public partial class MainWindow : Form
    {
        ProjectsController Controller;
        public MainWindow(ProjectsController Controller)
        {
            InitializeComponent();
            this.Controller = Controller;

            var test = Controller.CurrentProject.GetProjectInfo().TargetLanguages;
            this.ProjectNameLabel.Text = String.Format( "Currently working on: {0}", Controller.CurrentProject.GetProjectInfo().Name );
            LanguageSelector.Items.AddRange( Controller.CurrentProject.GetProjectInfo().TargetLanguages.Select( l => l.DisplayName ).ToArray() );
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string tempPath = Path.Combine( Path.GetTempPath(), Path.GetTempFileName() );

            var TqaTask = Controller.CurrentProject.RunAutomaticTask( Controller.CurrentProject.GetTargetLanguageFiles( Controller.CurrentProject.GetProjectInfo().TargetLanguages.Single( l => l.DisplayName == LanguageSelector.SelectedItem.ToString() ) ).Select( f => f.Id ).ToArray(), "Sdl.ProjectApi.AutomaticTasks.Feedback" );

            Controller.CurrentProject.SaveTaskReportAs( TqaTask.Reports[0].Id, tempPath, Sdl.ProjectAutomation.Core.ReportFormat.Xml );

            var extractedData = DataConverter.ExtractFromXml( tempPath ).ToArray();

            if( outputSaveDialog.ShowDialog() == DialogResult.OK )
            {
                try
                {
                    DataConverter.WriteExcel( outputSaveDialog.FileName, extractedData );
                }
                catch(IOException  ex )
                {
                    MessageBox.Show( "Unable to open file:\n" + outputSaveDialog.FileName+"\n\nPlease check if it not opened in another application and try again." );
                    return;
                }
                catch( Exception ex )
                {
                    MessageBox.Show( "Something went terribly wrong. Please send screenshot of this window to kpeka@sdl.com\n\n\n" + ex.ToString() );
                    return;
                }
                MessageBox.Show( "TQA completed. Please find your report here:\n"+outputSaveDialog.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information );

            }
            else
            {
                MessageBox.Show( "Operation terminated by user!", "Aborted", MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
            }

            this.Close();
        }
    }
}

