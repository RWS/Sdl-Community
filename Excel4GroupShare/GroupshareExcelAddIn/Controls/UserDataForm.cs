using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Properties;
using GroupshareExcelAddIn.Services;
using Sdl.Community.GroupShareKit.Models.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using GroupshareExcelAddIn.Interfaces;

namespace GroupshareExcelAddIn.Controls
{
    public partial class UserDataForm : Form
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private readonly UserDataService _userDataService;
        private ProgressBarForm _progressBarForm;

        public UserDataForm(UserDataService userDataService, IGroupshareConnection groupshareConnection)
        {
            _userDataService = userDataService;
            _groupshareConnection = groupshareConnection;
            InitializeComponent();

            AttachEventHandlers();
        }

        private ProgressBarForm ProgressBarForm => _progressBarForm == null || _progressBarForm.IsDisposed
            ? _progressBarForm = new ProgressBarForm(Resources.Getting_relevant_data,
                Resources.Getting_users,
                Resources.Adding_to_Excel)
            : _progressBarForm;

        public async void ResetForm()
        {
            await SetOrganizations();
        }

        private async void _getDataButton_Click(object sender, EventArgs e)
        {
            Hide();
            ProgressBarForm.Show(this);
            var filter = _orgComboTreeBox.SelectedNode?.Tag as Organization;
            await TryCreateUserDataExcelSheet(filter);
        }

        private void AttachEventHandlers()
        {
            if (_userDataService != null)
            {
                _userDataService.ProgressChanged += ShowProgress;
            }

            VisibleChanged += (s, e) => InvokeResetForm();
        }

        private async Task<List<Organization>> GetOrganizations()
        {
            return await _groupshareConnection.GetOrganizations();
        }

        private void InvokeResetForm()
        {
            if (!Visible) return;

            if (InvokeRequired)
            {
                Invoke(new Action(ResetForm));
            }
            else
            {
                ResetForm();
            }
        }

        private async Task SetOrganizations()
        {
            if (_orgComboTreeBox?.Nodes?.Count > 0)
            {
                _orgComboTreeBox.Nodes.Clear();
            }
            _lblProcessingMessage.Visible = true;
            _lblProcessingMessage.Text = Resources.Label_Loading_organizations;

            var organizations = await GetOrganizations();

            if (organizations == null || organizations.Count == 0)
            {
                _lblProcessingMessage.Text = Resources.There_are_no_organizations_on_this_server;
                return;
            }

            _orgComboTreeBox.SetDataSource(organizations[0], "ChildOrganizations", "Name");
            _orgComboTreeBox.SelectedNode = _orgComboTreeBox.Nodes[0];

            _lblProcessingMessage.Visible = false;
        }

        private void ShowProgress(Progress progress, int barIndex)
        {
            ProgressBarForm.ShowProgress(progress, barIndex);
        }

        private async Task TryCreateUserDataExcelSheet(Organization filter)
        {
            try
            {
                await _userDataService.CreateUserDataExcelSheet(ProgressBarForm.DataRetrievalCancellationToken, ProgressBarForm.DataWritingCancellationToken, Globals.ThisAddIn.Application.ActiveSheet, filter);
            }
            catch (HttpRequestException reqEx)
            {
                Invoke(new Action(() => MessageBox.Show(string.Format(Resources.Status_code, reqEx),
                    Resources.Getting_user_data_failed, MessageBoxButtons.OK,
                    MessageBoxIcon.Error)));
            }
        }

        private void UserDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void UserDataForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }
    }
}