using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    public partial class DefaultSettings : Form
    {

        public bool Saved { get; set; }
        public string VersionsFolderFullPath { get; set; }
        public bool CreateSubFolderProject { get; set; }
        public bool CreateShallowCopy { get; set; }

        public DefaultSettings()
        {
            InitializeComponent();

            #region  |  initialize properties  |

            Saved = false;
            VersionsFolderFullPath = string.Empty;
            CreateSubFolderProject = true;
            CreateShallowCopy = true;

            #endregion

        }

        private void button_browseProjectVersionsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                var sPath = textBox_projectVersionsFolder.Text;

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

                var fsd = new FolderSelectDialog
                {
                    Title = "Select Project Versions Folder",
                    InitialDirectory = sPath
                };
                if (!fsd.ShowDialog(IntPtr.Zero)) 
                    return;

                if (fsd.FileName.Trim() == string.Empty) 
                    return;

                sPath = fsd.FileName;


                textBox_projectVersionsFolder.Text = sPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                textBox_projectVersionsFolder_TextChanged(null, null);
            }

        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (textBox_projectVersionsFolder.Text.Trim() != string.Empty
                && Directory.Exists(textBox_projectVersionsFolder.Text))
            {
                Saved = true;
                VersionsFolderFullPath = textBox_projectVersionsFolder.Text;
                CreateSubFolderProject = checkBox_createSubFolderProject.Checked;
                CreateShallowCopy = checkBox_createShallowCopy.Checked;

                Close();
            }
            else
            {
                MessageBox.Show(this, PluginResources.Invalid_directory, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            Saved = false;
            textBox_projectVersionsFolder.Text = VersionsFolderFullPath;
            checkBox_createSubFolderProject.Checked = CreateSubFolderProject;
            checkBox_createShallowCopy.Checked = CreateShallowCopy;

            textBox_projectVersionsFolder_TextChanged(null, null);
        }

        private void textBox_projectVersionsFolder_TextChanged(object sender, EventArgs e)
        {
            button_save.Enabled = Directory.Exists(textBox_projectVersionsFolder.Text);
        }

        private void linkLabel_viewFoldersInWindowsExplorer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var sPath = textBox_projectVersionsFolder.Text;

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
    }
}
