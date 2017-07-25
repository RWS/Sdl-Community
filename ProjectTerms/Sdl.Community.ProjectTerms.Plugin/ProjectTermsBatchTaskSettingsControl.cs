using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public partial class ProjectTermsBatchTaskSettingsControl : UserControl, ISettingsAware<ProjectTermsBatchTaskSettings>
    {
        private ProjectTermsViewModel viewModel;

        public ProjectTermsViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }
        public ProjectTermsBatchTaskSettings Settings { get; set; }

        public ProjectTermsBatchTaskSettingsControl()
        {
            InitializeComponent();

            ViewModel = new ProjectTermsViewModel();
        }

        protected override void OnLoad(EventArgs e)
        {
            Settings.ResetToDefaults();
            base.OnLoad(e);
            SetSettings(Settings);
        }

        private void SetSettings(ProjectTermsBatchTaskSettings settings)
        {
            SettingsBinder.DataBindSetting<int>(numericUpDownTermsOccurrences, "Value", Settings, nameof(Settings.TermsOccurrencesSettings));
            SettingsBinder.DataBindSetting<int>(numericUpDownTermsLength, "Value", Settings, nameof(Settings.TermsLengthSettings));
        }

        public void ExtractProjectFileTerms(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            ViewModel.ExtractProjectFileTerms(projectFile, multiFileConverter);
        }

        public void ExtractProjectTerms(ProjectTermsBatchTaskSettings settings, string projectPath)
        {
            Settings = settings;
            ViewModel.ExtractProjectTerms(Settings.TermsOccurrencesSettings, Settings.TermsLengthSettings, Settings.BlackListSettings, projectPath);
        }

        private List<string> ExtractListViewItems(ListView listViewBlackList)
        {
            List<string> listViewItems = new List<string>();
            foreach (ListViewItem item in listViewBlackList.Items)
            {
                listViewItems.Add(item.Text);
            }

            return listViewItems;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string term = textBoxTerm.Text.ToLower();

            if (listViewBlackList.FindItemWithText(term) != null)
            {
                textBoxTerm.Text = "";
                MessageBox.Show("Term already exists!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            listViewBlackList.Items.Add(new ListViewItem(term));
            
            Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);

            textBoxTerm.Text = "";
        }

        private void buttonResetList_Click(object sender, EventArgs e)
        {
            listViewBlackList.Items.Clear();
            Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewBlackList.Items.Count == 0)
            {
                MessageBox.Show("Your list is empty!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (listViewBlackList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one term!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (ListViewItem seletectedTerm in listViewBlackList.SelectedItems)
            {
                listViewBlackList.Items.Remove(seletectedTerm);
                Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);
            }
        }

        // Redirect the enter key from textBox 
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == 256 && keyData == Keys.Enter)
            {
                buttonAdd.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
