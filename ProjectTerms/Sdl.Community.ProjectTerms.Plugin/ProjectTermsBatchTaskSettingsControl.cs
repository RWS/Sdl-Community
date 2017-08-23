using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Plugin
{
    
    public partial class ProjectTermsBatchTaskSettingsControl : UserControl, ISettingsAware<ProjectTermsBatchTaskSettings>
    {
        public ProjectTermsViewModel ViewModel { get; set; }
        public ProjectTermsBatchTaskSettings Settings { get; set; }
        public string ProjectPath { get; set; }
        public static bool ControlDisabled { get; set; }

        public ProjectTermsBatchTaskSettingsControl()
        {
            if (ControlDisabled)
            {
                MessageBox.Show(PluginResources.Error_SingleFileProject, PluginResources.MessageType_Error);
                return;
            }

            InitializeComponent();

            ViewModel = new ProjectTermsViewModel();
            ProjectPath = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().LocalProjectFolder;

            if (!File.Exists(GenerateBlackListPath())) buttonLoad.Enabled = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (ControlDisabled) return;

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

        public void ExtractProjectTerms(ProjectTermsBatchTaskSettings settings)
        {
            Settings = settings;
            ViewModel.ExtractProjectTerms(Settings.TermsOccurrencesSettings, Settings.TermsLengthSettings, Settings.BlackListSettings, ProjectPath);
        }

        public int GetNumbersOfExtractedTerms()
        {
            return ViewModel.Terms.Count();
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
                MessageBox.Show(PluginResources.MessageContent_buttonAdd, PluginResources.MessageType_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            listViewBlackList.Items.Add(new ListViewItem(term));
            
            Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);

            textBoxTerm.Text = "";
            ButtonsEnabled(true);
        }

        private void buttonResetList_Click(object sender, EventArgs e)
        {
            listViewBlackList.Items.Clear();
            Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);
            ButtonsEnabled(true);
        }

        private void ButtonsEnabled(bool value)
        {
            buttonSave.Enabled = value;
            buttonLoad.Enabled = value;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewBlackList.Items.Count == 0)
            {
                MessageBox.Show(PluginResources.MessageContent_buttonDelete_Empty, PluginResources.MessageType_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (listViewBlackList.SelectedItems.Count == 0)
            {
                MessageBox.Show(PluginResources.MessageContent_buttonDelete_Select, PluginResources.MessageType_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (ListViewItem seletectedTerm in listViewBlackList.SelectedItems)
            {
                listViewBlackList.Items.Remove(seletectedTerm);
                Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);
            }

            ButtonsEnabled(true);
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

        private string GenerateBlackListPath()
        {
            return Path.Combine(ProjectPath, "blackListTerms.txt");
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            string blackListFilePath = GenerateBlackListPath();
            if (!File.Exists(blackListFilePath))
            {
                MessageBox.Show(PluginResources.MessageContent_buttonLoad, PluginResources.MessageType_Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (StreamReader rw = new StreamReader(blackListFilePath))
            {
                listViewBlackList.Items.Clear();

                string term = string.Empty;
                while ((term = rw.ReadLine()) != null)
                {
                    listViewBlackList.Items.Add(new ListViewItem(term));
                    Settings.BlackListSettings = ExtractListViewItems(listViewBlackList);
                }
            }

            ButtonsEnabled(false);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string blackListFilePath = GenerateBlackListPath();

            if (File.Exists(blackListFilePath)) File.Delete(blackListFilePath);

            using (StreamWriter sw = new StreamWriter(blackListFilePath))
            {
                foreach (ListViewItem item in listViewBlackList.Items)
                {
                    sw.WriteLine(item.Text);
                }
            }

            ButtonsEnabled(false);
        }
    }
}
