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
using ProjectStatus = GroupshareExcelAddIn.Models.ProjectStatus;

namespace GroupshareExcelAddIn.Controls
{
    public partial class ProjectForm : Form
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private readonly ProjectReportService _reportService;
        private ProgressBarForm _progressBarForm;

        public ProjectForm(ProjectReportService reportService, IGroupshareConnection groupshareConnection)
        {
            InitializeComponent();
            SetBindings();
            SetDefaultTimeInterval();
            _reportService = reportService;
            _groupshareConnection = groupshareConnection;

            AttachEventHandlers();
        }

        private ProgressBarForm ProgressBarForm => (_progressBarForm == null || _progressBarForm.IsDisposed)
            ? _progressBarForm = new ProgressBarForm(Resources.Projects, Resources.Files, Resources.Adding_files_to_Excel)
            : _progressBarForm;

        private void AttachEventHandlers()
        {
            _reportService.ProgressChanged += ShowProgress;
            VisibleChanged += (s, e) => InvokeResetForm();
        }

        private async Task CreateExcelSheet(ProjectFilter filter)
        {
            try
            {
                await _reportService.CreateFilteredProjectDataExcelSheet(filter,
                    ProgressBarForm.DataRetrievalCancellationToken, ProgressBarForm.DataWritingCancellationToken,
                    Globals.ThisAddIn.Application.ActiveSheet);
            }
            catch (HttpRequestException reqEx)
            {
                Invoke(new Action(() => MessageBox.Show(string.Format(Resources.Status_code, reqEx.Message),
                    Resources.GroupshareConnection_GetGsProjects_Getting_projects_info_failed, MessageBoxButtons.OK,
                    MessageBoxIcon.Error)));
            }
        }

        private ProjectFilter GatherFilterData()
        {
            DateTime? startPublishDate = null;
            DateTime? endPublishDate = null;
            if (_projectPubDate_checkBox.Checked)
            {
                startPublishDate = _startPublishingDate.Value;
                endPublishDate = _endPublishingDate.Value;
            }

            DateTime? startDelDate = null;
            DateTime? endDelDate = null;
            if (_projectDelDate_checkBox.Checked)
            {
                startDelDate = _startDeliveryDate.Value;
                endDelDate = _endDeliveryDate.Value;
            }

            var dateRange = new DateRange(startPublishDate, endPublishDate, startDelDate, endDelDate);
            var projectStatus = GetProjectStatus();

            var organization = _orgComboTreeBox.SelectedNode?.Tag as Organization;
            var includeSubOrgs = organization == null || _includeSubOrgs_checkBox.Checked;

            var filter = new ProjectFilter(organization != null ? organization.Path : "\u002F",
                dateRange,
                projectStatus, includeSubOrgs, _includePhasesCheckBox.Checked);
            return filter;
        }

        private async Task<List<Organization>> GetOrganizations()
        {
            return await _groupshareConnection.GetOrganizations();
        }

        private async void GetProjectDataButton_Click(object sender, EventArgs e)
        {
            var filter = GatherFilterData();
            Hide();
            ProgressBarForm.Show(this);
            await CreateExcelSheet(filter);
        }

        private ProjectStatus GetProjectStatus()
        {
            if (!_projectStatus_checkBox.Checked)
                return (ProjectStatus)31;

            var projectStatus = new ProjectStatus();
            if (checkBoxInProgress.Checked)
            {
                projectStatus |= ProjectStatus.InProgress;
            }
            if (checkBoxCompleted.Checked)
            {
                projectStatus |= ProjectStatus.Completed;
            }
            if (checkBoxArchived.Checked)
            {
                projectStatus |= ProjectStatus.Archived;
            }
            if (detachedCheckBox.Checked)
            {
                projectStatus |= ProjectStatus.Detached;
            }
            if (pendingCheckBox.Checked)
            {
                projectStatus |= ProjectStatus.Pending;
            }

            return projectStatus;
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

        private async void ResetForm()
        {
            await SetOrganizations();
        }

        private async Task SetOrganizations()
        {
            if (_orgComboTreeBox?.Nodes.Count > 0)
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
    }
}