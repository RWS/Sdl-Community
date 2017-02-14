using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.DQF.Core;
using Sdl.Community.Qualitivity.Custom;
using Sdl.Community.Structures.DQF;

namespace Sdl.Community.Qualitivity.Dialogs.DQF
{
    public partial class DqfProjectCreate : Form
    {
        public DqfProjectCreate()
        {
            InitializeComponent();
        }
        public bool Saved { get; set; }
        public DqfProject DqfProject { get; set; }

        private void DQFProjectCreate_Load(object sender, EventArgs e)
        {
            Saved = false;
            InitializeDqfComboboxes();
            textBox_name_TextChanged(null, null);
        }


        private void InitializeDqfComboboxes()
        {

            textBox_name.Text = DqfProject.Name;

            comboBox_contentType.BeginUpdate();
            comboBox_contentType.Items.Clear();
            foreach (var contentType in Configuration.ContentTypes.OrderBy(a => a.DisplayName))
                comboBox_contentType.Items.Add(new ComboboxItem(contentType.DisplayName, contentType));
            comboBox_contentType.Sorted = true;
            var selectedIndex = comboBox_contentType.Items.Cast<ComboboxItem>().TakeWhile(cbi => ((ContentType) cbi.Value).Id != DqfProject.ContentType).Count();
            comboBox_contentType.SelectedIndex = selectedIndex;
            comboBox_contentType.EndUpdate();



            comboBox_industry.BeginUpdate();
            comboBox_industry.Items.Clear();
            foreach (var industry in Configuration.Industries.OrderBy(a => a.DisplayName))
                comboBox_industry.Items.Add(new ComboboxItem(industry.DisplayName, industry));
            comboBox_industry.Sorted = true;
            selectedIndex = comboBox_industry.Items.Cast<ComboboxItem>().TakeWhile(cbi => ((Industry) cbi.Value).Id != DqfProject.Industry).Count();
            comboBox_industry.SelectedIndex = selectedIndex;
            comboBox_industry.EndUpdate();



            comboBox_process.BeginUpdate();
            comboBox_process.Items.Clear();
            foreach (var process in Configuration.Processes.OrderBy(a => a.Name))
                comboBox_process.Items.Add(new ComboboxItem(process.Name, process));
            comboBox_process.Sorted = true;
            selectedIndex = comboBox_process.Items.Cast<ComboboxItem>().TakeWhile(cbi => ((Process) cbi.Value).Id != DqfProject.Process).Count();
            comboBox_process.SelectedIndex = selectedIndex;
            comboBox_process.EndUpdate();


            comboBox_qualityLevel.BeginUpdate();
            comboBox_qualityLevel.Items.Clear();
            foreach (var qualityLevel in Configuration.QualityLevel.OrderBy(a => a.DisplayName))
                comboBox_qualityLevel.Items.Add(new ComboboxItem(qualityLevel.DisplayName, qualityLevel));
            comboBox_qualityLevel.Sorted = true;
            selectedIndex = comboBox_qualityLevel.Items.Cast<ComboboxItem>().TakeWhile(cbi => ((QualityLevel) cbi.Value).Id != DqfProject.QualityLevel).Count();
            comboBox_qualityLevel.SelectedIndex = selectedIndex;
            comboBox_qualityLevel.EndUpdate();

        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            button_Save.Enabled = textBox_name.Text.Trim() != string.Empty;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            
            DqfProject.Name = textBox_name.Text.Trim();
            DqfProject.QualityLevel = ((QualityLevel)((ComboboxItem)comboBox_qualityLevel.SelectedItem).Value).Id;
            DqfProject.Process = ((Process)((ComboboxItem)comboBox_process.SelectedItem).Value).Id;
            DqfProject.ContentType = ((ContentType)((ComboboxItem)comboBox_contentType.SelectedItem).Value).Id;
            DqfProject.Industry = ((Industry)((ComboboxItem)comboBox_industry.SelectedItem).Value).Id;
            

            Saved = true;
            Close();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


    }
}
