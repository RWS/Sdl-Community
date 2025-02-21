using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Compare.Properties;


namespace PostEdit.Compare.Forms
{
    public partial class FilterAppend : Form
    {
        public bool Saved { get; set; }
        public bool IsEdit { get; set; }

        public Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting FilterSetting { get; set; }

        public FilterAppend()
        {
            FilterSetting = new Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting();
            InitializeComponent();

            dateTimePicker_filterDate.Value = DateTime.Now;
            comboBox_filterDate_type.SelectedIndex = 0;



            comboBox_attributes_archive.SelectedIndex = 0;
            comboBox_attributes_hidden.SelectedIndex = 0;
            comboBox_attributes_readOnly.SelectedIndex = 0;
            comboBox_attributes_system.SelectedIndex = 0;

            checkBox_attributes_archive.Checked = false;
            checkBox_attributes_hidden.Checked = false;
            checkBox_attributes_readOnly.Checked = false;
            checkBox_attributes_system.Checked = false;
        }


        private void CheckEnabled()
        {
            if (textBox_name.Text.Trim() != string.Empty
                && textBox_names_extension_list.Text.Trim() != string.Empty)
            {
                button_save.Enabled = true;
            }
            else
            {
                button_save.Enabled = false;
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


        
        private void button_save_Click(object sender, EventArgs e)
        {
            var r = new Regex(@"\r\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            FilterSetting.Name = textBox_name.Text;
            FilterSetting.FilterNamesInclude.Clear();
            FilterSetting.FilterNamesExclude.Clear();

            if (textBox_names_extension_list.Text.Trim() != string.Empty)
            {
                var includedIndex = new List<string>();
                var items = r.Split(textBox_names_extension_list.Text).ToList();
                foreach (string item in items)
                {
                    if (item.Trim() == string.Empty) continue;
                    if (includedIndex.Contains(item.Trim().ToLower())) continue;
                    includedIndex.Add(item.Trim().ToLower());

                    if (comboBox_names_extension_list_type.SelectedIndex==0)
                        FilterSetting.FilterNamesInclude.Add(item.Trim());
                    else
                        FilterSetting.FilterNamesExclude.Add(item.Trim());
                }
            }

            FilterSetting.UseRegularExpressionMatching = checkBox_regularExpression.Checked;

            FilterSetting.FilterDateUsed = checkBox_filterDate.Checked;
            FilterSetting.FilterDate = new Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate();
            FilterSetting.FilterDate.Date = dateTimePicker_filterDate.Value;
            FilterSetting.FilterDate.Type = (comboBox_filterDate_type.SelectedIndex == 0 ? Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.GreaterThan : Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.LessThan);


            FilterSetting.FilterAttributeAchiveType = comboBox_attributes_archive.SelectedItem.ToString();
            FilterSetting.FilterAttributeArchiveUsed = checkBox_attributes_archive.Checked;

            FilterSetting.FilterAttributeHiddenType = comboBox_attributes_hidden.SelectedItem.ToString();
            FilterSetting.FilterAttributeHiddenUsed = checkBox_attributes_hidden.Checked;

            FilterSetting.FilterAttributeReadOnlyType = comboBox_attributes_readOnly.SelectedItem.ToString();
            FilterSetting.FilterAttributeReadOnlyUsed = checkBox_attributes_readOnly.Checked;

            FilterSetting.FilterAttributeSystemType = comboBox_attributes_system.SelectedItem.ToString();
            FilterSetting.FilterAttributeSystemUsed = checkBox_attributes_system.Checked;

            Saved = true;
            Close();
        }

        private void FilterAppend_Load(object sender, EventArgs e)
        {
            Text = IsEdit ? Resources.FilterAppend_FilterAppend_Load_Filter_Settings_Edit : Resources.FilterAppend_FilterAppend_Load_Filter_Settings_New;
            textBox_name.Text = FilterSetting.Name;


            comboBox_names_extension_list_type.SelectedIndex = 0;


            textBox_names_extension_list.Text = string.Empty;                                   

            if (FilterSetting.FilterNamesInclude.Count > 0)
            {
                comboBox_names_extension_list_type.SelectedIndex = 0;
                foreach (var str in FilterSetting.FilterNamesInclude)
                {
                    if (str.Trim() != string.Empty)
                    {
                        textBox_names_extension_list.Text += (textBox_names_extension_list.Text.Trim() != string.Empty ? "\r\n" : string.Empty) + str.Trim();
                    }
                }
            }
            else if (FilterSetting.FilterNamesExclude.Count > 0)
            {
                comboBox_names_extension_list_type.SelectedIndex = 1;
                foreach (var str in FilterSetting.FilterNamesExclude)
                {
                    if (str.Trim() != string.Empty)
                    {
                        textBox_names_extension_list.Text += (textBox_names_extension_list.Text.Trim() != string.Empty ? "\r\n" : string.Empty) + str.Trim();
                    }
                }
            }

            checkBox_regularExpression.Checked = FilterSetting.UseRegularExpressionMatching;

            if (FilterSetting.FilterDateUsed)
            {
                checkBox_filterDate.Checked = true;
                comboBox_filterDate_type.SelectedIndex = (FilterSetting.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.GreaterThan ? 0 : 1);
                dateTimePicker_filterDate.Value = FilterSetting.FilterDate.Date;
            }
            else
            {
                checkBox_filterDate.Checked = false;
            }
            checkBox_filterDate_CheckedChanged(null, null);



            comboBox_attributes_archive.SelectedItem = FilterSetting.FilterAttributeAchiveType;
            checkBox_attributes_archive.Checked = FilterSetting.FilterAttributeArchiveUsed;


            comboBox_attributes_hidden.SelectedItem = FilterSetting.FilterAttributeHiddenType;
            checkBox_attributes_hidden.Checked = FilterSetting.FilterAttributeHiddenUsed;

            comboBox_attributes_readOnly.SelectedItem = FilterSetting.FilterAttributeReadOnlyType;
            checkBox_attributes_readOnly.Checked = FilterSetting.FilterAttributeReadOnlyUsed;

            comboBox_attributes_system.SelectedItem = FilterSetting.FilterAttributeSystemType;
            checkBox_attributes_system.Checked = FilterSetting.FilterAttributeSystemUsed;


            checkBox_attributes_archive_CheckedChanged(null, null);
            checkBox_attributes_system_CheckedChanged(null, null);
            checkBox_attributes_hidden_CheckedChanged(null, null);
            checkBox_attributes_readOnly_CheckedChanged(null, null);

            CheckEnabled();
            
        }

        private void checkBox_filterDate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_filterDate.Checked)
            {
                comboBox_filterDate_type.Enabled = true;
                dateTimePicker_filterDate.Enabled = true;
            }
            else
            {
                comboBox_filterDate_type.Enabled = false;
                dateTimePicker_filterDate.Enabled = false;
            }

            CheckEnabled();
        }

        private void textBox_names_include_TextChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void textBox_names_exclude_TextChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void comboBox_filterDate_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void dateTimePicker_filterDate_ValueChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void checkBox_attributes_archive_CheckedChanged(object sender, EventArgs e)
        {
            comboBox_attributes_archive.Enabled = checkBox_attributes_archive.Checked;
        }

        private void checkBox_attributes_system_CheckedChanged(object sender, EventArgs e)
        {
            comboBox_attributes_system.Enabled = checkBox_attributes_system.Checked;            
        }

        private void checkBox_attributes_hidden_CheckedChanged(object sender, EventArgs e)
        {
            comboBox_attributes_hidden.Enabled = checkBox_attributes_hidden.Checked;
        }

        private void checkBox_attributes_readOnly_CheckedChanged(object sender, EventArgs e)
        {
            comboBox_attributes_readOnly.Enabled = checkBox_attributes_readOnly.Checked;
            
        }
    }
}
