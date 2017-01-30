using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Custom;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.Community.Studio.Time.Tracker.Tracking;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class TimeTrackerProject : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        private bool IsLoading { get; set; }

        public ProjectsController ProjectsController { get; set; }
        public string SelectedProjectId { get; set; }

        public TrackerProject TrackerProject { get; set; }
        public List<ClientProfileInfo> Clients { get; set; }


        private readonly ErrorProvider _projectName;
        private readonly ErrorProvider _dateDue;
        private readonly ErrorProvider _dateComplete;


        public TimeTrackerProject()
        {
            InitializeComponent();

            IsLoading = true;

            _projectName = new ErrorProvider();
            _projectName.SetIconAlignment(textBox_project_name, ErrorIconAlignment.MiddleRight);
            _projectName.SetIconPadding(textBox_project_name, 2);
            _projectName.BlinkRate = 1000;
            _projectName.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;


            _dateDue = new ErrorProvider();
            _dateDue.SetIconAlignment(dateTimePicker_due, ErrorIconAlignment.MiddleRight);
            _dateDue.SetIconPadding(dateTimePicker_due, 2);
            _dateDue.BlinkRate = 1000;
            _dateDue.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;


            _dateComplete = new ErrorProvider();
            _dateComplete.SetIconAlignment(dateTimePicker_completed, ErrorIconAlignment.MiddleRight);
            _dateComplete.SetIconPadding(dateTimePicker_completed, 2);
            _dateComplete.BlinkRate = 1000;
            _dateComplete.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;

        }




        private void button_save_Click(object sender, EventArgs e)
        {

            var nameExists = Tracked.Preferences.TrackerProjects.Any(trackerProject =>
                string.Compare(trackerProject.Name, textBox_project_name.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0
                && trackerProject.Id != TrackerProject.Id);
            if (nameExists)
            {
                MessageBox.Show(this, @"The project name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var cbiProject = (ComboboxItem)comboBox_sdl_projects.SelectedItem;
                if (cbiProject.Value != null)
                {
                    var projectInfo = (ProjectInfo)cbiProject.Value;
                    TrackerProject.IdStudio = projectInfo.Id.ToString();
                    TrackerProject.NameStudio = projectInfo.Name.Trim();
                    TrackerProject.PathStudio = projectInfo.LocalProjectFolder.Trim();
                }
                else
                {
                    TrackerProject.IdStudio = cbiProject.Text.Trim();
                    TrackerProject.NameStudio = string.Empty;
                    TrackerProject.PathStudio = string.Empty;
                }

                TrackerProject.Name = textBox_project_name.Text.Trim();
                TrackerProject.Path = TrackerProject.PathStudio;

                TrackerProject.Description = textBox_description.Text.Trim();

                var comboboxItem = (ComboboxItem)comboBox_client.SelectedItem;
                TrackerProject.ClientId = comboboxItem.Value != null ? ((ClientProfileInfo)comboboxItem.Value).Id.Trim() : string.Empty;


                TrackerProject.ProjectStatus = comboBox_project_status.SelectedItem.ToString();
                TrackerProject.DateCreated = dateTimePicker_created.Value;
                TrackerProject.DateDue = dateTimePicker_due.Value;
                TrackerProject.DateCompleted = dateTimePicker_completed.Value;


                Saved = true;
                Close();
            }


        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


        private void TimeTrackerProject_Load(object sender, EventArgs e)
        {
            Text += IsEdit ? " (Edit)" : " (New)";


            initialize_studio_projects_combobox();

            if (IsEdit && comboBox_sdl_projects.Items.Count == 0
                && TrackerProject.IdStudio.Trim() != string.Empty)
            {
                var comboboxItem = new ComboboxItem(TrackerProject.IdStudio.Trim(), null);
                comboBox_sdl_projects.Items.Add(comboboxItem);
            }

            if (comboBox_sdl_projects.Items.Count > 0)
            {
                initialize_company_combobox();

                initialize_trackerProject();


                IsLoading = false;


                if (!IsEdit)
                    comboBox_sdl_projects.SelectedIndex = 0;


                comboBox_project_status_SelectedIndexChanged(sender, e);


                textBox_project_name_TextChanged(sender, e);
                CheckDates();
                CheckEnableSave();


                textBox_project_name.Select();
            }
            else
            {
                MessageBox.Show(this, @"No new SDL Studio Project available!" + "\r\n\r\n"
                + @"Note: All Studio Projects are currently listed in the navigation area", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }
        }


        private void initialize_studio_projects_combobox()
        {

            comboBox_sdl_projects.BeginUpdate();

            comboBox_sdl_projects.Items.Clear();

            var projects = ProjectsController.GetProjects().ToList();
            foreach (var project in projects)
            {
                var projectInfo = project.GetProjectInfo();



                var addToList = true;
                foreach (var trackerProject in Tracked.Preferences.TrackerProjects)
                {
                    if (trackerProject.IdStudio != projectInfo.Id.ToString()) continue;
                    if (IsEdit)
                    {
                        if (TrackerProject.IdStudio == trackerProject.IdStudio) continue;
                        addToList = false;
                        break;
                    }
                    addToList = false;
                    break;
                }
                if (!addToList) continue;
                var comboboxItem = new ComboboxItem(projectInfo.Name, projectInfo);
                comboBox_sdl_projects.Items.Add(comboboxItem);
            }

            comboBox_sdl_projects.EndUpdate();
        }

        private void initialize_company_combobox()
        {
            try
            {
                comboBox_client.BeginUpdate();

                comboBox_client.Items.Add(new ComboboxItem(" (none)", null));


                foreach (var clientProfileInfo in Clients)
                {
                    comboBox_client.Items.Add(new ComboboxItem(clientProfileInfo.ClientName, clientProfileInfo));
                }


            }
            finally
            {
                comboBox_client.EndUpdate();
            }

        }

        private void comboBox_sdl_projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            var comboboxItem = (ComboboxItem)comboBox_sdl_projects.SelectedItem;
            var itemValue = (ProjectInfo)comboboxItem.Value;


            textBox_project_name.Text = itemValue.Name.Trim();
            textBox_description.Text = itemValue.Description != null ? itemValue.Description.Trim() : string.Empty;

            comboBox_project_status.SelectedIndex = !itemValue.IsCompleted ? 0 : 1;


            dateTimePicker_created.Value = itemValue.CreatedAt;
            dateTimePicker_due.Value = itemValue.DueDate ?? DateTime.Now.AddDays(7);

            dateTimePicker_completed.Value = DateTime.Now;
        }

        private void initialize_trackerProject()
        {

            if (IsEdit)
            {
                var selectedIndex = 0;
                for (var i = 0; i < comboBox_sdl_projects.Items.Count; i++)
                {
                    var comboboxItem = (ComboboxItem)comboBox_sdl_projects.Items[i];
                    if (comboboxItem.Value != null)
                    {
                        var pi = (ProjectInfo)comboboxItem.Value;

                        if (TrackerProject.IdStudio != pi.Id.ToString() &&
                            string.Compare(TrackerProject.PathStudio, pi.LocalProjectFolder.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                            continue;
                        selectedIndex = i;
                        break;
                    }
                    if (comboboxItem.Text != TrackerProject.IdStudio) continue;
                    selectedIndex = i;
                    break;
                }

                comboBox_sdl_projects.SelectedIndex = selectedIndex;
            }


            textBox_project_name.Text = TrackerProject.Name.Trim();
            textBox_description.Text = TrackerProject.Description.Trim();

            if (TrackerProject.ProjectStatus.IndexOf("In progress", StringComparison.Ordinal) > -1)
                comboBox_project_status.SelectedIndex = 0;
            else if (TrackerProject.ProjectStatus.IndexOf("Completed", StringComparison.Ordinal) > -1)
                comboBox_project_status.SelectedIndex = 1;
            else
                comboBox_project_status.SelectedIndex = 0;


            dateTimePicker_created.Value = TrackerProject.DateCreated;
            dateTimePicker_due.Value = TrackerProject.DateDue;
            dateTimePicker_completed.Value = TrackerProject.DateCompleted;




            #region  |  client  |
            if (!IsEdit || TrackerProject.ClientId.Trim() == string.Empty)
            {
                comboBox_client.SelectedIndex = 0;
            }
            else
            {
                var selectedIndex = -1;
                for (var i = 0; i < comboBox_client.Items.Count; i++)
                {
                    var comboboxItem = (ComboboxItem)comboBox_client.Items[i];
                    if (comboboxItem.Value == null) continue;
                    var clientProfileInfo = (ClientProfileInfo)comboboxItem.Value;

                    if (clientProfileInfo.Id != TrackerProject.ClientId.Trim()) continue;
                    selectedIndex = i;
                    break;
                }
                comboBox_client.SelectedIndex = selectedIndex > -1 ? selectedIndex : 0;
            }
            #endregion



        }

        private void comboBox_project_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_project_status.SelectedItem.ToString().Trim() == "Completed")
            {
                label_completed.Visible = true;
                dateTimePicker_completed.Visible = true;

                if (TrackerProject.ProjectStatus != "Completed")
                {
                    dateTimePicker_completed.Value = DateTime.Now;
                }
            }
            else
            {
                label_completed.Visible = false;
                dateTimePicker_completed.Visible = false;
            }
        }

        private void textBox_project_name_TextChanged(object sender, EventArgs e)
        {
            _projectName.SetError(textBox_project_name,
                textBox_project_name.Text.Trim() == string.Empty ? "The project name cannot be empty!" : "");
            CheckEnableSave();
        }

        private void CheckDates()
        {
            _dateDue.SetError(dateTimePicker_due,
                dateTimePicker_due.Value < dateTimePicker_created.Value
                    ? "The project due date cannot precede the project creation date!"
                    : "");

            _dateComplete.SetError(dateTimePicker_completed,
                dateTimePicker_completed.Value < dateTimePicker_created.Value
                    ? "The project completed date cannot precede the project creation date!"
                    : "");

            CheckEnableSave();

        }
        private void dateTimePicker_due_ValueChanged(object sender, EventArgs e)
        {
            CheckDates();
        }

        private void dateTimePicker_completed_ValueChanged(object sender, EventArgs e)
        {
            CheckDates();
        }


        private void CheckEnableSave()
        {
            var enableSave = true;

            if (dateTimePicker_completed.Value < dateTimePicker_created.Value)
                enableSave = false;
            else if (dateTimePicker_due.Value < dateTimePicker_created.Value)
                enableSave = false;
            else if (textBox_project_name.Text.Trim() == string.Empty)
                enableSave = false;

            button_save.Enabled = enableSave;
        }

        private void linkLabel_view_client_profile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            var comboboxItem = (ComboboxItem)comboBox_client.SelectedItem;

            if (comboboxItem.Value == null) return;
            var clientProfile = new ClientProfile();

            var clientProfileInfo = (ClientProfileInfo)comboboxItem.Value;

            clientProfile.ClientProfileInfo = clientProfileInfo;
            clientProfile.IsEdit = true;
            clientProfile.ShowDialog();
            if (!clientProfile.Saved) return;
            comboboxItem.Text = clientProfile.ClientProfileInfo.ClientName;
            comboboxItem.Value = clientProfile.ClientProfileInfo;
            comboBox_client.SelectedItem = comboboxItem;
            comboBox_client.Invalidate(true);

            SettingsSerializer.SaveSettings(Tracked.Preferences);
        }

        private void comboBox_client_SelectedIndexChanged(object sender, EventArgs e)
        {


            linkLabel_view_client_profile.Enabled = false;

        }


    }
}
