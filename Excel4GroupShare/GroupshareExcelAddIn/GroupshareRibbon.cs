using GroupshareExcelAddIn.Controls;
using GroupshareExcelAddIn.Services;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using GroupshareExcelAddIn.Interfaces;

namespace GroupshareExcelAddIn
{
    public partial class GroupshareRibbon
    {
        private readonly IGroupshareConnection _groupShareConnection = new GroupshareConnection();
        private LoginForm _loginForm;
        private ProjectForm _projectForm;
        private ResourcesControl _resourcesForm;
        private UserDataForm _userDataForm;
        private ExcelReporterService _excelReporterService;

        public GroupshareRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            _excelReporterService = new ExcelReporterService();
            InitializeComponent();
            LoginForm.LoginEnsured += LoginEnsured;
        }

        private LoginForm LoginForm => _loginForm ?? (_loginForm = new LoginForm(_groupShareConnection));

        private ProjectForm ProjectForm => _projectForm ??
                                           (_projectForm =
                                               new ProjectForm(new ProjectReportService(_groupShareConnection, _excelReporterService), _groupShareConnection));

        private ResourcesControl ResourceForm => _resourcesForm ??
                                                 (_resourcesForm =
                                                     new ResourcesControl(
                                                         new ResourcesReportServices(_groupShareConnection),
                                                         _groupShareConnection,
                                                         new LanguageService(_groupShareConnection)));

        private UserDataForm UserDataForm =>
            _userDataForm ?? (_userDataForm = new UserDataForm(new UserDataService(_groupShareConnection, _excelReporterService), _groupShareConnection));

        private void LoginButton_Click(object sender, RibbonControlEventArgs e)
        {
            _resourcesForm?.Hide();
            _projectForm?.Hide();
            _userDataForm?.Hide();
            LoginForm.Login(true, null);
        }

        private void LoginEnsured(object owner, EventArgs e)
        {
            if (owner != null && owner is Form form)
            {
                form.Show();
            }
        }

        private void ShowProjectFormButton_Click(object sender, RibbonControlEventArgs e)
        {
            if (_loginForm?.Visible == true) return;
            _resourcesForm?.Hide();
            _userDataForm?.Hide();
            if (_projectForm?.Visible == true)
            {
                return;
            }
            LoginForm.Login(false, ProjectForm);
        }

        private void ShowResourcesFormButton_Click(object sender, RibbonControlEventArgs e)
        {
            if (_loginForm?.Visible == true) return;
            _projectForm?.Hide();
            _userDataForm?.Hide();
            if (_resourcesForm?.Visible ==  true)
            {
                return;
            }

            LoginForm.Login(false, ResourceForm);
        }

        private void ShowUserDataButton_Click(object sender, RibbonControlEventArgs e)
        {
            if (_loginForm?.Visible == true) return;
            _resourcesForm?.Hide();
            _projectForm?.Hide();
            if (_userDataForm?.Visible ==  true)
            {
                return;
            }
            LoginForm.Login(false, UserDataForm);
        }
    }
}