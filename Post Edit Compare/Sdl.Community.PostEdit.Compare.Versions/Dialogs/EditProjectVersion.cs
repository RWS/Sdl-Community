using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Versions.Structures;

namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    public partial class EditProjectVersion : Form
    {
        public bool Saved { get; set; }
        public ProjectVersion ProjectVersion { get; set; }
        

        public EditProjectVersion()
        {
            InitializeComponent();
        }

        private void EditProjectVersion_Load(object sender, EventArgs e)
        {
            textBox_name.Text = ProjectVersion.name;
            textBox_location.Text = ProjectVersion.location;
            textBox_description.Text = ProjectVersion.description;
            textBox_createdAt.Text = ProjectVersion.createdAt;
        
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            button_save.Enabled = textBox_name.Text.Trim() != string.Empty;
        }

        private void linkLabel_viewFoldersInWindowsExplorer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var sPath = textBox_location.Text;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }
                if (Directory.Exists(sPath))
                {
                    Process.Start(sPath);
                }
                else
                {
                    MessageBox.Show(this, PluginResources.Invalid_directory, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Saved = true;
            ProjectVersion.name = textBox_name.Text;
            ProjectVersion.description = textBox_description.Text;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close(); 
        }
    }
}
