using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace PostEdit.Compare.Forms
{
    public partial class ComparisonProject : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }

        public Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject { get; set; }

        public ComparisonProject()
        {
            InitializeComponent();
        }

        private void ComparisonListPair_Load(object sender, EventArgs e)
        {
            textBox_name.Text = comparisonProject.Name;
            textBox_created.Text = comparisonProject.Created;
            textBox_path_left.Text = comparisonProject.PathLeft;
            textBox_path_right.Text = comparisonProject.PathRight;

            CheckEnableButtons();
        }


        private void CheckEnableButtons()
        {
            var enabled = true;
            if (textBox_name.Text.Trim() == string.Empty)
                enabled = false;
            else if (textBox_path_left.Text.Trim() == string.Empty
                || !Directory.Exists(textBox_path_left.Text.Trim()))
                enabled = false;
            else if (textBox_path_right.Text.Trim() == string.Empty
               || !Directory.Exists(textBox_path_right.Text.Trim()))
                enabled = false;

            button_save.Enabled = enabled;

        }
        private void button_save_Click(object sender, EventArgs e)
        {

            comparisonProject.Name = textBox_name.Text;
            comparisonProject.Created = textBox_created.Text;
            comparisonProject.PathLeft = textBox_path_left.Text;
            comparisonProject.PathRight = textBox_path_right.Text;

            Saved = true;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_browse_path_left_Click(object sender, EventArgs e)
        {
            BrowseFolder(true);
        }

        private void button_browse_path_right_Click(object sender, EventArgs e)
        {
            BrowseFolder(false);
        }
        private void BrowseFolder(bool leftSide)
        {
            try
            {
                var sPath = (leftSide ? this.textBox_path_left.Text : this.textBox_path_right.Text);

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
                    Title = "Select base folder ( " + (leftSide ? "left" : "right") + " side )",
                    InitialDirectory = sPath
                };
                if (!fsd.ShowDialog(IntPtr.Zero)) return;
                if (fsd.FileName.Trim() == string.Empty) return;
                sPath = fsd.FileName;

                if (leftSide)
                    textBox_path_left.Text = sPath;
                else
                    textBox_path_right.Text = sPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                CheckEnableButtons();
            }
        }

        private void ComparisonListPair_DragDrop(object sender, DragEventArgs e)
        {
            try
            {

                var directoryList = (string[])e.Data.GetData(DataFormats.FileDrop, false);


                if (directoryList.Count() == 2
                    && Directory.Exists(directoryList[0]) && Directory.Exists(directoryList[1]))
                {
                    textBox_path_left.Text = directoryList[0];
                    textBox_path_right.Text = directoryList[1];
                }
                else if (Directory.Exists(directoryList[0]))
                {
                    textBox_path_left.Text = directoryList[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                CheckEnableButtons();
            }
        }

        private void ComparisonListPair_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_path_left_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_path_left_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var directoryList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                textBox_path_left.Text = directoryList[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                CheckEnableButtons();
            }
        }

        private void textBox_path_right_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_path_right_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var directoryList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                textBox_path_right.Text = directoryList[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                CheckEnableButtons();
            }
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            CheckEnableButtons();
        }

        private void textBox_path_left_TextChanged(object sender, EventArgs e)
        {
            CheckEnableButtons();
        }

        private void textBox_path_right_TextChanged(object sender, EventArgs e)
        {
            CheckEnableButtons();
        }

        private void linkLabel_fileAlignment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {


            var f = new ComparisonProjectFileAlignment
            {
                ComparisonProject = (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject) comparisonProject.Clone()
            };

            f.ShowDialog();

            if (f.Saved)
            {
                comparisonProject.FileAlignment = f.ComparisonProject.FileAlignment;   
            }

        }
    }
}
