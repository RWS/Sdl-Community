using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Plugin.Utils;
using Sdl.Community.ProjectTerms.Plugin.ViewPart;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin.Views
{
    public partial class ProjectTermsView : Form
    {
        private ProjectTermsViewModel viewModel;
        private bool checkboxEnabled = false;
        public bool ProjectSelected { get; set; }

        public ProjectTermsView()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitializeComponent();
            progressBarExtractTerms.Visible = false;

            viewModel = new ProjectTermsViewModel();
            viewModel.Filters = ExtractFilters();
        }

        private void ProjectTermsView_Load(object sender, EventArgs e)
        {
            viewModel.ProjectSelected = ProjectSelected;
            if (ProjectSelected)
            {
                buttonExtractTerms.Text = PluginResources.Button_ExtractTermsProject;
            }
            else
            {
                buttonExtractTerms.Text = PluginResources.Button_ExtractTermsFiles;
            }

            if (!File.Exists(Utils.Utils.GetXMLFilePath(Utils.Utils.GetProjectPath(), true)))
            {
                buttonAdd.Enabled = false;
                buttonDelete.Enabled = false;
                buttonResetList.Enabled = false;
                buttonSave.Enabled = false;
                buttonLoad.Enabled = false;
                buttonIncludeFile.Enabled = false;
                buttonShowWordCloud.Enabled = false;
                numericUpDownTermsOccurrences.Enabled = false;
                numericUpDownTermsLength.Enabled = false;
            }
        }

        private void buttonExtractTerms_Click(object sender, EventArgs e)
        {
            progressBarExtractTerms.Value = 0;
            buttonShowWordCloud.Enabled = false;
            buttonIncludeFile.Enabled = false;
            progressBarExtractTerms.Visible = true;

            viewModel.ExtractProjectTermsAsync((result)
                =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(Form.ActiveForm, "Failed to extract project terms: " + result.Exception.Message, "Error");
                }

                progressBarExtractTerms.Visible = false;
                buttonShowWordCloud.Enabled = true;
                buttonIncludeFile.Enabled = true;

                buttonAdd.Enabled = true;
                buttonDelete.Enabled = true;
                buttonResetList.Enabled = true;
                buttonSave.Enabled = true;
                buttonLoad.Enabled = true;
                buttonIncludeFile.Enabled = true;
                buttonShowWordCloud.Enabled = true;
                numericUpDownTermsOccurrences.Enabled = true;
                numericUpDownTermsLength.Enabled = true;

                buttonShowWordCloud.PerformClick();
            }, (progress) => { progressBarExtractTerms.Value = progress; });
        }

        #region Utils
        private List<string> ExtractListViewItems(ListView listViewBlackList)
        {
            List<string> blacklist = new List<string>();
            foreach (ListViewItem item in listViewBlackList.Items)
            {
                blacklist.Add(item.Text);
            }

            return blacklist;
        }

        public Filters ExtractFilters()
        {
            return new Filters()
            {
                Blacklist = ExtractListViewItems(listViewBlackList),
                Occurrences = (int)numericUpDownTermsOccurrences.Value,
                Length = (int)numericUpDownTermsLength.Value
            };
        }

        private void ButtonsEnabled(bool value)
        {
            buttonSave.Enabled = value;
            buttonLoad.Enabled = value;
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
        #endregion

        #region Settings Group
        private void numericUpDownTermsOccurrences_ValueChanged(object sender, EventArgs e)
        {
            viewModel.Filters.Occurrences = (int)numericUpDownTermsOccurrences.Value;
        }

        private void numericUpDownTermsLength_ValueChanged(object sender, EventArgs e)
        {
            viewModel.Filters.Length = (int)numericUpDownTermsLength.Value;
        }

        private void AddTerm(string term)
        {
            listViewBlackList.Items.Add(new ListViewItem(term));
            viewModel.Filters.Blacklist.Add(term);
            textBoxTerm.Text = "";
            ButtonsEnabled(true);
        }

        private void PopulateBlacklist(Regex regex)
        {
            viewModel.ReadProjectTermsFromFile();
            var blacklistRegex = viewModel.Terms.Where(x => regex.IsMatch(x.Text)).ToList();
            if (blacklistRegex.Count == 0)
            {
                MessageBox.Show(PluginResources.Info_NotMatch, PluginResources.MessageType_Info);
                textBoxTerm.Text = "";
                return;
            }
            foreach (var item in blacklistRegex)
            {
                if (CheckExactMatch(item.Text)) continue;
                AddTerm(item.Text);
            }

            textBoxTerm.Text = "";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string term = textBoxTerm.Text;

            if (checkboxEnabled)
            {
                if (!Utils.Utils.VerifyRegexPattern(term))
                {
                    labelErrorRegex.Text = PluginResources.Error_Regex;
                    return;
                }
                else
                {
                    var regex = new Regex(term);
                    PopulateBlacklist(regex);
                }
            } else if (checkBoxCaseSensitive.Checked)
            {
                var regex = new Regex(@"(?<!\S)" + term + @"(?!\S)", RegexOptions.IgnoreCase);
                PopulateBlacklist(regex);
            }
            else
            {
                if (CheckExactMatch(term))
                {
                    textBoxTerm.Text = "";
                    MessageBox.Show(PluginResources.MessageContent_buttonAdd, PluginResources.MessageType_Info);
                    return;
                }

                AddTerm(term);
            }
        }

        private bool CheckExactMatch(string term)
        {
            foreach (ListViewItem item in listViewBlackList.Items)
            {
                if (Regex.IsMatch(item.Text.ToString(), @"(^|\s)" + term + @"(\s|$)")) return true;
            }

            return false;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listViewBlackList.Items.Count == 0)
            {
                MessageBox.Show(PluginResources.MessageContent_buttonDelete_Empty, PluginResources.MessageType_Info);
                return;
            }

            if (listViewBlackList.SelectedItems.Count == 0)
            {
                MessageBox.Show(PluginResources.MessageContent_buttonDelete_Select, PluginResources.MessageType_Info);
                return;
            }

            foreach (ListViewItem seletectedTerm in listViewBlackList.SelectedItems)
            {
                listViewBlackList.Items.Remove(seletectedTerm);
                viewModel.Filters.Blacklist.Remove(seletectedTerm.Text);
            }

            ButtonsEnabled(true);
        }

        private void buttonResetList_Click(object sender, EventArgs e)
        {
            listViewBlackList.Items.Clear();
            viewModel.Filters.Blacklist.Clear();
            ButtonsEnabled(true);
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialogLoadFile.Filter ="TXT|*.txt";
            if (openFileDialogLoadFile.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(openFileDialogLoadFile.FileName))
                {
                    MessageBox.Show(PluginResources.MessageContent_buttonLoad, PluginResources.MessageType_Info);
                    return;
                }

                using (StreamReader rw = new StreamReader(openFileDialogLoadFile.FileName))
                {
                    listViewBlackList.Items.Clear();

                    string term = string.Empty;
                    while ((term = rw.ReadLine()) != null)
                    {
                        listViewBlackList.Items.Add(new ListViewItem(term));
                        viewModel.Filters.Blacklist.Add(term);
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialogBlacklist.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialogBlacklist.ShowDialog() == DialogResult.OK)
            {
                openFileDialogLoadFile.FileName = "";

                using (Stream s = File.Create(saveFileDialogBlacklist.FileName))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    foreach (ListViewItem item in listViewBlackList.Items)
                    {
                        sw.WriteLine(item.Text);
                    }
                }
            } 
        }

        private void checkBoxRegex_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRegex.Checked == true)
            {
                checkboxEnabled = true;
            }
            else
            {
                checkboxEnabled = false;
            }
        }

        private void textBoxTerm_TextChanged(object sender, EventArgs e)
        {
            labelErrorRegex.Text = "";
        }
        #endregion

        #region Project Terms Word Cloud
        private void buttonShowWordCloud_Click(object sender, EventArgs e)
        {
            try
            {
                if (ProjectSelected)
                {
                    var viewPartProjectWordCloudProject = SdlTradosStudio.Application.GetController<ProjectTermsViewPartProject>();
                    viewPartProjectWordCloudProject.Activate();
                    viewPartProjectWordCloudProject.GenerateWordCloud(viewModel);
                }
                else
                {
                    var viewPartProjectWordCloudFiles = SdlTradosStudio.Application.GetController<ProjectTermsViewPartFiles>();
                    viewPartProjectWordCloudFiles.Activate();
                    viewPartProjectWordCloudFiles.GenerateWordCloud(viewModel);
                }
            }
            catch (ProjectTermsException ex)
            {
                MessageBox.Show(ex.Message, PluginResources.MessageType_Error);
            }
        }
        #endregion

        #region Include terms into xml file and load to the Studio
        private void buttonIncludeFile_Click(object sender, EventArgs e)
        {
            try
            {
                progressBarExtractTerms.Value = 0;
                buttonIncludeFile.Enabled = false;
                progressBarExtractTerms.Visible = true;

                viewModel.AddXMlFileToProjectAsync((result) => 
                {
                    if (result.Exception != null)
                    {
                        MessageBox.Show(Form.ActiveForm, "Failed to include project terms file: " + result.Exception.Message, "Error");
                        return;
                    }
                    this.Close();
                }, (progress) => { progressBarExtractTerms.Value = progress; });
            }
            catch (ProjectTermsException ex)
            {
                MessageBox.Show(ex.Message, PluginResources.MessageType_Error);
            }
        }
        #endregion
    }
}
