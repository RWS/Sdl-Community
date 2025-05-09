using GroupshareExcelAddIn.Controls.Embedded_controls;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Properties;
using GroupshareExcelAddIn.Services;
using Sdl.Community.GroupShareKit.Models.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.Controls
{
    public partial class ResourcesControl : Form
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private readonly ResourcesReportServices _resourcesReportService;
        private readonly UserControl[] _secondaryFilters;
        private ProgressBarForm _progressBarForm;

        public ResourcesControl(ResourcesReportServices resourceReportService, IGroupshareConnection groupshareConnection, LanguageService languagesService)
        {
            _resourcesReportService = resourceReportService;
            InitializeComponent();
            _secondaryFilters = new UserControl[]
                {new LanguagePairFilterControl(languagesService), new ResourceTypeFilterControl()};
            _dataTypeComboBox.DataSource = _resourcesReportService.ReportServices;
            _dataTypeComboBox.DisplayMember = "DisplayName";
            _dataTypeComboBox.SelectedIndex = -1;
            _resourcesReportService = resourceReportService;
            _groupshareConnection = groupshareConnection;

            AttachEventHandlers();
        }

        private ProgressBarForm ProgressBarForm => _progressBarForm == null || _progressBarForm.IsDisposed
            ? _progressBarForm = new ProgressBarForm(Resources.Getting_data, Resources.Adding_to_Excel)
            : _progressBarForm;

        private Control SecondaryFilterControl
        {
            get => _translationMemoriesTableLayoutPanel.GetControlFromPosition(0, 2);
            set
            {
                if (SecondaryFilterControl != null)
                {
                    _translationMemoriesTableLayoutPanel.Controls.Remove(SecondaryFilterControl);
                }
                if (value != null)
                {
                    _translationMemoriesTableLayoutPanel.Controls.Add(value, 0, 2);
                    SecondaryFilterControl.Dock = DockStyle.Fill;
                }
            }
        }

        public async void ResetForm()
        {
            await SetOrganizations();
        }

        private void AttachEventHandlers()
        {
            _resourcesReportService.ProgressChanged += ShowProgress;
            VisibleChanged += (s, e) => InvokeResetForm();
            _dataTypeComboBox.SelectedIndexChanged += DataTypeCombobox_SelectedIndexChanged;
            _dataTypeComboBox.SelectedIndexChanged += EnableGetDataButton;
        }

        private async Task CreateResourceDataExcelSheet(ResourceFilter filter)
        {
            try
            {
                await ((IReportService)_dataTypeComboBox.SelectedItem).CreateResourceDataExcelSheet(ProgressBarForm.DataRetrievalCancellationToken, ProgressBarForm.DataWritingCancellationToken, filter, Globals.ThisAddIn.Application.ActiveSheet);
            }
            catch (HttpRequestException reqEx)
            {
                Invoke(new Action(() => MessageBox.Show(string.Format(Resources.Status_code, reqEx),
                    Resources.Getting_resource_data_failed, MessageBoxButtons.OK,
                    MessageBoxIcon.Error)));
            }
        }

        private void DataTypeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _includeSubOrgs_checkBox.Visible = true;
            if (_dataTypeComboBox.SelectedItem is TermbasesReportService)
            {
                _includeSubOrgs_checkBox.Visible = false;
            }
            if (_dataTypeComboBox.SelectedIndex <= -1) return;

            var correspondingFilterControl = new UserControl();
            switch (_dataTypeComboBox.SelectedIndex)
            {
                case 0:
                    correspondingFilterControl = _secondaryFilters[0];
                    break;

                case 1:
                case 2:
                case 3:
                case 4:
                    correspondingFilterControl = null;
                    break;

                case 5:
                    correspondingFilterControl = _secondaryFilters[1];
                    break;
            }
            SecondaryFilterControl = correspondingFilterControl;
        }

        private void EnableGetDataButton(object sender, EventArgs e)
        {
            _getDataButton.Enabled = true;
            _dataTypeComboBox.SelectedIndexChanged -= EnableGetDataButton;
        }

        private ResourceFilter GatherFilterData()
        {
            var filterParameters = GetAdditionalFilterParameter();

            var organization = (Organization)_orgComboTreeBox.SelectedNode?.Tag;
            return new ResourceFilter
            {
                SecondParameter = filterParameters,
                Organization = organization,
                IncludeSubOrganizations = _includeSubOrgs_checkBox.Checked
            };
        }

        private FilterParameter GetAdditionalFilterParameter()
        {
            return ((IFilterControl)SecondaryFilterControl)?.FilterParameter;
        }

        private async void GetDataButton_Click(object sender, EventArgs e)
        {
            Hide();
            var filter = GatherFilterData();

            ProgressBarForm.Show(this);

            await CreateResourceDataExcelSheet(filter);
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

        private void ResourcesControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
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

        private void TranslationMemoryControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }
    }
}