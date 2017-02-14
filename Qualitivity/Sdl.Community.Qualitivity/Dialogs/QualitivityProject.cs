using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Custom;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Dialogs
{
    public partial class QualitivityProject : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        private bool IsLoading { get; set; }

        public ProjectsController ProjectsController { get; set; }


        public Project TrackerProject { get; set; }
        public List<CompanyProfile> Clients { get; set; }


        private ErrorProvider _projectName;
        private ErrorProvider _dateDue;
        private ErrorProvider _dateComplete;


        public QualitivityProject()
        {
            InitializeComponent();

            InitializeErrorProviders();





        }

        private void TimeTrackerProject_Load(object sender, EventArgs e)
        {
            IsLoading = true;

            Text += IsEdit ? PluginResources.___Edit_ : PluginResources.___New_;


            initialize_studio_projects_combobox();

            if (IsEdit && comboBox_sdl_projects.Items.Count == 0
                && TrackerProject.StudioProjectId.Trim() != string.Empty)
            {
                var ci = new ComboboxItem(TrackerProject.StudioProjectId.Trim(), null);
                comboBox_sdl_projects.Items.Add(ci);
            }

            if (comboBox_sdl_projects.Items.Count > 0)
            {
                initialize_company_combobox();
                initialize_trackerProject();
                IsLoading = false;


                if (!IsEdit && TrackerProject.StudioProjectId == string.Empty)
                    comboBox_sdl_projects.SelectedIndex = 0;

                comboBox_project_status_SelectedIndexChanged(sender, e);


                textBox_project_name_TextChanged(sender, e);

                CheckDates();

                comboBox_sdl_projects_SelectedIndexChanged(null, null);
                CheckEnableSave();

                textBox_project_name.Select();
            }
            else
            {
                MessageBox.Show(this, PluginResources.No_new_SDL_Studio_Project_available_ + "\r\n\r\n"
                + PluginResources.Note__All_Studio_Projects_are_currently_listed_in_the_navigation_area, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }
        }


        private void InitializeErrorProviders()
        {
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

        private void initialize_studio_projects_combobox()
        {

            try
            {
                comboBox_sdl_projects.BeginUpdate();

                comboBox_sdl_projects.Items.Clear();

                var projects = ProjectsController.GetProjects().ToList();

                foreach (var proj in projects)
                {
                    var pi = proj.GetProjectInfo();

                    var addToList = true;
                    foreach (var tp in Tracked.TrackingProjects.TrackerProjects)
                    {
                        if (tp.StudioProjectId != pi.Id.ToString()) continue;
                        if (IsEdit)
                        {
                            if (TrackerProject.StudioProjectId == tp.StudioProjectId) continue;
                            addToList = false;
                            break;
                        }
                        addToList = false;
                        break;
                    }
                    if (!addToList) continue;
                    var ci = new ComboboxItem(pi.Name, pi);
                    comboBox_sdl_projects.Items.Add(ci);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                comboBox_sdl_projects.EndUpdate();
            }
        }
        private void initialize_company_combobox()
        {
            try
            {
                comboBox_client.BeginUpdate();

                comboBox_client.Items.Add(new ComboboxItem(@" (none)", null));

                if (Clients == null) return;
                foreach (var cpi in Clients)
                {
                    comboBox_client.Items.Add(new ComboboxItem(cpi.Name, cpi));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                comboBox_client.EndUpdate();
            }

        }
        private void initialize_trackerProject()
        {

            if (IsEdit || TrackerProject.StudioProjectId != string.Empty)
            {
                var selectedIndex = 0;
                for (var i = 0; i < comboBox_sdl_projects.Items.Count; i++)
                {
                    var cbi = (ComboboxItem)comboBox_sdl_projects.Items[i];
                    if (cbi.Value != null)
                    {
                        var pi = (ProjectInfo)cbi.Value;

                        if (TrackerProject.StudioProjectId != pi.Id.ToString() &&
                            string.Compare(TrackerProject.StudioProjectPath, pi.LocalProjectFolder.Trim(),
                                StringComparison.OrdinalIgnoreCase) != 0) continue;
                        selectedIndex = i;
                        break;
                    }
                    if (cbi.Text != TrackerProject.StudioProjectId) continue;
                    selectedIndex = i;
                    break;
                }

                comboBox_sdl_projects.SelectedIndex = selectedIndex;
            }


            textBox_project_name.Text = TrackerProject.Name.Trim();
            textBox_description.Text = TrackerProject.Description.Trim();

            if (TrackerProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                comboBox_project_status.SelectedIndex = 0;
            else if (TrackerProject.ProjectStatus.IndexOf(@"Completed", StringComparison.Ordinal) > -1)
                comboBox_project_status.SelectedIndex = 1;
            else
                comboBox_project_status.SelectedIndex = 0;


            if (IsEdit)
            {
                if (TrackerProject.Created != null) dateTimePicker_created.Value = TrackerProject.Created.Value;
                if (TrackerProject.Due != null) dateTimePicker_due.Value = TrackerProject.Due.Value;
                if (TrackerProject.Completed != null) dateTimePicker_completed.Value = TrackerProject.Completed.Value;
            }




            #region  |  client  |

            if (!IsEdit || TrackerProject.CompanyProfileId == -1)
            {
                comboBox_client.SelectedIndex = 0;
            }
            else
            {
                var selectedIndex = -1;
                for (var i = 0; i < comboBox_client.Items.Count; i++)
                {
                    var cbi = (ComboboxItem)comboBox_client.Items[i];
                    if (cbi.Value == null) continue;
                    var cpi = (CompanyProfile)cbi.Value;

                    if (cpi.Id != TrackerProject.CompanyProfileId) continue;
                    selectedIndex = i;
                    break;
                }
                comboBox_client.SelectedIndex = selectedIndex > -1 ? selectedIndex : 0;
            }
            #endregion



        }




        private void button_save_Click(object sender, EventArgs e)
        {

            var nameExists = Tracked.TrackingProjects.TrackerProjects.Any(tp => string.Compare(tp.Name, textBox_project_name.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0 && tp.Id != TrackerProject.Id);
            if (nameExists)
            {
                MessageBox.Show(this, PluginResources.The_project_name_already_exists_, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var cbiProject = (ComboboxItem)comboBox_sdl_projects.SelectedItem;
                if (cbiProject.Value != null)
                {
                    var pi = (ProjectInfo)cbiProject.Value;
                    TrackerProject.StudioProjectId = pi.Id.ToString();
                    TrackerProject.StudioProjectName = pi.Name.Trim();
                    TrackerProject.StudioProjectPath = pi.LocalProjectFolder.Trim();
                    TrackerProject.SourceLanguage = pi.SourceLanguage.CultureInfo.Name;
                }
                else
                {
                    TrackerProject.StudioProjectId = cbiProject.Text.Trim();
                    TrackerProject.StudioProjectName = string.Empty;
                    TrackerProject.StudioProjectPath = string.Empty;
                    TrackerProject.SourceLanguage = string.Empty;
                }

                TrackerProject.Name = textBox_project_name.Text.Trim();
                TrackerProject.Path = TrackerProject.StudioProjectPath;

                TrackerProject.Description = textBox_description.Text.Trim();

                var cbi = (ComboboxItem)comboBox_client.SelectedItem;
                if (cbi.Value != null)
                    TrackerProject.CompanyProfileId = ((CompanyProfile)cbi.Value).Id;
                else
                    TrackerProject.CompanyProfileId = -1;


                TrackerProject.ProjectStatus = comboBox_project_status.SelectedItem.ToString();
                TrackerProject.Created = dateTimePicker_created.Value;
                TrackerProject.Due = dateTimePicker_due.Value;
                TrackerProject.Completed = dateTimePicker_completed.Value;


                Saved = true;
                Close();
            }


        }
        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


        private void comboBox_sdl_projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            var cbi = (ComboboxItem)comboBox_sdl_projects.SelectedItem;
            var pi = (ProjectInfo)cbi.Value;


            textBox_project_name.Text = pi.Name.Trim();
            textBox_description.Text = pi.Description != null ? pi.Description.Trim() : string.Empty;

            comboBox_project_status.SelectedIndex = !pi.IsCompleted ? 0 : 1;


            dateTimePicker_created.Value = pi.CreatedAt;
            dateTimePicker_due.Value = pi.DueDate.HasValue ? pi.DueDate.Value : DateTime.Now.AddDays(7);

            dateTimePicker_completed.Value = DateTime.Now;
        }



        private void comboBox_project_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_project_status.SelectedItem.ToString().Trim() == @"Completed")
            {
                label_completed.Visible = true;
                dateTimePicker_completed.Visible = true;

                if (TrackerProject.ProjectStatus != @"Completed")
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
                textBox_project_name.Text.Trim() == string.Empty
                    ? PluginResources.The_project_name_cannot_be_empty_
                    : "");
            CheckEnableSave();
        }

        private void CheckDates()
        {
            _dateDue.SetError(dateTimePicker_due,
                dateTimePicker_due.Value < dateTimePicker_created.Value
                    ? PluginResources.The_project_due_date_cannot_precede_the_project_creation_date_
                    : "");

            _dateComplete.SetError(dateTimePicker_completed,
                dateTimePicker_completed.Value < dateTimePicker_created.Value
                    ? PluginResources.The_project_completed_date_cannot_precede_the_project_creation_date_
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



        private void comboBox_client_SelectedIndexChanged(object sender, EventArgs e)
        {


            linkLabel_view_client_profile.Enabled = false;

        }


    }
}
