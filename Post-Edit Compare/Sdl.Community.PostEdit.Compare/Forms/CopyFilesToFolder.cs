using System;
using System.Windows.Forms;

namespace PostEdit.Compare.Forms
{
    public partial class CopyFilesToFolder : Form
    {

        public bool Saved { get; set; }


        public CopyFilesToFolder()
        {
            InitializeComponent();
            Saved = false;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }

        private void ManipulateFiles_Load(object sender, EventArgs e)
        {

            var fileCountLeft = Convert.ToInt32(label_fileCount_left.Text);
            var fileCountRight = Convert.ToInt32(label_fileCount_right.Text);

            if (fileCountLeft == 0
                && fileCountRight == 0)
            {
                radioButton_left_side.Checked = false;
                radioButton_right_side.Checked = false;

                radioButton_left_side.Enabled = false;
                radioButton_right_side.Enabled = false;

                button_save.Enabled = false;
            }
            else if (fileCountLeft > 0
                && fileCountRight > 0)
            {
                if (fileCountLeft > fileCountRight)
                {
                    radioButton_left_side.Checked = true;
                    radioButton_right_side.Checked = false;
                }
                else
                {
                    radioButton_right_side.Checked = true;
                    radioButton_left_side.Checked = false;
                }
            }
            else if (fileCountLeft > 0)
            {
                radioButton_right_side.Enabled = false;
                radioButton_left_side.Checked = true;
                radioButton_right_side.Checked = false;
            }
            else
            {
                radioButton_left_side.Enabled = false;
                radioButton_right_side.Checked = true;
                radioButton_left_side.Checked = false;
            }


            UpdatedObjects();
        }
        private void UpdatedObjects()
        {
            if (radioButton_left_side.Checked)
            {
               
                label_scope_message.Text = "Copy selected files from the left side to the specified folder and "
                    + (checkBox_overwirte_existing_files.Checked ? "overwrite" : "do not overwrite") + " files if they already exist!";
            }
            else
            {
                label_scope_message.Text = "Copy selected files from the right side to the specified folder and "
                 + (checkBox_overwirte_existing_files.Checked ? "overwrite" : "do not overwrite") + " files if they already exist!";
              
            }
        }

        private void checkBox_overwirte_existing_files_CheckedChanged(object sender, EventArgs e)
        {
            UpdatedObjects();
        }

        private void radioButton_right_side_CheckedChanged(object sender, EventArgs e)
        {
            UpdatedObjects();
        }

        private void radioButton_left_side_CheckedChanged(object sender, EventArgs e)
        {
            UpdatedObjects();
        }

   
    }
}
