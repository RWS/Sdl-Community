using System;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Compare.Properties;


namespace PostEdit.Compare.Forms
{
    public partial class DeleteFiles : Form
    {

        public bool Saved { get; set; }


        public DeleteFiles()
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

        private bool IsInitializing { get; set; }
        private void ManipulateFiles_Load(object sender, EventArgs e)
        {
            try
            {
                IsInitializing = true;

                var fileCountLeft = Convert.ToInt32(label_fileCount_left.Text);
                var fileCountRight = Convert.ToInt32(label_fileCount_right.Text);

                if (fileCountLeft == 0
                    && fileCountRight == 0)
                {
                    checkBox_selected_left_side.Checked = false;
                    checkBox_selected_right_side.Checked = false;

                    checkBox_selected_left_side.Enabled = false;
                    checkBox_selected_right_side.Enabled = false;

                    button_save.Enabled = false;
                }
                else if (fileCountLeft > 0
                    && fileCountRight > 0)
                {
                    if (fileCountLeft > fileCountRight)
                    {
                        checkBox_selected_left_side.Checked = true;
                        checkBox_selected_right_side.Checked = false;
                    }
                    else
                    {
                        checkBox_selected_left_side.Checked = true;
                        checkBox_selected_right_side.Checked = false;
                    }
                }
                else if (fileCountLeft > 0)
                {
                    checkBox_selected_right_side.Enabled = false;
                    checkBox_selected_right_side.Checked = false;

                    checkBox_selected_left_side.Checked = true;
                  
                }
                else
                {
                    checkBox_selected_left_side.Enabled = false;
                    checkBox_selected_left_side.Checked = false;

                    checkBox_selected_right_side.Checked = true;
                }

                UpdatedObjects();
            }
            finally
            {
                IsInitializing = false;
            }
        }
        private void UpdatedObjects()
        {
            if (checkBox_selected_left_side.Checked && checkBox_selected_right_side.Checked)
            {
                label_scope_message.Text = Resources.DeleteFiles_UpdatedObjects_Delete_selected_files_from_both_the_left_and_right_sides;
            }
            else if (checkBox_selected_left_side.Checked)
            {
               
                label_scope_message.Text = Resources.DeleteFiles_UpdatedObjects_Delete_selected_files_from_the_left_side;
            }
            else if (checkBox_selected_right_side.Checked)
            {
                label_scope_message.Text = Resources.DeleteFiles_UpdatedObjects_Delete_selected_files_from_the_right_side;

            }
            
        }

        private void checkBox_selected_left_side_CheckedChanged(object sender, EventArgs e)
        {
            if (IsInitializing) return;
            if (!checkBox_selected_right_side.Checked && !checkBox_selected_left_side.Checked)
                checkBox_selected_right_side.Checked = true;
            UpdatedObjects();
        }

        private void checkBox_selected_right_side_CheckedChanged(object sender, EventArgs e)
        {
            if (IsInitializing) return;
            if (!checkBox_selected_right_side.Checked && !checkBox_selected_left_side.Checked)
                checkBox_selected_left_side.Checked = true;

            UpdatedObjects();
        }
    }
}
