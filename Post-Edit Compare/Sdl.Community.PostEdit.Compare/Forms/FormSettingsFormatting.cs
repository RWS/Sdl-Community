﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Compare.Properties;


namespace PostEdit.Compare.Forms
{
    public partial class FormSettingsFormatting : Form
    {
        public bool SaveSettings { get; set; }
        private bool IsLoading { get; set; }
        public FormSettingsFormatting()
        {
            IsLoading = true;
            InitializeComponent();
            SaveSettings = false;
        }

        private void FormSettingsFormatting_Load(object sender, EventArgs e)
        {
            
            CheckEnableControls();
            
            IsLoading = false;
            UpdateVisualTextFormatting();
        }

        
        private void button_ok_Click(object sender, EventArgs e)
        {
            SaveSettings = true;
            Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CheckEnableControls()
        {
            if (checkBox_fontColor.Checked)
            {
                label_fontColorDisplay.Enabled = true;
                button_fontColorEdit.Enabled = true;
            }
            else
            {
                label_fontColorDisplay.Enabled = false;
                button_fontColorEdit.Enabled = false;
            }



            if (checkBox_backroundColor.Checked)
            {
                label_backroundColorDisplay.Enabled = true;
                button_backroundColorEdit.Enabled = true;
            }
            else
            {
                label_backroundColorDisplay.Enabled = false;
                button_backroundColorEdit.Enabled = false;
            }
        }




        private void UpdateVisualTextFormatting()
        {
            if (IsLoading) return;
            richTextBox_sample.Text = Resources.FormSettingsFormatting_updateVisualTextFormatting_This_is_some_sample_text;

            bool isBold = (string.Compare(comboBox_styleBold.SelectedItem.ToString(), "Activate", StringComparison.OrdinalIgnoreCase) == 0 ? true : false);
            bool isItalic = (string.Compare(comboBox_styleItalic.SelectedItem.ToString(), "Activate", StringComparison.OrdinalIgnoreCase) == 0 ? true : false);
            bool isStrikethrough = (string.Compare(comboBox_styleStrikethrough.SelectedItem.ToString(), "Activate", StringComparison.OrdinalIgnoreCase) == 0 ? true : false);
            bool isUnderline = (string.Compare(comboBox_styleUnderline.SelectedItem.ToString(), "Activate", StringComparison.OrdinalIgnoreCase) == 0 ? true : false);
            string strPosition = comboBox_stylePosition.Text;

            var fontStyleNew = GetFontStyle(isBold, isItalic, isStrikethrough, isUnderline);

            richTextBox_sample.Select(0, richTextBox_sample.Text.Length);
            richTextBox_sample.SelectionFont = new Font(richTextBox_sample.Font.Name, richTextBox_sample.Font.Size, fontStyleNew);

            richTextBox_sample.SelectionColor = checkBox_fontColor.Checked ? label_fontColorDisplay.BackColor : richTextBox_sample.ForeColor;

            richTextBox_sample.SelectionBackColor = checkBox_backroundColor.Checked ? label_backroundColorDisplay.BackColor : richTextBox_sample.BackColor;

            switch (strPosition)
            {
                case "Normal": richTextBox_sample.SelectionCharOffset = 0 ; break;
                case "Superscript": richTextBox_sample.SelectionCharOffset = 5; break;
                case "Subscript": richTextBox_sample.SelectionCharOffset = -5; break;
            }
        }

        private static FontStyle GetFontStyle(bool bold, bool italic, bool strikethrough, bool underline)
        {
            var fontStyle = FontStyle.Regular;
            if (bold)
            {
                fontStyle = FontStyle.Bold;
            }
            if (italic)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Italic;
                else
                    fontStyle = FontStyle.Italic;
            }
            if (strikethrough)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Strikeout;
                else
                    fontStyle = FontStyle.Strikeout;
            }
            if (underline)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Underline;
                else
                    fontStyle = FontStyle.Underline;
            }
        

            return fontStyle;
        }





        private void button_fontColorEdit_Click(object sender, EventArgs e)
        {
            var f = new ColorDialog {Color = label_fontColorDisplay.BackColor};

            if (f.ShowDialog() != DialogResult.OK) return;
            label_fontColorDisplay.BackColor = f.Color;

            UpdateVisualTextFormatting();
        }

        private void button_backroundColorEdit_Click(object sender, EventArgs e)
        {
            var f = new ColorDialog {Color = label_backroundColorDisplay.BackColor};


            if (f.ShowDialog() != DialogResult.OK) return;
            label_backroundColorDisplay.BackColor = f.Color;

            UpdateVisualTextFormatting();
        }

        private void checkBox_fontColor_CheckedChanged(object sender, EventArgs e)
        {

            CheckEnableControls();
            UpdateVisualTextFormatting();
        }

        private void checkBox_backroundColor_CheckedChanged(object sender, EventArgs e)
        {
            CheckEnableControls();
            UpdateVisualTextFormatting();
        }

        private void comboBox_styleBold_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisualTextFormatting();
        }

        private void comboBox_styleItalic_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisualTextFormatting();
        }

        private void comboBox_styleStrikethrough_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisualTextFormatting();
        }

        private void comboBox_styleUnderline_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisualTextFormatting();
        }

        private void comboBox_stylePosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisualTextFormatting();
        }

        


        



     
    }
}
