using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Plugin.Utils;
using Sdl.Community.ProjectTerms.Plugin.ViewPart;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin.Views
{
    public partial class ProjectTermsView : Form
    {
        private ProjectTermsViewModel viewModel;
        private bool checkboxEnabled = false;

        public ProjectTermsView()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitializeComponent();

            viewModel = ProjectTermsViewModel.Instance;
            viewModel.Filters = ExtractFilters();
        }

        private void ProjectTermsView_Load(object sender, EventArgs e)
        {
            progressBarExtractTerms.Value = 0;
            buttonShowWordCloud.Enabled = false;
            buttonExtractTerms.Enabled = false;
            progressBarExtractTerms.Visible = true;

            viewModel.ExtractProjectTermsAsync((result)
                =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(Form.ActiveForm, "Failed to extract project terms: " + result.Exception.Message, "Error");
                }

                progressBarExtractTerms.Visible = false;
                labelExtractTerms.Text = PluginResources.Message_ExtractedTerms;
                buttonShowWordCloud.Enabled = true;
                buttonExtractTerms.Enabled = true;

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

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string term = textBoxTerm.Text.ToLower();

            if (listViewBlackList.FindItemWithText(term) != null)
            {
                textBoxTerm.Text = "";
                MessageBox.Show(PluginResources.MessageContent_buttonAdd, PluginResources.MessageType_Info);
                return;
            }

            if (checkboxEnabled)
            {
                string exp = textBoxTerm.Text.ToString();
                if (!Utils.Utils.VerifyRegexPattern(exp))
                {
                    labelErrorRegex.Text = PluginResources.Error_Regex;
                    return;
                }
                else
                {

                }
            }

            listViewBlackList.Items.Add(new ListViewItem(term));
            viewModel.Filters.Blacklist.Add(term);
            textBoxTerm.Text = "";
            ButtonsEnabled(true);
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
            string blackListFilePath = Utils.Utils.GenerateBlackListPath();
            if (!File.Exists(blackListFilePath))
            {
                MessageBox.Show(PluginResources.MessageContent_buttonLoad, PluginResources.MessageType_Info);
                return;
            }

            using (StreamReader rw = new StreamReader(blackListFilePath))
            {
                listViewBlackList.Items.Clear();

                string term = string.Empty;
                while ((term = rw.ReadLine()) != null)
                {
                    listViewBlackList.Items.Add(new ListViewItem(term));
                    viewModel.Filters.Blacklist.Add(term);
                }
            }

            ButtonsEnabled(false);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string blackListFilePath = Utils.Utils.GenerateBlackListPath();

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
                var viewPartProjectWordCloud = SdlTradosStudio.Application.GetController<ProjectTermsViewPart>();
                viewPartProjectWordCloud.Activate();
                viewPartProjectWordCloud.GenerateWordCloud(viewModel);
            }
            catch (ProjectTermsException ex)
            {
                MessageBox.Show(ex.Message, PluginResources.MessageType_Error, MessageBoxButtons.OK);
            }
        }
        #endregion

        #region Extract term into xml file
        private void buttonExtractTerms_Click(object sender, EventArgs e)
        {
            try
            {
                viewModel.AddXMlFileToProject();
                this.Close();
            } catch(ProjectTermsException ex)
            {
                MessageBox.Show(ex.Message, PluginResources.MessageType_Error);
            }
        }
        #endregion

        private void ProjectTermsView_FormClosed(object sender, FormClosedEventArgs e)
        {
            var viewPartProjectWordCloud = SdlTradosStudio.Application.GetController<ProjectTermsViewPart>();
            viewPartProjectWordCloud.Activate();
            viewPartProjectWordCloud.ResetCloud();
        }
    }
}
