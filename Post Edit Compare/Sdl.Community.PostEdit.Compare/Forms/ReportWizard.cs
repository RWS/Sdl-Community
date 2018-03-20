using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PostEdit.Compare.Forms
{
    public partial class ReportWizard : Form
    {
        private int _indexCurrentPanel;
		public bool IsFromProjectsViewCall { get; set; }
		public bool Saved { get; set; }

        public ReportWizard()
        {
            InitializeComponent();
            Saved = false;
			IsFromProjectsViewCall = false;

		}

        private void ReportWizard_Load(object sender, EventArgs e)
        {          
		
            panel_action.Dock = DockStyle.Fill;
            panel_options.Dock = DockStyle.Fill;
			if (IsFromProjectsViewCall)
			{
				switch_Panel(panel_options);
			}
			else
			{
				switch_Panel(panel_action);
			}
            

            radioButton_compareSelectedFiles_CheckedChanged(null, null);

            textBox_javaExecutablePath_TextChanged(null, null);
        }

        public void switch_Panel(Control panel)
        {          
            switch (panel.Name)
            {
                case "panel_action":
                    {
                        label_titleBar_title.Text = "Report Actions - Step 1 of 2";
                        label_titlebar_description.Text = "Select the report action and optionally choose the rate group";
                        label_titlebar_note.Text = "";

                        panel_action.BringToFront(); 

                        button_wizard_help.Enabled = false;

                        button_wizard_back.Enabled = false;
                        button_wizard_next.Enabled = true;

                        button_wizard_finish.Enabled = true;
                        button_wizard_cancel.Enabled = true;
                        break;
                    }
                case "panel_options":
                    {
						if (IsFromProjectsViewCall)
						{
							label_titleBar_title.Text = "Report Options - Step 1 of 1";
							button_wizard_back.Enabled = false;
						}
						else
						{
							label_titleBar_title.Text = "Report Options - Step 2 of 2";
							button_wizard_back.Enabled = true;
						}
                        
                        label_titlebar_description.Text = "Choose the report options";
                        label_titlebar_note.Text = "Note: changing the settings here will override the general report settings that are saved with the application";

                        panel_options.BringToFront();

                        button_wizard_help.Enabled = false;
						                       
                        button_wizard_next.Enabled = false;

                        button_wizard_finish.Enabled = true;
                        button_wizard_cancel.Enabled = true;

                        break;
                    }
            }
        }

        private void button_wizard_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_wizard_finish_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }

        private void button_wizard_next_Click(object sender, EventArgs e)
        {
            if (_indexCurrentPanel == 0)
            {
                _indexCurrentPanel++;
            }

            switch_Panel(_indexCurrentPanel == 0 ? panel_action : panel_options);
        }

        private void button_wizard_back_Click(object sender, EventArgs e)
        {
            if (_indexCurrentPanel == 1)
            {
                _indexCurrentPanel--;
            }

            switch_Panel(_indexCurrentPanel == 0 ? panel_action : panel_options);
        }

        private void button_wizard_help_Click(object sender, EventArgs e)
        {

        }

        private void radioButton_compareSelectedFiles_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_includeAllSubfolders.Enabled = !radioButton_compareSelectedFiles.Checked;
        }

        private void radioButton_compareAllFiles_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_includeAllSubfolders.Enabled = !radioButton_compareSelectedFiles.Checked;
        }








        private void textBox_javaExecutablePath_TextChanged(object sender, EventArgs e)
        {
            if (textBox_javaExecutablePath.Text.Trim() != string.Empty
                         && File.Exists(textBox_javaExecutablePath.Text))
            {
                checkBox_showSegmentTERPAnalysis.Enabled = true;
            }
            else
            {
                checkBox_showSegmentTERPAnalysis.Enabled = false;
                checkBox_showSegmentTERPAnalysis.Checked = false;
            }
        }

        private void button_javaExecutablePath_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog
            {
                Title = "Select the Java executable file",
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "Java executable file (java.exe)|java.exe",
                FilterIndex = 0
            };

            if (textBox_javaExecutablePath.Text.Trim() != string.Empty)
            {
                var dir = Path.GetDirectoryName(textBox_javaExecutablePath.Text);
                if (Directory.Exists(dir))
                    f.InitialDirectory = dir;
            }

            var dr = f.ShowDialog();
            if (dr != DialogResult.OK)
                return;
            try
            {
                if (f.FileName != null && f.FileName.Trim() != string.Empty)
                    textBox_javaExecutablePath.Text = f.FileName;

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void javaWebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.oracle.com/technetwork/java/javase/downloads/index.html");
        }
    }
}
