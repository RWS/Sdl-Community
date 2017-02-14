using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.DQF.Core;
using Sdl.Community.Hooks;
using Sdl.Community.Parser;
using Sdl.Community.Qualitivity.Dialogs;
using Sdl.Community.Qualitivity.Dialogs.DQF;
using Sdl.Community.Qualitivity.Dialogs.Export;
using Sdl.Community.Qualitivity.Panels.Activity_Records;
using Sdl.Community.Qualitivity.Panels.Document_Reports;
using Sdl.Community.Qualitivity.Panels.DQF;
using Sdl.Community.Qualitivity.Panels.Properties;
using Sdl.Community.Qualitivity.Panels.QualityMetrics;
using Sdl.Community.Qualitivity.Progress;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Report;
using Sdl.Community.Structures.Configuration;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.DQF;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.PropertyView;
using Sdl.Community.TM.Database;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Activity = Sdl.Community.Structures.Projects.Activities.Activity;
using DocumentActivities = Sdl.Community.Structures.Projects.Activities.DocumentActivities;
using Project = Sdl.Community.Structures.Projects.Project;
using QualityMetric = Sdl.Community.Structures.Documents.Records.QualityMetric;
using Settings = Sdl.Community.Structures.Configuration.Settings;

namespace Sdl.Community.Qualitivity.Panels.Main
{


    [View(
        Id = "QualitivityViewController",
        Name = "Qualitivity",
        Description = "Qualitivity",
        Icon = "QualitivityApp_Icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation),
        AllowViewParts = true
        )]
    public class QualitivityViewController : AbstractViewController
    {


        private Timer Timer4ProjectArea { get; set; }

        private static readonly Lazy<QualitivityViewControl> _viewContent = new Lazy<QualitivityViewControl>();
        private static readonly Lazy<QualitivityNavigationControl> _viewNavigation = new Lazy<QualitivityNavigationControl>();

        public FileBasedProject CurrentSelectedProject { get; set; }
        public static ProjectsController ProjectsController { get; set; }
        public ProjectInfo CurrentProjectInfo { get; set; }

        private bool IsLoading { get; set; }
        public Project SelectedProject { get; set; }
        public List<Activity> SelectedActivities { get; set; }


        protected override Control GetContentControl()
        {
            return _viewContent.Value;
        }
        protected override Control GetExplorerBarControl()
        {
            return _viewNavigation.Value;
        }


        public event EventHandler CheckEnabledObjectsEvent;
        public void CheckEnabledObjects()
        {

            try
            {

                #region  |  navigation selection  |



                TimeTrackerProjectNewEnabled = true;

                _viewNavigation.Value.newTimeTrackerProjectToolStripMenuItem.Enabled = TimeTrackerProjectNewEnabled;
                _viewNavigation.Value.editTimeTrackerProjectToolStripMenuItem.Enabled = TimeTrackerProjectEditEnabled;
                _viewNavigation.Value.removeTimeTrackerProjectToolStripMenuItem.Enabled = TimeTrackerProjectRemoveEnabled;


                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null && _viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
                {
                    if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                        && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    {
                        ProjectActivityNewEnabled = true;
                        _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                        _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                    }
                    else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null
                        || _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(CompanyProfile))
                    {
                        if (_viewNavigation.Value.treeView_navigation.SelectedNode.Nodes.Count > 0)
                        {
                            ProjectActivityNewEnabled = true;
                            _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                            _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            ProjectActivityNewEnabled = false;
                            _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = false;
                            _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = false;

                            _viewNavigation.Value.editTimeTrackerProjectToolStripMenuItem.Enabled = true;
                            _viewNavigation.Value.removeTimeTrackerProjectToolStripMenuItem.Enabled = true;

                            _viewNavigation.Value.newDQFProjectToolStripMenuItem.Enabled = true;
                        }
                    }
                    else if (_viewContent.Value.objectListView1.SelectedItems.Count > 0)
                    {
                        ProjectActivityNewEnabled = true;
                        _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                        _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        ProjectActivityNewEnabled = false;
                        _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = false;
                        _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    FillActivityDataView(new List<Activity>());

                    ProjectActivityNewEnabled = false;
                    _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = false;
                    _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = false;
                }

                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null
                    && _viewContent.Value.objectListView1.SelectedItems.Count > 0)
                {
                    ProjectActivityEditEnabled = true;
                    ProjectActivityRemoveEnabled = true;

                    _viewContent.Value.editProjectActivityToolStripMenuItem.Enabled = true;
                    _viewContent.Value.removeProjectActivityToolStripMenuItem.Enabled = true;
                }
                else
                {
                    ProjectActivityEditEnabled = false;
                    ProjectActivityRemoveEnabled = false;

                    _viewContent.Value.editProjectActivityToolStripMenuItem.Enabled = false;
                    _viewContent.Value.removeProjectActivityToolStripMenuItem.Enabled = false;
                }

                if (_viewContent.Value.objectListView1.SelectedItems.Count > 1)
                {
                    ProjectActivityMergeEnabled = true;
                    _viewContent.Value.mergeProjectActivitiesToolStripMenuItem.Enabled = true;

                    ProjectActivityDuplicateEnabled = true;
                    _viewContent.Value.duplicateTheProjectActivityToolStripMenuItem.Enabled = true;
                }
                else if (_viewContent.Value.objectListView1.SelectedItems.Count == 1)
                {
                    ProjectActivityMergeEnabled = false;
                    _viewContent.Value.mergeProjectActivitiesToolStripMenuItem.Enabled = false;

                    ProjectActivityDuplicateEnabled = true;
                    _viewContent.Value.duplicateTheProjectActivityToolStripMenuItem.Enabled = true;
                }
                else
                {
                    ProjectActivityMergeEnabled = false;
                    _viewContent.Value.mergeProjectActivitiesToolStripMenuItem.Enabled = false;

                    ProjectActivityDuplicateEnabled = false;
                    _viewContent.Value.duplicateTheProjectActivityToolStripMenuItem.Enabled = false;
                }

                if (_viewContent.Value.objectListView1.SelectedItems.Count > 0)
                {
                    ProjectActivityCreateReportEnabled = true;
                    _viewContent.Value.createAnActivitiesReportToolStripMenuItem.Enabled = true;
                }
                else
                {
                    ProjectActivityCreateReportEnabled = false;
                    _viewContent.Value.createAnActivitiesReportToolStripMenuItem.Enabled = false;
                }
                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {

                    ProjectActivityExportExcelEnabled = true;
                    _viewContent.Value.exportActivitiesToExcelToolStripMenuItem.Enabled = true;
                }
                else
                {
                    ProjectActivityExportExcelEnabled = false;
                    _viewContent.Value.exportActivitiesToExcelToolStripMenuItem.Enabled = false;
                }
                #endregion


                #region  |  Quality Metric  |

                IsEnabledCreateNewQualityMetric = true;


                #endregion


                #region  |  DQF Project  |



                _viewNavigation.Value.newDQFProjectToolStripMenuItem.Enabled = ProjectActivityNewEnabled;
                IsEnabledCreateNewDqfProject = ProjectActivityNewEnabled;
                DqfProjectEnabled = ProjectActivityNewEnabled;
                DqfProjectImportEnabled = ProjectActivityNewEnabled;


                DqfProjectSaveEnabled = QualitivityViewDqfController.Control.Value.treeView_dqf_projects.SelectedNode != null;

                if (_viewContent.Value.objectListView1.SelectedItems.Count > 0 && QualitivityViewDqfController.Control.Value.treeView_dqf_projects.SelectedNode != null)
                {
                    DqfProjectTaskEnabled = true;
                }
                else
                {
                    DqfProjectTaskEnabled = false;
                }
                #endregion



                if (CheckEnabledObjectsEvent != null)
                    CheckEnabledObjectsEvent(this, EventArgs.Empty);

                QualitivityViewPropertiesController.NavigationTreeView = _viewNavigation.Value.treeView_navigation;
                QualitivityViewPropertiesController.ObjectListView = _viewContent.Value.objectListView1;
                QualitivityViewPropertiesController.UpdateActivityPropertiesViewer();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private bool IsActive { get; set; }
        protected override void Initialize(IViewContext context)
        {
            ActivationChanged += StudioTimeTrackerViewController_ActivationChanged;


            IsLoading = true;

            IsEnabledCreateNewDqfProject = false;
            IsEnabledCreateNewActivityProject = false;
            IsEnabledCreateNewQualityMetric = false;

            TimeTrackerProjectNewEnabled = true;
            TimeTrackerProjectEditEnabled = false;
            TimeTrackerProjectRemoveEnabled = false;


            ProjectActivityNewEnabled = false;
            ProjectActivityEditEnabled = false;
            ProjectActivityRemoveEnabled = false;


            ProjectActivityStartTrackerEnabled = true;
            ProjectActivityStopTrackerEnabled = false;

            ProjectActivityDuplicateEnabled = false;
            ProjectActivityMergeEnabled = false;

            ProjectActivityCreateReportEnabled = false;
            ProjectActivityExportExcelEnabled = false;

            DqfProjectEnabled = false;
            DqfProjectImportEnabled = false;
            DqfProjectSaveEnabled = false;
            DqfProjectTaskEnabled = false;


            #region  |  initialize the tracker cache  |


            Tracked.TrackingState = Tracked.TimerState.None;
            Tracked.TrackingTimer = new Stopwatch();
            Tracked.TrackingStart = null;
            Tracked.TrackingEnd = null;
            Tracked.TrackingPaused = new Stopwatch();


            Tracked.DocumentSegmentPairs = new Dictionary<string, SegmentPair>();
            Tracked.ActiveDocument = null;
            Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();


            #endregion




            LoadCurrencies();


            Tracked.Settings = new Settings();

            Sdl.Community.TM.Database.Helper.InitializeDatabases(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabasePath);



            var query = new Query();


            query.InitializeSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath
                , Tracked.Settings.ViewSettings.ViewProperties
                , Tracked.Settings.BackupSettings.BackupProperties
                , Tracked.Settings.GeneralSettings.GeneralProperties
                , Tracked.Settings.TrackingSettings.TrackingProperties);




            Tracked.Settings.ViewSettings.ViewProperties = query.GetViewSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath);
            Tracked.Settings.BackupSettings.BackupProperties = query.GetBackupSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath);
            Tracked.Settings.GeneralSettings.GeneralProperties = query.GetGeneralSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath);
            Tracked.Settings.TrackingSettings.TrackingProperties = query.GetTrackerSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath);

            Tracked.Settings.DqfSettings = query.GetDqfSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath);




            Tracked.Settings.QualityMetricGroupSettings.QualityMetricGroups = query.GetQualityMetricGroupSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, null);
            foreach (var qmg in Tracked.Settings.QualityMetricGroupSettings.QualityMetricGroups)
                if (qmg.IsDefault)
                    Tracked.Settings.QualityMetricGroup = qmg;



            Tracked.Settings.UserProfile = new UserProfile();
            var userProfiles = query.GetUserProfiles(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, null);
            if (userProfiles.Count > 0)
                Tracked.Settings.UserProfile = userProfiles[0];


            Tracked.Settings.CompanyProfiles = query.GetCompanyProfiles(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, null);
            Tracked.Settings.LanguageRateGroups = query.GetLanguageRateGroupSetttings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, null);


            Tracked.TrackingProjects = new TrackingProjects
            {
                TrackerProjects =
                    query.GetProjects(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath,
                        "", "", "", "")
            };


            var defaultActivityViewGroupsIsOn = Convert.ToBoolean(Tracked.Settings.GetGeneralProperty("defaultActivityViewGroupsIsOn").Value);
            _viewContent.Value.objectListView1.ShowGroups = defaultActivityViewGroupsIsOn;
            _viewContent.Value.linkLabel_turn_off_groups.Text = defaultActivityViewGroupsIsOn ? "Turn off groups" : "Turn on groups";


            var reportProcessor = new Processor();
            reportProcessor.InitializeWriteFlagsFolder(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, new List<string> { "empty" });



            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();


            InitializeTrackingEventsTab();

            InitializeNavigationControl();

            InitializeViewControl();

            InitializeDqfControl();
            CheckEnabledObjects();



            Timer4ProjectArea = new Timer {Interval = 1000};
            Timer4ProjectArea.Tick += Timer4ProjectArea_Tick;
            Timer4ProjectArea.Start();


            Viewer.IsTracking = Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("recordKeyStokes").Value);
            if (Viewer.IsTracking)
                Viewer.StartTracking();
            else
                Viewer.StopTracking();

            IsLoading = false;



            if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("startOnLoad").Value))
                StartTimeTracking();


            CheckBackupStatus();

            ProjectsController.SelectedProjectsChanged += projectsController_SelectedProjectsChanged;

        }

        private void projectsController_SelectedProjectsChanged(object sender, EventArgs e)
        {

            if (!TimeTrackerProjectNewEnabled)
            {
                IsEnabledCreateNewActivityProject = false;
                IsEnabledCreateNewDqfProject = false;
            }
            else if (ProjectsController.SelectedProjects.FirstOrDefault() != null)
            {
                CurrentSelectedProject = ProjectsController.SelectedProjects.FirstOrDefault();
                if (CurrentSelectedProject != null)
                    CurrentProjectInfo = CurrentSelectedProject.GetProjectInfo();

                var enableNewQualitivityProject = false;
                if (CurrentSelectedProject != null && CurrentProjectInfo != null)
                    enableNewQualitivityProject = Tracked.TrackingProjects.TrackerProjects.Exists(a => a.StudioProjectId == CurrentProjectInfo.Id.ToString());


                IsEnabledCreateNewActivityProject = !enableNewQualitivityProject;
            }
            else
            {
                IsEnabledCreateNewActivityProject = false;

            }

            CheckEnabledObjects();


        }

        private static void CheckBackupStatus()
        {

            var backupLastDateStr = Tracked.Settings.GetBackupProperty("backupLastDate").Value.Trim();
            var backupFolderStr = Tracked.Settings.GetBackupProperty("backupFolder").Value.Trim();
            var backupEveryStr = Tracked.Settings.GetBackupProperty("backupEvery").Value.Trim();
            var backupEveryTypeStr = Tracked.Settings.GetBackupProperty("backupEveryType").Value.Trim();

            if (backupLastDateStr.Trim() == string.Empty) return;
            try
            {
                var dateTimeFromSqLite = Sdl.Community.TM.Database.Helper.DateTimeFromSQLite(backupLastDateStr);
                if (dateTimeFromSqLite == null) return;
                var dtA = dateTimeFromSqLite.Value;
                var dtOriginal = dateTimeFromSqLite.Value;
                var dtNow = DateTime.Now;

                dtA = Convert.ToInt32(backupEveryTypeStr) == 0 ? dtA.AddDays(Convert.ToInt32(backupEveryStr)) : dtA.AddDays(Convert.ToInt32(backupEveryStr) * 7);

                if (dtA >= dtNow) return;
                if (backupFolderStr.Trim() == string.Empty || !Directory.Exists(backupFolderStr)) return;
                var ts = dtNow.Subtract(dtOriginal);

                var dr = MessageBox.Show(string.Format(PluginResources.The_last_backup_for_Qualitivity_was_performed_0_days_ago_on_the_1_, Math.Truncate(ts.TotalDays), backupLastDateStr.Replace("T", " ")) + "\r\n\r\n"
                                    + PluginResources.Do_you_want_to_backup_your_settings_now + "\r\n\r\n"
                                    + PluginResources.Select_Yes_to_backup_your_setting_now + "\r\n"
                                    + PluginResources.Select_No_to_ignore_this_message, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    Helper.BackUpMyDatabasesNow(backupFolderStr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void StudioTimeTrackerViewController_ActivationChanged(object sender, ActivationChangedEventArgs e)
        {
            IsActive = e.Active;

            if (IsActive)
            {
                CheckEnabledObjects();
            }
        }

        private void Timer4ProjectArea_Tick(object sender, EventArgs e)
        {

            if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("idleTimeOut").Value))
            {
                if ((Tracked.TrackingState == Tracked.TimerState.Started) && Tracked.ActiveDocument != null)
                {

                    var ts = DateTime.Now.Subtract(Tracked.TrackerLastActivity);
                    if (ts.TotalMinutes > Convert.ToInt32(Tracked.Settings.GetTrackingProperty("idleTimeOutMinutes").Value))
                    {
                        Tracked.TrackingIsDirtyC1 = true;
                        Tracked.TrackingIsDirtyC2 = true;

                        Tracked.TrackingTimer.Stop();
                        Tracked.TrackingState = Tracked.TimerState.Paused;

                        MessageBox.Show(string.Format(PluginResources.Paused_the_activity_timer_at_0, DateTime.Now), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    Tracked.TrackerLastActivity = DateTime.Now;
                }
            }

            if (!Tracked.TrackingIsDirtyC0 && !Tracked.TarckerCheckNewProjectAdded &&
                !Tracked.TarckerCheckNewActivityAdded) return;


            try
            {
                //stop timer for the duration of processing
                Timer4ProjectArea.Stop();

                if (Tracked.TrackingIsDirtyC0)
                {
                    #region  |  Cache.tracking_is_dirty_c0  |
                    Tracked.TrackingIsDirtyC0 = false;

                    if (_viewContent.Value.Parent != null)
                        _viewContent.Value.Parent.Cursor = Cursors.Default;

                    if (Tracked.TrackingState == Tracked.TimerState.None
                        || Tracked.TrackingState == Tracked.TimerState.Stopped
                        || Tracked.TrackingState == Tracked.TimerState.Deleted)
                    {
                        ProjectActivityStartTrackerEnabled = true;
                        ProjectActivityStopTrackerEnabled = false;
                    }
                    else
                    {
                        ProjectActivityStartTrackerEnabled = false;
                        ProjectActivityStopTrackerEnabled = true;
                    }

                    if (CheckEnabledObjectsEvent != null)
                    {
                        CheckEnabledObjectsEvent(this, EventArgs.Empty);
                    }


                    if (Tracked.HandlerPartent != 0)
                        treeView_navigation_AfterSelect(null, null);

                    #endregion
                }

                if (!Tracked.TarckerCheckNewProjectAdded && !Tracked.TarckerCheckNewActivityAdded)
                    return;

                #region  |  Cache.tarcker_check_new_project_added || Cache.tarcker_check_new_activity_added)  |
                try
                {
                    if (Tracked.TarckerCheckNewProjectAdded || Tracked.TarckerCheckNewActivityAdded)
                    {

                        Tracked.TarckerCheckNewProjectAdded = false;
                        CheckRefreshNavigationNewProject();
                    }

                    if (!Tracked.TarckerCheckNewActivityAdded) return;
                    Tracked.TarckerCheckNewActivityAdded = false;
                    FilterViewerControl(Tracked.TarckerCheckNewActivityId);
                }
                finally
                {
                    Tracked.TarckerCheckNewProjectAdded = false;
                    Tracked.TarckerCheckNewActivityAdded = false;
                }
                #endregion
            }
            finally
            {
                //restart timer
                Timer4ProjectArea.Start();

                //ensure rest the parameters to false
                Tracked.TrackingIsDirtyC0 = false;
                Tracked.TarckerCheckNewProjectAdded = false;
                Tracked.TarckerCheckNewActivityAdded = false;
            }




        }
        public void CheckRefreshNavigationNewProject()
        {
            Project project = null;
            try
            {

                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {
                    if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                        && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                        project = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                    else
                        project = Tracked.TrackingProjects.TrackerProjects.Find(a => a.Id == Tracked.TarckerCheckNewProjectId);
                }
            }
            catch
            {
                // ignored
            }

            Tracked.TarckerCheckNewProjectAdded = false;
            Tracked.TarckerCheckNewProjectId = -1;


            UpdateNavigationControl(project);
            CheckEnabledObjects();

        }


        #region  |  Tracking document activities  |


        private static EditorController GetEditorController()
        {
            return SdlTradosStudio.Application.GetController<EditorController>();
        }
        private static void InitializeTrackingEventsTab()
        {            
            TrackedEditorEvents.InitializeDocumentTrackingEvents(GetEditorController());
            TrackedEditorEvents.EditorControllerClosing += TrackedEditorEvents_EditorControllerClosing;
        }

        private static void TrackedEditorEvents_EditorControllerClosing(object sender, EventArgs e)
        {
            FilterViewerControl(Tracked.TarckerCheckNewActivityId);

            if (Tracked.DictCacheDocumentItems.Count == 0)
                CleanQualityMetricsDataContainer();

            ProjectActivityStartTrackerEnabled = false;
            ProjectActivityStopTrackerEnabled = true;
        }


        #endregion


        #region  |  Navigation control  |

        public bool IsEnabledCreateNewDqfProject { get; set; }
        public bool IsEnabledCreateNewActivityProject { get; set; }
        public bool IsEnabledCreateNewQualityMetric { get; set; }

        public bool TimeTrackerProjectNewEnabled { get; set; }
        public bool TimeTrackerProjectEditEnabled { get; set; }
        public bool TimeTrackerProjectRemoveEnabled { get; set; }


        private void InitializeNavigationControl()
        {
            try
            {

                _viewNavigation.Value.comboBox_project_status.BeginUpdate();
                _viewNavigation.Value.comboBox_project_status.SelectedItem = Tracked.Settings.GetGeneralProperty("defaultFilterProjectStatus").Value;
                _viewNavigation.Value.comboBox_project_status.EndUpdate();

                _viewNavigation.Value.comboBox_activity_status.BeginUpdate();
                _viewNavigation.Value.comboBox_activity_status.SelectedItem = Tracked.Settings.GetGeneralProperty("defaultFilterActivityStatus").Value;
                _viewNavigation.Value.comboBox_activity_status.EndUpdate();

                _viewNavigation.Value.comboBox_groupBy.BeginUpdate();
                _viewNavigation.Value.comboBox_groupBy.SelectedItem = Tracked.Settings.GetGeneralProperty("defaultFilterGroupBy").Value;
                _viewNavigation.Value.comboBox_groupBy.EndUpdate();

                _viewNavigation.Value.checkBox_include_unlisted_projects.Checked = Convert.ToBoolean(Tracked.Settings.GetGeneralProperty("defaultIncludeUnlistedProjects").Value);


                _viewNavigation.Value.button_project_search.Click += button_project_search_Click;
                _viewNavigation.Value.textBox_filter_name.KeyUp += textBox_filter_name_KeyUp;
                _viewNavigation.Value.button_auto_expand_treeview.Click += button_auto_expand_treeview_Click;
                _viewNavigation.Value.checkBox_include_unlisted_projects.CheckStateChanged += checkBox_include_unlisted_projects_CheckStateChanged;


                _viewNavigation.Value.newTimeTrackerProjectToolStripMenuItem.Click += newTimeTrackerProjectToolStripMenuItem_Click;
                _viewNavigation.Value.editTimeTrackerProjectToolStripMenuItem.Click += editTimeTrackerProjectToolStripMenuItem_Click;
                _viewNavigation.Value.removeTimeTrackerProjectToolStripMenuItem.Click += removeTimeTrackerProjectToolStripMenuItem_Click;
                _viewNavigation.Value.newProjectActivityToolStripMenuItem.Click += newProjectActivityToolStripMenuItem_Click;

                _viewNavigation.Value.comboBox_project_status.SelectedIndexChanged += comboBox_project_status_SelectedIndexChanged;
                _viewNavigation.Value.comboBox_activity_status.SelectedIndexChanged += comboBox_activity_status_SelectedIndexChanged;
                _viewNavigation.Value.comboBox_groupBy.SelectedIndexChanged += comboBox_groupBy_SelectedIndexChanged;

                _viewNavigation.Value.treeView_navigation.AfterExpand += treeView_navigation_AfterExpand;
                _viewNavigation.Value.treeView_navigation.AfterCollapse += treeView_navigation_AfterCollapse;
                _viewNavigation.Value.treeView_navigation.DoubleClick += treeView_navigation_DoubleClick;
                _viewNavigation.Value.treeView_navigation.AfterSelect += treeView_navigation_AfterSelect;
                _viewNavigation.Value.treeView_navigation.KeyUp += treeView_navigation_KeyUp;
                _viewNavigation.Value.newDQFProjectToolStripMenuItem.Click += newDQFProjectToolStripMenuItem_Click;

                UpdateNavigationControl(null);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }




        private void treeView_navigation_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveTimeTrackerProject();
        }


        public void NewTimeTrackerProject()
        {

            var f = new QualitivityProject();

            var project = new Project
            {
                Id = -1,
                StudioProjectId = string.Empty,
                Created = DateTime.Now,
                Started = DateTime.Now,
                Due = DateTime.Now.AddMonths(1)
            };


            CurrentSelectedProject = ProjectsController.SelectedProjects.FirstOrDefault();

            if (CurrentSelectedProject != null)
                CurrentProjectInfo = CurrentSelectedProject.GetProjectInfo();

            if (CurrentSelectedProject != null && CurrentProjectInfo != null)
            {
                var projectExists = Tracked.TrackingProjects.TrackerProjects.Exists(a => a.StudioProjectId == CurrentProjectInfo.Id.ToString());

                if (!projectExists)
                {

                    project.Name = CurrentProjectInfo.Name;
                    project.Path = CurrentProjectInfo.LocalProjectFolder;
                    project.Description = CurrentProjectInfo.Description ?? string.Empty;
                    project.CompanyProfileId = -1;
                    project.Created = CurrentProjectInfo.CreatedAt;
                    project.Started = DateTime.Now;
                    project.Due = CurrentProjectInfo.DueDate ?? DateTime.Now.AddDays(7);
                    project.ProjectStatus = CurrentProjectInfo.IsCompleted ? "Completed" : "In progress";
                    project.Completed = project.Due;

                    project.StudioProjectId = CurrentProjectInfo.Id.ToString();
                    project.StudioProjectName = CurrentProjectInfo.Name;
                    project.StudioProjectPath = CurrentProjectInfo.LocalProjectFolder;
                    project.SourceLanguage = CurrentProjectInfo.SourceLanguage.CultureInfo.Name;

                }
            }



            f.TrackerProject = project;
            f.Clients = Tracked.Settings.CompanyProfiles;

            f.IsEdit = false;
            f.ProjectsController = ProjectsController;

            f.ShowDialog();
            if (!f.Saved) return;

            var query = new Query();
            project.Id = query.CreateProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, project);
            Tracked.TrackingProjects.TrackerProjects.Add(project);

            UpdateNavigationControl(project);

            projectsController_SelectedProjectsChanged(null, null);
        }
        public void EditTimeTrackerProject()
        {

            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                {
                    var project = _viewNavigation.Value.treeView_navigation.SelectedNode.Tag as Project;

                    if (project != null && !Helper.QualitivityProjectInStudioProjectList(ProjectsController, project.StudioProjectId))
                        return;
                    if (project == null) return;

                    var clientIdRef = project.CompanyProfileId;


                    var f = new QualitivityProject
                    {
                        TrackerProject = project,
                        Clients = Tracked.Settings.CompanyProfiles,
                        IsEdit = true,
                        ProjectsController = ProjectsController
                    };
                    f.ShowDialog();
                    if (!f.Saved)
                        return;

                    if (f.TrackerProject.CompanyProfileId != clientIdRef)
                        foreach (var tpa in project.Activities)
                            tpa.CompanyProfileId = f.TrackerProject.CompanyProfileId;

                    project = f.TrackerProject;
                    var query = new Query();
                    query.UpdateProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, project);

                    UpdateNavigationControl(project);
                }
            }
        }
        public void RemoveTimeTrackerProject()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null ||
                _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() != typeof(Project)) return;
            var project = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

            var dr = MessageBox.Show(PluginResources.Are_you_sure_that_you_want_to_remove_this_project_and_all_its_data + "\r\n\r\n"
                                     + PluginResources.Note__you_will_not_be_able_to_recover_this_data_if_you_continue + "\r\n\r\n"
                                     + PluginResources.Click_Yes_to_continue_and_remove_the_project + "\r\n"
                                     + PluginResources.Click_No_to_cancel
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;
            Tracked.TrackingProjects.TrackerProjects.RemoveAll(a => a.Id == project.Id);
            _viewNavigation.Value.treeView_navigation.SelectedNode.Remove();

            var query = new Query();
            query.DeleteProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath
                , Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + project.Id.ToString().PadLeft(6, '0')
                , project.Id);
        }
        public void ViewQualitivityProjects()
        {
            Activate();
        }


        private void newTimeTrackerProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTimeTrackerProject();
        }
        private void editTimeTrackerProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTimeTrackerProject();
        }
        private void removeTimeTrackerProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveTimeTrackerProject();
        }
        private void newDQFProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null ||
                _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() != typeof(Project)) return;
            if (Tracked.Settings.DqfSettings.UserKey.Trim() == string.Empty)
            {
                MessageBox.Show(PluginResources.The_DQF_API_key_cannot_be_null + "\r\n\r\n"
                + PluginResources.To_create_a_TAUS_DQF_Project_you_must_first_save_your_DQF_API_key
                    , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var project = _viewNavigation.Value.treeView_navigation.SelectedNode.Tag as Project;


                if (project == null) return;

                var f = new DqfProjectCreate { DqfProject = new DqfProject { Name = project.Name } };
                f.ShowDialog();
                if (!f.Saved) return;

                try
                {
                    var dqfProject = f.DqfProject;
                    dqfProject.ProjectId = project.Id;
                    dqfProject.ProjectIdStudio = project.StudioProjectId;
                    dqfProject.SourceLanguage = project.SourceLanguage;
                    dqfProject.Created = DateTime.Now;
                    dqfProject.DqfPmanagerKey = Tracked.Settings.DqfSettings.UserKey;
                    dqfProject.DqfProjectId = -1;
                    dqfProject.DqfProjectKey = string.Empty;
                    dqfProject.Imported = false;

                    var processor = new global::Sdl.Community.DQF.Processor();
                    var productivityProject = new ProductivityProject
                    {
                        Name = dqfProject.Name,
                        QualityLevel = dqfProject.QualityLevel,
                        SourceLanguage = dqfProject.SourceLanguage,
                        Process = dqfProject.Process,
                        ContentType = dqfProject.ContentType,
                        Industry = dqfProject.Industry
                    };
                    productivityProject = processor.PostDqfProject(Tracked.Settings.DqfSettings.UserKey, productivityProject);
                    dqfProject.DqfProjectId = productivityProject.ProjectId;
                    dqfProject.DqfProjectKey = productivityProject.ProjectKey;

                    var query = new Query();
                    dqfProject.Id = query.CreateDqfProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, dqfProject);

                    var tn = QualitivityViewDqfController.Control.Value.treeView_dqf_projects.Nodes.Add(dqfProject.Name);
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = tn.ImageIndex;
                    tn.Tag = dqfProject;

                    project.DqfProjects.Add(dqfProject);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void treeView_navigation_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (IsLoading) return;
            try
            {

                SelectedProject = null;

                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                {
                    SelectedProject = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

                    TimeTrackerProjectEditEnabled = Helper.QualitivityProjectInStudioProjectList(ProjectsController, SelectedProject.StudioProjectId);


                    TimeTrackerProjectRemoveEnabled = true;

                }
                else
                {
                    TimeTrackerProjectEditEnabled = false;
                    TimeTrackerProjectRemoveEnabled = false;
                }

                QualitivityViewDqfControl.Project = SelectedProject;
                QualitivityViewDqfController.Control.Value.initialize_dqfProjects();


                FilterViewerControl(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                CheckEnabledObjects();
            }
        }
        private void treeView_navigation_DoubleClick(object sender, EventArgs e)
        {
            EditTimeTrackerProject();
        }
        private void treeView_navigation_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            checkEnableExpandAll_Navigation_treeview();
        }
        private void treeView_navigation_AfterExpand(object sender, TreeViewEventArgs e)
        {
            checkEnableExpandAll_Navigation_treeview();
        }
        private void checkEnableExpandAll_Navigation_treeview()
        {
            if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf("In progress", StringComparison.Ordinal) > -1)
            {
                _viewNavigation.Value.button_auto_expand_treeview.Enabled = false;
            }
            else
            {
                var isAllExpanded = true;
                foreach (TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes)
                {
                    if (tn.Nodes.Count <= 0) continue;
                    if (!tn.IsExpanded)
                    {
                        isAllExpanded = false;
                        break;
                    }
                    if (tn.Nodes.Cast<TreeNode>().Where(_tn => _tn.Nodes.Count > 0).Any(_tn => !_tn.IsExpanded))
                    {
                        isAllExpanded = false;
                    }
                }

                _viewNavigation.Value.button_auto_expand_treeview.Enabled = !isAllExpanded;
            }
        }
        private void button_auto_expand_treeview_Click(object sender, EventArgs e)
        {
            _viewNavigation.Value.treeView_navigation.ExpandAll();
            _viewNavigation.Value.button_auto_expand_treeview.Enabled = false;
        }
        private void textBox_filter_name_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsLoading) return;

            if (e.KeyCode != Keys.Return) return;

            Project tp = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
            }
            UpdateNavigationControl(tp);
            CheckEnabledObjects();
        }
        private void button_project_search_Click(object sender, EventArgs e)
        {
            if (IsLoading) return;
            Project tp = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
            }
            UpdateNavigationControl(tp);
            CheckEnabledObjects();
        }
        private void comboBox_groupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            Project tp = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
            }
            UpdateNavigationControl(tp);
            CheckEnabledObjects();
        }
        private void comboBox_project_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            Project tp = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
            }
            UpdateNavigationControl(tp);
            CheckEnabledObjects();
        }
        private void comboBox_activity_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            Project tp = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
            }
            UpdateNavigationControl(tp);
            CheckEnabledObjects();
        }

        private void checkBox_include_unlisted_projects_CheckStateChanged(object sender, EventArgs e)
        {
            if (IsLoading) return;
            Project tp = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
            }
            UpdateNavigationControl(tp);
            CheckEnabledObjects();
        }

        private static IEnumerable<Project> GetFilteredProjects()
        {
            var tps = new List<Project>();
            try
            {
                //Show all projects
                //In progress
                //Completed
                var projectStatus = _viewNavigation.Value.comboBox_project_status.SelectedItem.ToString().Trim();
                var activityStatus = _viewNavigation.Value.comboBox_activity_status.SelectedItem.ToString().Trim();
                var filterName = _viewNavigation.Value.comboBox_filter_name.SelectedItem.ToString().Trim();
                var nameSearch = _viewNavigation.Value.textBox_filter_name.Text.Trim();

                //Client name
                //Project name
                //Date (year/month)
                var groupBy = _viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().Trim();

                foreach (var tp in Tracked.TrackingProjects.TrackerProjects)
                {
                    #region  |  filter?  |

                    var filterStatusProject = true;
                    if (_viewNavigation.Value.comboBox_project_status.SelectedIndex > 0)
                    {
                        if (projectStatus.IndexOf("In progress", StringComparison.Ordinal) > -1)
                        {
                            if (tp.ProjectStatus.IndexOf("In progress", StringComparison.Ordinal) <= -1)
                                filterStatusProject = false;
                        }
                        else if (projectStatus.IndexOf("Completed", StringComparison.Ordinal) > -1)
                        {
                            if (tp.ProjectStatus.IndexOf("Completed", StringComparison.Ordinal) <= -1)
                                filterStatusProject = false;
                        }
                    }

                    var filterStatusActivity = true;
                    if (_viewNavigation.Value.comboBox_activity_status.SelectedIndex > 0)
                    {
                        filterStatusActivity = false;
                        if (activityStatus.IndexOf("New", StringComparison.Ordinal) > -1)
                        {
                            if (tp.Activities.Any(tpa => tpa.ActivityStatus == Activity.Status.New))
                            {
                                filterStatusActivity = true;
                            }
                        }
                        else if (activityStatus.IndexOf("Confirmed", StringComparison.Ordinal) > -1)
                        {
                            if (tp.Activities.Any(tpa => tpa.ActivityStatus == Activity.Status.Confirmed))
                            {
                                filterStatusActivity = true;
                            }
                        }
                    }

                    var filterSearch = true;
                    if (nameSearch.Trim() != string.Empty)
                    {
                        if (_viewNavigation.Value.comboBox_filter_name.SelectedIndex == 0)
                        {
                            if (tp.Name.Trim().ToLower().IndexOf(nameSearch.ToLower(), StringComparison.Ordinal) <= -1)
                                filterSearch = false;
                        }
                        else
                        {
                            filterSearch = false;
                            foreach (var tpa in tp.Activities)
                            {
                                if (tpa.Name.Trim().ToLower().IndexOf(nameSearch.ToLower(), StringComparison.Ordinal) > -1)
                                    filterSearch = true;
                            }
                        }
                    }

                    #endregion

                    if (filterStatusProject && filterStatusActivity && filterSearch)
                        tps.Add(tp);
                }
            }
            catch
            {
                // ignore
            }

            return tps;
        }
        private void UpdateNavigationControl(Project selectedProject)
        {

            //Client name
            //Project name
            //Date (year/month)
            var groupBy = _viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().Trim();


            var projects = ProjectsController.GetProjects().ToList();
            var studioProjectListIds = new List<string>();
            foreach (var proj in projects)
            {
                var pi = proj.GetProjectInfo();
                if (!studioProjectListIds.Contains(pi.Id.ToString()))
                    studioProjectListIds.Add(pi.Id.ToString());
            }


            try
            {
                _viewNavigation.Value.treeView_navigation.BeginUpdate();
                _viewNavigation.Value.treeView_navigation.Nodes.Clear();

                #region  |  get index  |
                var cpiNameDict = new Dictionary<string, CompanyProfile>();
                var cpiProjects = new Dictionary<string, List<Project>>();
                var nameKeys = new List<string>();
                var cpiDateCreated = new Dictionary<string, Dictionary<string, List<Project>>>();
                var cpiDateDue = new Dictionary<string, Dictionary<string, List<Project>>>();

                var tps = GetFilteredProjects();
                foreach (var tp in tps)
                {
                    var addToList = !(!studioProjectListIds.Contains(tp.StudioProjectId) && !_viewNavigation.Value.checkBox_include_unlisted_projects.Checked);

                    if (!addToList) continue;

                    #region  |  build the structures  |

                    #region  |  by date created  |

                    if (tp.Created != null && cpiDateCreated.ContainsKey(tp.Created.Value.Year.ToString()))
                    {
                        var tpCreatedYear = cpiDateCreated[tp.Created.Value.Year.ToString()];

                        if (tpCreatedYear.ContainsKey(tp.Created.Value.Month.ToString().PadLeft(2, '0')))
                        {
                            var tpCreatedMonth = tpCreatedYear[tp.Created.Value.Month.ToString().PadLeft(2, '0')];
                            tpCreatedMonth.Add(tp);
                            tpCreatedYear[tp.Created.Value.Month.ToString().PadLeft(2, '0')] = tpCreatedMonth;
                        }
                        else
                        {
                            var tpCreatedMonth = new List<Project> { tp };
                            tpCreatedYear.Add(tp.Created.Value.Month.ToString().PadLeft(2, '0'), tpCreatedMonth);
                        }

                        cpiDateCreated[tp.Created.Value.Year.ToString()] = tpCreatedYear;
                    }
                    else
                    {

                        var tpCreatedYear = new Dictionary<string, List<Project>>();

                        var tpCreatedMonth = new List<Project> { tp };
                        if (tp.Created != null)
                        {
                            tpCreatedYear.Add(tp.Created.Value.Month.ToString().PadLeft(2, '0'), tpCreatedMonth);
                            cpiDateCreated.Add(tp.Created.Value.Year.ToString(), tpCreatedYear);
                        }
                    }
                    #endregion


                    #region  |  by date due  |

                    if (tp.Due != null && cpiDateDue.ContainsKey(tp.Due.Value.Year.ToString()))
                    {
                        var tpDueYear = cpiDateDue[tp.Due.Value.Year.ToString()];

                        if (tpDueYear.ContainsKey(tp.Due.Value.Month.ToString().PadLeft(2, '0')))
                        {
                            var tpDueMonth = tpDueYear[tp.Due.Value.Month.ToString().PadLeft(2, '0')];
                            tpDueMonth.Add(tp);
                            tpDueYear[tp.Due.Value.Month.ToString().PadLeft(2, '0')] = tpDueMonth;
                        }
                        else
                        {
                            var tpDueMonth = new List<Project> { tp };
                            if (tp.Due != null)
                                tpDueYear.Add(tp.Due.Value.Month.ToString().PadLeft(2, '0'), tpDueMonth);
                        }

                        if (tp.Due != null) cpiDateDue[tp.Due.Value.Year.ToString()] = tpDueYear;
                    }
                    else
                    {

                        var tpDueYear = new Dictionary<string, List<Project>>();

                        var tpDueMonth = new List<Project> { tp };
                        if (tp.Due != null)
                        {
                            tpDueYear.Add(tp.Due.Value.Month.ToString().PadLeft(2, '0'), tpDueMonth);


                            cpiDateDue.Add(tp.Due.Value.Year.ToString(), tpDueYear);
                        }
                    }
                    #endregion


                    if (tp.CompanyProfileId > -1)
                    {
                        #region  |  has client  |
                        var cpi = Helper.GetClientFromId(tp.CompanyProfileId);

                        if (!cpiNameDict.ContainsKey(cpi.Name.Trim()))
                        {
                            cpiNameDict.Add(cpi.Name.Trim(), cpi);
                            nameKeys.Add(cpi.Name.Trim());
                            cpiProjects.Add(cpi.Name, new List<Project> { tp });
                        }
                        else
                        {
                            var tpList = cpiProjects[cpi.Name];
                            tpList.Add(tp);
                            cpiProjects[cpi.Name] = tpList;
                        }
                        #endregion
                    }
                    else
                    {
                        #region  |  [no client]  |
                        if (!cpiNameDict.ContainsKey(@"[no client]"))
                        {
                            cpiNameDict.Add(@"[no client]", null);
                            nameKeys.Add(@"[no client]");
                            cpiProjects.Add(@"[no client]", new List<Project> { tp });
                        }
                        else
                        {
                            var tpList = cpiProjects[@"[no client]"];
                            tpList.Add(tp);
                            cpiProjects[@"[no client]"] = tpList;
                        }
                        #endregion
                    }

                    #endregion
                }
                nameKeys.Sort();
                #endregion

                var treeviewKeyIndex = new Dictionary<string, string>();
                var itreeviewKeyIndex = 0;

                if (groupBy.IndexOf(@"Client name", StringComparison.Ordinal) > -1)
                {
                    #region  |  group by Client name  |
                    foreach (var companyName in nameKeys)
                    {
                        var cpi = cpiNameDict[companyName];
                        var tnCompany = _viewNavigation.Value.treeView_navigation.Nodes.Add(companyName);

                        var iStatusCompleted = 0;
                        var iStatusInProgress = 0;

                        var strIndexes = new List<string>();
                        foreach (var tpProject in cpiProjects[companyName])
                        {
                            if (!strIndexes.Contains(tpProject.Name))
                                strIndexes.Add(tpProject.Name);
                        }
                        strIndexes.Sort();

                        tnCompany.Tag = cpi;
                        foreach (var strIndex in strIndexes)
                        {
                            foreach (var tpProject in cpiProjects[companyName])
                            {
                                if (tpProject.Name != strIndex) continue;
                                itreeviewKeyIndex++;
                                var tnProejct = tnCompany.Nodes.Add(itreeviewKeyIndex.ToString(), tpProject.Name);
                                treeviewKeyIndex.Add(tpProject.Id.ToString(), itreeviewKeyIndex.ToString());
                                if (tpProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                                {
                                    tnProejct.ImageKey = @"ProjectInProgress";
                                    tnProejct.SelectedImageKey = @"ProjectInProgress";
                                    iStatusInProgress++;
                                }
                                else
                                {
                                    tnProejct.ImageKey = @"ProjectCompleted";
                                    tnProejct.SelectedImageKey = @"ProjectCompleted";
                                    iStatusCompleted++;
                                }


                                if (!studioProjectListIds.Contains(tpProject.StudioProjectId))
                                {
                                    tnProejct.BackColor = Color.Yellow;
                                    tnProejct.ForeColor = Color.DarkBlue;
                                }

                                tnProejct.Tag = tpProject;
                            }
                        }

                        if (companyName == "[no client]")
                        {
                            tnCompany.ImageKey = @"Client";
                            tnCompany.SelectedImageKey = @"Client";
                        }
                        else
                        {
                            if (iStatusInProgress > 0 && iStatusCompleted > 0)
                            {
                                tnCompany.ImageKey = @"Client";
                                tnCompany.SelectedImageKey = @"Client";
                            }
                            else if (iStatusInProgress > 0 && iStatusCompleted == 0)
                            {
                                tnCompany.ImageKey = @"Client";
                                tnCompany.SelectedImageKey = @"Client";
                            }
                            else if (iStatusInProgress == 0 && iStatusCompleted > 0)
                            {
                                tnCompany.ImageKey = @"Client";
                                tnCompany.SelectedImageKey = @"Client";
                            }
                        }
                    }
                    #endregion
                }
                else if (groupBy.IndexOf(@"Project created", StringComparison.Ordinal) > -1)
                {
                    #region  |  projecct created date  |

                    var yearsIndex = new List<string>();
                    var monthsIndex = new List<string>();
                    foreach (var kvp in cpiDateCreated)
                    {
                        if (!yearsIndex.Contains(kvp.Key))
                            yearsIndex.Add(kvp.Key);

                        foreach (var _kvp in kvp.Value)
                        {
                            if (!monthsIndex.Contains(_kvp.Key))
                                monthsIndex.Add(_kvp.Key);
                        }
                    }
                    yearsIndex.Sort();
                    monthsIndex.Sort();

                    foreach (var yearIndex in yearsIndex)
                    {

                        foreach (var kvp in cpiDateCreated)
                        {
                            if (kvp.Key != yearIndex) continue;
                            var tnYear = _viewNavigation.Value.treeView_navigation.Nodes.Add(kvp.Key);
                            tnYear.ImageKey = @"Year";
                            tnYear.SelectedImageKey = @"Year";
                            tnYear.Tag = kvp.Value;

                            foreach (var monthIndex in monthsIndex)
                            {
                                foreach (var _kvp in kvp.Value)
                                {
                                    if (_kvp.Key != monthIndex) continue;
                                    var tnMonth = tnYear.Nodes.Add(Helper.GetMonthName(new DateTime(Convert.ToInt32(kvp.Key), Convert.ToInt32(_kvp.Key), 1)));
                                    tnMonth.ImageKey = @"MonthBlue";
                                    tnMonth.SelectedImageKey = @"MonthBlue";
                                    tnMonth.Tag = kvp.Value;


                                    var strIndexes = new List<string>();
                                    foreach (var tpProject in _kvp.Value)
                                    {
                                        if (!strIndexes.Contains(tpProject.Name))
                                            strIndexes.Add(tpProject.Name);
                                    }
                                    strIndexes.Sort();

                                    foreach (var strIndex in strIndexes)
                                    {
                                        foreach (var tpProject in _kvp.Value)
                                        {
                                            if (tpProject.Name != strIndex) continue;
                                            itreeviewKeyIndex++;
                                            var tnProejct = tnMonth.Nodes.Add(itreeviewKeyIndex.ToString(), tpProject.Name);
                                            treeviewKeyIndex.Add(tpProject.Id.ToString(), itreeviewKeyIndex.ToString());
                                            if (tpProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                                            {
                                                tnProejct.ImageKey = @"ProjectInProgress";
                                                tnProejct.SelectedImageKey = @"ProjectInProgress";
                                            }
                                            else
                                            {
                                                tnProejct.ImageKey = @"ProjectCompleted";
                                                tnProejct.SelectedImageKey = @"ProjectCompleted";
                                            }

                                            if (!studioProjectListIds.Contains(tpProject.StudioProjectId))
                                            {
                                                tnProejct.BackColor = Color.Yellow;
                                                tnProejct.ForeColor = Color.DarkBlue;
                                            }

                                            tnProejct.Tag = tpProject;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (groupBy.IndexOf(@"Project due", StringComparison.Ordinal) > -1)
                {
                    #region  |  project due date  |

                    var yearsIndex = new List<string>();
                    var monthsIndex = new List<string>();
                    foreach (var kvp in cpiDateDue)
                    {
                        if (!yearsIndex.Contains(kvp.Key))
                            yearsIndex.Add(kvp.Key);

                        foreach (var _kvp in kvp.Value)
                        {
                            if (!monthsIndex.Contains(_kvp.Key))
                                monthsIndex.Add(_kvp.Key);
                        }
                    }
                    yearsIndex.Sort();
                    monthsIndex.Sort();

                    foreach (var yearIndex in yearsIndex)
                    {
                        foreach (var kvp in cpiDateDue)
                        {
                            if (kvp.Key != yearIndex) continue;
                            var tnYear = _viewNavigation.Value.treeView_navigation.Nodes.Add(kvp.Key);
                            tnYear.ImageKey = @"Year";
                            tnYear.SelectedImageKey = @"Year";
                            tnYear.Tag = kvp.Value;

                            foreach (var monthIndex in monthsIndex)
                            {
                                foreach (var _kvp in kvp.Value)
                                {
                                    if (_kvp.Key != monthIndex) continue;
                                    var tnMonth = tnYear.Nodes.Add(Helper.GetMonthName(new DateTime(Convert.ToInt32(kvp.Key), Convert.ToInt32(_kvp.Key), 1)));
                                    tnMonth.ImageKey = @"MonthBlue";
                                    tnMonth.SelectedImageKey = @"MonthBlue";
                                    tnMonth.Tag = kvp.Value;

                                    var strIndexes = new List<string>();
                                    foreach (var tpProject in _kvp.Value)
                                    {
                                        if (!strIndexes.Contains(tpProject.Name))
                                            strIndexes.Add(tpProject.Name);
                                    }
                                    strIndexes.Sort();
                                    foreach (var strIndex in strIndexes)
                                    {
                                        foreach (var tpProject in _kvp.Value)
                                        {
                                            if (tpProject.Name != strIndex) continue;
                                            itreeviewKeyIndex++;
                                            var tnProejct = tnMonth.Nodes.Add(itreeviewKeyIndex.ToString(), tpProject.Name);
                                            treeviewKeyIndex.Add(tpProject.Id.ToString(), itreeviewKeyIndex.ToString());
                                            if (tpProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                                            {
                                                tnProejct.ImageKey = @"ProjectInProgress";
                                                tnProejct.SelectedImageKey = @"ProjectInProgress";
                                            }
                                            else
                                            {
                                                tnProejct.ImageKey = @"ProjectCompleted";
                                                tnProejct.SelectedImageKey = @"ProjectCompleted";
                                            }
                                            if (!studioProjectListIds.Contains(tpProject.StudioProjectId))
                                            {
                                                tnProejct.BackColor = Color.Yellow;
                                                tnProejct.ForeColor = Color.DarkBlue;
                                            }

                                            tnProejct.Tag = tpProject;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (groupBy.IndexOf("Project name", StringComparison.Ordinal) > -1)
                {
                    #region  |  group by Project name  |
                    foreach (var companyName in nameKeys)
                    {


                        var strIndexes = new List<string>();
                        foreach (var tpProject in cpiProjects[companyName])
                        {
                            if (!strIndexes.Contains(tpProject.Name))
                                strIndexes.Add(tpProject.Name);
                        }
                        strIndexes.Sort();


                        var cpi = cpiNameDict[companyName];

                        foreach (var strIndex in strIndexes)
                        {
                            foreach (var tpProject in cpiProjects[companyName])
                            {
                                if (tpProject.Name != strIndex) continue;
                                itreeviewKeyIndex++;
                                var tnProejct = _viewNavigation.Value.treeView_navigation.Nodes.Add(itreeviewKeyIndex.ToString(), tpProject.Name);
                                treeviewKeyIndex.Add(tpProject.Id.ToString(), itreeviewKeyIndex.ToString());
                                if (tpProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                                {
                                    tnProejct.ImageKey = @"ProjectInProgress";
                                    tnProejct.SelectedImageKey = @"ProjectInProgress";
                                }
                                else
                                {
                                    tnProejct.ImageKey = @"ProjectCompleted";
                                    tnProejct.SelectedImageKey = @"ProjectCompleted";
                                }

                                if (!studioProjectListIds.Contains(tpProject.StudioProjectId))
                                {
                                    tnProejct.BackColor = Color.Yellow;
                                    tnProejct.ForeColor = Color.DarkBlue;
                                }

                                tnProejct.Tag = tpProject;
                            }
                        }
                    }
                    #endregion
                }

                checkEnableExpandAll_Navigation_treeview();

                if (selectedProject != null)
                {
                    if (treeviewKeyIndex.ContainsKey(selectedProject.Id.ToString()))
                    {
                        var tn = _viewNavigation.Value.treeView_navigation.Nodes.Find(treeviewKeyIndex[selectedProject.Id.ToString()], true);
                        if (tn.Length > 0)
                        {
                            _viewNavigation.Value.treeView_navigation.SelectedNode = tn[0];
                            tn[0].EnsureVisible();
                        }
                    }
                }
                if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                {
                    if (_viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
                        _viewNavigation.Value.treeView_navigation.SelectedNode = _viewNavigation.Value.treeView_navigation.Nodes[0];
                }

                if (_viewNavigation.Value.treeView_navigation.Nodes.Count > 0 && _viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {
                    if (!_viewNavigation.Value.treeView_navigation.SelectedNode.IsExpanded)
                    {
                        if (_viewNavigation.Value.treeView_navigation.SelectedNode.Nodes.Count > 0)
                        {
                            _viewNavigation.Value.treeView_navigation.SelectedNode.Expand();
                        }
                    }
                }



                if (_viewNavigation.Value.treeView_navigation.Nodes.Count == 0)
                {
                    ClearDqfProjectList();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _viewNavigation.Value.treeView_navigation.EndUpdate();
            }
        }

        #endregion


        #region  |  Viewer control  |

        #region  |  ribbon properties  |

        public bool ProjectActivityNewEnabled { get; set; }
        public bool ProjectActivityEditEnabled { get; set; }
        public bool ProjectActivityRemoveEnabled { get; set; }


        public static bool ProjectActivityStartTrackerEnabled { get; set; }
        public static bool ProjectActivityStopTrackerEnabled { get; set; }

        public bool ProjectActivityDuplicateEnabled { get; set; }
        public bool ProjectActivityMergeEnabled { get; set; }

        public bool ProjectActivityCreateReportEnabled { get; set; }
        public bool ProjectActivityExportExcelEnabled { get; set; }
        #endregion


        private void InitializeViewControl()
        {

            QualitivityViewTrackChangesController.ObjectListView = _viewContent.Value.objectListView1;

            _viewContent.Value.olvColumn_activity_description.IsVisible = false;

            _viewContent.Value.objectListView1.RebuildColumns();

            _viewContent.Value.objectListView1.SelectionChanged += objectListView1_SelectionChanged;
            _viewContent.Value.objectListView1.DoubleClick += objectListView1_DoubleClick;
            _viewContent.Value.objectListView1.KeyUp += objectListView1_KeyUp;

            _viewContent.Value.contextMenuStrip_listView.Opening += contextMenuStrip_listView_Opening;
            _viewContent.Value.newProjectActivityToolStripMenuItem.Click += newProjectActivityToolStripMenuItem_Click;
            _viewContent.Value.editProjectActivityToolStripMenuItem.Click += editProjectActivityToolStripMenuItem_Click;
            _viewContent.Value.removeProjectActivityToolStripMenuItem.Click += removeProjectActivityToolStripMenuItem_Click;
            _viewContent.Value.duplicateTheProjectActivityToolStripMenuItem.Click += duplicateTheProjectActivityToolStripMenuItem_Click;
            _viewContent.Value.exportActivitiesToExcelToolStripMenuItem.Click += exportActivitesToExcelToolStripMenuItem_Click;
            _viewContent.Value.createAnActivitiesReportToolStripMenuItem.Click += createAnActivitiesReportToolStripMenuItem_Click;
            _viewContent.Value.mergeProjectActivitiesToolStripMenuItem.Click += mergeProjectActivitiesToolStripMenuItem_Click;
            _viewContent.Value.addDQFProjectTaskToolStripMenuItem.Click += addDQFProjectTaskToolStripMenuItem_Click;

        }

        private void contextMenuStrip_listView_Opening(object sender, CancelEventArgs e)
        {
            if (_viewContent.Value.objectListView1.SelectedItems.Count > 0 && QualitivityViewDqfController.Control.Value.treeView_dqf_projects.SelectedNode != null)
                _viewContent.Value.addDQFProjectTaskToolStripMenuItem.Enabled = true;
            else
                _viewContent.Value.addDQFProjectTaskToolStripMenuItem.Enabled = false;
        }

        private void mergeProjectActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeProjectActivities();
        }
        private void exportActivitesToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportActivitesToExcel();
        }
        private void createAnActivitiesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateActivitiesReport();
        }
        private void duplicateTheProjectActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateProjectActivity();
        }
        private void newProjectActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectActivity();
        }
        private void editProjectActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditProjectActivity();
        }
        private void removeProjectActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveProjectActivity();
        }

        private void objectListView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveProjectActivity();
        }
        private void objectListView1_DoubleClick(object sender, EventArgs e)
        {
            if (_viewContent.Value.objectListView1.SelectedObjects.Count > 0)
            {
                EditProjectActivity();
            }
        }
        private void objectListView1_SelectionChanged(object sender, EventArgs e)
        {
            CheckEnabledObjects();

            if (IsLoading)
                return;

            try
            {
                var documentActivities = new List<DocumentActivity>();


                SelectedActivities = new List<Activity>();

                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null && _viewContent.Value.objectListView1.SelectedObjects.Count > 0)
                {
                    var properties = new ActivityPropertiesView();

                    foreach (OLVListItem obj in _viewContent.Value.objectListView1.SelectedItems)
                    {
                        SelectedActivities.Add((Activity)obj.RowObject);
                    }

                    documentActivities = Helper.GetDocumentActivityObjects(SelectedActivities[0]);
                }
                if (SelectedActivities.Count > 0)
                {
                    SelectedProject = Helper.GetProjectFromId(SelectedActivities[0].ProjectId);

                    QualitivityViewDqfControl.Activity = SelectedActivities[0];
                    QualitivityViewDqfControl.Project = SelectedProject;
                    QualitivityViewDqfController.Control.Value.initialize_dqfProjects();
                }
                else
                {
                    QualitivityViewDqfControl.Activity = null;
                }

                QualitivityViewTrackChangesController.NavigationTreeView = _viewNavigation.Value.treeView_navigation;
                QualitivityViewTrackChangesController.ObjectListView = _viewContent.Value.objectListView1;
                QualitivityViewTrackChangesController.UpdateReportsArea(documentActivities, SelectedActivities.Count > 0 ? SelectedActivities[0] : null);
                QualitivityViewActivityRecordsController.UpdateReportsArea(SelectedActivities);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }


        public void NewProjectActivity()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            Tracked.TrackerLastActivity = DateTime.Now;

            Project project = null;
            var tps = new List<Project>();

            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
            {
                project = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                tps.Add(project);
            }
            else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null
                     || _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(CompanyProfile))
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Nodes.Count > 0)
                {
                    project = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Nodes[0].Tag;
                    tps.AddRange(from TreeNode tn in _viewNavigation.Value.treeView_navigation.SelectedNode.Nodes select (Project)tn.Tag);
                }
            }
            else if (_viewContent.Value.objectListView1.SelectedObjects.Count > 0)
            {
                var tpa = _viewContent.Value.objectListView1.SelectedObjects[0] as Activity;

                if (tpa != null) project = Helper.GetProjectFromId(tpa.ProjectId);
                tps.Add(project);
            }


            if (project == null) return;
            {
                var f = new TrackProjectActivity();

                var activity = new Activity { ProjectId = project.Id };

                CompanyProfile cpi = null;
                if (project.CompanyProfileId > -1)
                    cpi = Helper.GetClientFromId(project.CompanyProfileId);
                if (cpi != null)
                    activity.CompanyProfileId = cpi.Id;


                f.Activity = activity;
                f.Projects = tps;
                f.IsEdit = false;


                f.ShowDialog();
                if (!f.Saved) return;

                var query = new Query();
                f.Activity.Id = query.CreateActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, f.Activity);

                project.Activities.Add(f.Activity);
                FilterViewerControl(activity.Id);
                _viewContent.Value.objectListView1.SelectedObject = activity;


                if (f.CompanyProfile == null || f.CompanyProfile.Id == -1 || activity.CompanyProfileId != -1) return;

                Tracked.TarckerCheckNewProjectId = project.Id;
                activity.CompanyProfileId = f.CompanyProfile.Id;
                project.CompanyProfileId = activity.CompanyProfileId;

                foreach (var tpa in project.Activities)
                    tpa.CompanyProfileId = project.CompanyProfileId;

                query.UpdateProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, project);

                UpdateNavigationControl(project);
            }
        }
        public void EditProjectActivity()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            Tracked.TrackerLastActivity = DateTime.Now;
            if (_viewContent.Value.objectListView1.SelectedObjects.Count <= 0) return;
            var f = new TrackProjectActivity();

            if (_viewContent.Value.objectListView1.SelectedObjects.Count <= 0) return;
            var tpa = (Activity)_viewContent.Value.objectListView1.SelectedObjects[0];
            var tp = Helper.GetProjectFromId(tpa.ProjectId);
            var tps = new List<Project> { tp };
            tpa.ProjectId = tp.Id;

            f.Projects = tps;
            f.Activity = tpa;
            f.IsEdit = true;
            f.ShowDialog();
            if (!f.Saved) return;
            tpa = f.Activity;


            var query = new Query();
            query.UpdateActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, tpa);

            Application.DoEvents();

            //ensure that a new static versions gets created on first load
            query.DeleteActivityReports(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, tpa.Id);

            FilterViewerControl(tpa.Id);
            _viewContent.Value.objectListView1.SelectedObject = tpa;
        }
        public void RemoveProjectActivity()
        {
            Tracked.TrackerLastActivity = DateTime.Now;
            if (_viewContent.Value.objectListView1.Items.Count <= 0) return;
            var dr = MessageBox.Show(PluginResources.Are_you_sure_that_you_want_to_remove_the_selected_project_activities + "\r\n\r\n"
                                     + PluginResources.Note_you_will_not_be_able_to_recover_this_data_if_you_continue + "\r\n\r\n"
                                     + PluginResources.Click_Yes_to_continue_and_remove_the_project_activities + "\r\n"
                                     + PluginResources.Click_No_to_cancel
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;
            var activities = (from OLVListItem listViewItem in _viewContent.Value.objectListView1.SelectedItems select listViewItem.RowObject as Activity).ToList();

            _viewContent.Value.objectListView1.RemoveObjects(activities);

            var query = new Query();


            foreach (var project in Tracked.TrackingProjects.TrackerProjects)
            {
                foreach (var activity in activities)
                {
                    if (activity.ProjectId != project.Id) continue;
                    for (var i = 0; i < project.Activities.Count; i++)
                    {
                        if (activity.Id != project.Activities[i].Id) continue;

                        query.DeleteActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath
                            , Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + activity.ProjectId.ToString().PadLeft(6, '0')
                            , activity.Id, true);
                        project.Activities.Remove(activity);
                    }
                }
            }

            treeView_navigation_AfterSelect(null, null);
        }
        public void DuplicateProjectActivity()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            Tracked.TrackerLastActivity = DateTime.Now;
            if (_viewContent.Value.objectListView1.SelectedItems.Count <= 0) return;
            var activitesToClone = new List<Activity>();
            var activitesToCloned = new List<Activity>();
            for (var i = 0; i < _viewContent.Value.objectListView1.SelectedItems.Count; i++)
            {
                var itmx = (OLVListItem)_viewContent.Value.objectListView1.SelectedItems[i];
                var activity = (Activity)itmx.RowObject;
                activitesToClone.Add(activity);
            }

            var query = new Query();

            try
            {
                //ensure that this event gets removed
                query.ProgressChanged += TrackedController.ProgressChanged;


                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;

                ProgressWindow.ProgressDialog = new ProgressDialog();
                try
                {
                    ProgressWindow.ProgressDialogWorker = new BackgroundWorker();
                    ProgressWindow.ProgressDialogWorker.WorkerReportsProgress = true;
                    ProgressWindow.ProgressDialogWorker.DoWork += (sender, e) => activitesToCloned = TrackedController.DuplicateProjectActivity_DoWork(null, new DoWorkEventArgs(activitesToClone));
                    ProgressWindow.ProgressDialogWorker.RunWorkerCompleted += ProgressWindowHandlers.worker_RunWorkerCompleted;
                    ProgressWindow.ProgressDialogWorker.ProgressChanged += ProgressWindowHandlers.worker_ProgressChanged;
                    ProgressWindow.ProgressDialogWorker.RunWorkerAsync();
                    ProgressWindow.ProgressDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ProgressWindow.ProgressDialog.Dispose();
                }



                foreach (var activityCloned in activitesToCloned)
                {
                    var project = Helper.GetProjectFromId(activityCloned.ProjectId);
                    project.Activities.Add(activityCloned);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                query.ProgressChanged -= TrackedController.ProgressChanged;
            }


            if (activitesToCloned.Count <= 0) return;
            FilterViewerControl(activitesToCloned[0].Id);
            _viewContent.Value.objectListView1.SelectedObject = activitesToCloned[0];
        }
        public void MergeProjectActivities()
        {

            Tracked.TrackerLastActivity = DateTime.Now;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            var query = new Query();

            DateTime? dateStart = DateTime.Now.AddYears(200);
            DateTime? dateEnd = DateTime.Now.Subtract(new TimeSpan(10000, 0, 0, 0));
            double hourlyRateQuantity = 0;
            double customRateTotal = 0;

            var projectId = -1;
            var projectIdError = false;
            bool activityTypeWarning = false;

            var languageCombinations = new List<string>();

            var projectDocumentActivities = new List<DocumentActivities>();
            var documentActivities = new List<DocumentActivity>();
            var activities = new List<Activity>();

            foreach (OLVListItem itmx in _viewContent.Value.objectListView1.SelectedItems)
            {
                var activity = (Activity)itmx.RowObject;
                activities.Add(activity);

                hourlyRateQuantity += activity.DocumentActivityRates.HourlyRateQuantity;
                customRateTotal += activity.DocumentActivityRates.CustomRateTotal;


                foreach (var currentProjectDocumentActivities in activity.Activities)
                {
                    if (!languageCombinations.Contains(currentProjectDocumentActivities.TranslatableDocument.SourceLanguage + "-" + currentProjectDocumentActivities.TranslatableDocument.TargetLanguage))
                        languageCombinations.Add(currentProjectDocumentActivities.TranslatableDocument.SourceLanguage + "-" + currentProjectDocumentActivities.TranslatableDocument.TargetLanguage);

                    if (projectDocumentActivities.Exists(x => x.Id == currentProjectDocumentActivities.Id))
                    {
                        var ptd = projectDocumentActivities.Find(x => x.Id == currentProjectDocumentActivities.Id);
                        foreach (var tcaid in currentProjectDocumentActivities.DocumentActivityIds)
                            if (!ptd.DocumentActivityIds.Contains(tcaid))
                                ptd.DocumentActivityIds.Add(tcaid);
                    }
                    else
                    {
                        projectDocumentActivities.Add(currentProjectDocumentActivities.Clone() as DocumentActivities);
                    }
                }

                var existingDocumentActivities = query.GetDocumentActivities(
                    Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + activity.ProjectId.ToString().PadLeft(6, '0')
                    , Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath
                    , activity.Id, null);
                foreach (var existingDocumentActivity in existingDocumentActivities)
                    if (!documentActivities.Exists(a => a.Id == existingDocumentActivity.Id))
                        documentActivities.Add(existingDocumentActivity);


                DateTime? ds = null;
                DateTime? de = null;
                if (existingDocumentActivities.Count > 0)
                {
                    #region  |  get dates from documentActivity  |

                    foreach (var documentActivity in existingDocumentActivities)
                    {
                        if (!ds.HasValue)
                            ds = documentActivity.Started;
                        if (!de.HasValue)
                            de = documentActivity.Stopped;

                        if (ds > documentActivity.Started)
                            ds = documentActivity.Started;
                        if (de < documentActivity.Stopped)
                            de = documentActivity.Stopped;
                    }
                    #endregion
                }
                else
                {
                    #region  |  get dates from projectActivity  |

                    if (!ds.HasValue)
                        ds = activity.Started;
                    if (!de.HasValue)
                        de = activity.Stopped;

                    if (ds > activity.Started)
                        ds = activity.Started;
                    if (de < activity.Stopped)
                        de = activity.Stopped;

                    #endregion
                }

                if (ds < dateStart)
                    dateStart = ds;
                if (de > dateEnd)
                    dateEnd = de;

                #region  |  check for errors/warnings  |



                if (projectId > -1)
                {
                    if (projectId != activity.ProjectId)
                        projectIdError = true;
                }
                else
                    projectId = activity.ProjectId;

                #endregion
            }


            if (projectIdError)
            {
                MessageBox.Show(PluginResources.Unable_to_merge_activities_that_belong_to_different_projects, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (languageCombinations.Count > 1)
            {
                MessageBox.Show(PluginResources.Unable_to_merge_activities_with_different_language_combinations, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var project = Helper.GetProjectFromId(projectId);


                var continueMerge = true;
                #region  |  get flattened words  |

                var foundDuplicateSegments = false;
                var keyList = new List<string>();
                foreach (var documentActivity in documentActivities)
                {
                    foreach (var record in documentActivity.Records)
                    {
                        if (!keyList.Contains(record.ParagraphId + record.SegmentId))
                        {
                            keyList.Add(record.ParagraphId + record.SegmentId);
                        }
                        else
                            foundDuplicateSegments = true;
                    }
                }
                #endregion


                if (foundDuplicateSegments)
                {
                    #region  |  foundDuplicateSegments  |
                    var dr0 = MessageBox.Show(
                        PluginResources.Warning_You_are_merging_activities_with_tracking_information_where_the_same_segments_exist_in_multiple_documents_ + "\r\n"
                        + PluginResources.Note_This_might_cuase_a_discrepancy_with_the_word_count_total_if_you_are_accumulating_word_counts_from_ + "\r\n\r\n"
                        + PluginResources.Do_you_want_to_accumulate_the_word_count_from_the_duplicated_segments + "\r\n\r\n"
                        + PluginResources.Click_Yes_to_accumulate_the_word_counts + "\r\n"
                        + PluginResources.Click_No_to_exclude_the_duplicated_word_counts
                        , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr0 == DialogResult.No)
                        ;
                    #endregion
                }

                if (!continueMerge) return;

                if (activityTypeWarning)
                {
                    #region  |  activity_type_warning  |
                    var dr = MessageBox.Show(PluginResources.Located_multiple_activity_types_from_the_selected_records + "\r\n\r\n"
                                             + PluginResources.Are_you_sure_that_you_want_to_continue_merging_these_records + "\r\n\r\n"
                                             + PluginResources.Click_Yes_to_continue + "\r\n"
                                             + PluginResources.Click_No_to_cancel, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.No)
                        continueMerge = false;
                    #endregion
                }
                if (!continueMerge) return;
                if (project == null) return;
                var f = new TrackProjectActivity();


                #region  |  CH.Dialogs.TrackProjectActivity  |

                //PH - 2015-05-09
                //it is easier to use one of the existing activites that are being merged as the new base
                //this will avoid unessesarily removing (only to reinsert the same record data at a later stage) 
                //too much data which will deterioriate performance during that process.
                var mergedActivity = activities[0];
                mergedActivity.Started = dateStart;
                mergedActivity.Stopped = dateEnd;


                mergedActivity.Activities = projectDocumentActivities;
                mergedActivity.ProjectId = project.Id;
                mergedActivity.Name = activities[0].Name;

                mergedActivity.DocumentActivityRates = activities[0].DocumentActivityRates;
                mergedActivity.DocumentActivityRates.HourlyRateQuantity = hourlyRateQuantity;
                mergedActivity.DocumentActivityRates.CustomRateTotal = customRateTotal;

                if (project.CompanyProfileId > -1)
                {
                    var cpi = Helper.GetClientFromId(project.CompanyProfileId);
                    if (cpi != null)
                        project.CompanyProfileId = cpi.Id;
                }

                mergedActivity.CompanyProfileId = project.CompanyProfileId;


                f.Projects = new List<Project> { project };
                f.DocumentActivities = documentActivities;
                f.Activity = mergedActivity;
                f.IsEdit = true;
                f.IsMerge = true;

                #endregion


                f.ShowDialog();
                if (!f.Saved) return;

                #region  |  save the activity |



                foreach (var activity in activities)
                {
                    //do not removed the version of the activity that we are now using as the base for the merged file
                    if (activity.Id != mergedActivity.Id)
                    {
                        query.DeleteActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath
                            , Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + activity.ProjectId.ToString().PadLeft(6, '0')
                            , activity.Id, false);
                        _viewContent.Value.objectListView1.RemoveObject(activity);
                    }
                }


                //ensure that a new static versions gets created on first load
                query.DeleteActivityReports(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, f.Activity.Id);

                foreach (var activity in activities)
                {
                    if (activity.Id != mergedActivity.Id)
                    {
                        project.Activities.RemoveAll(a => a.Id == activity.Id);
                    }
                }


                query.UpdateActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, f.Activity);


                foreach (var documentActivity in documentActivities)
                {
                    //no need to update the document activities that were already associated with the project activity 
                    //that was used as the new base for the merged activity
                    if (documentActivity.ProjectActivityId != mergedActivity.Id)
                    {
                        documentActivity.ProjectActivityId = mergedActivity.Id;
                    }
                }


                ProgressWindow.ProgressDialog = new ProgressDialog();
                try
                {
                    ProgressWindow.ProgressDialogWorker = new BackgroundWorker();
                    ProgressWindow.ProgressDialogWorker.WorkerReportsProgress = true;



                    ProgressWindow.ProgressDialogWorker.DoWork += (sender, e) => documentActivities = TrackedController.UpdateDocumentActivity_DoWork(null, new DoWorkEventArgs(documentActivities));

                    ProgressWindow.ProgressDialogWorker.RunWorkerCompleted += ProgressWindowHandlers.worker_RunWorkerCompleted;
                    ProgressWindow.ProgressDialogWorker.ProgressChanged += ProgressWindowHandlers.worker_ProgressChanged;
                    ProgressWindow.ProgressDialogWorker.RunWorkerAsync();
                    ProgressWindow.ProgressDialog.ShowDialog();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ProgressWindow.ProgressDialog.Dispose();
                }


                Application.DoEvents();
                //ensure that a new static versions gets created on first load
                query.DeleteActivityReports(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, f.Activity.Id);

                treeView_navigation_AfterSelect(null, null);
                _viewContent.Value.objectListView1.SelectedObject = f.Activity;

                #endregion
            }

        }
        public void ExportActivitesToExcel()
        {

            Tracked.TrackerLastActivity = DateTime.Now;
            var f = new ExportActivities();

            var idsSelected = new List<int>();
            var idsAll = new List<int>();
            if (_viewContent.Value.objectListView1.Items.Count > 0)
            {
                foreach (OLVListItem itmx in _viewContent.Value.objectListView1.Items)
                {

                    var tpa = (Activity)itmx.RowObject;
                    if (itmx.Selected)
                        idsSelected.Add(tpa.Id);

                    idsAll.Add(tpa.Id);
                }
            }

            f.IdsSelected = idsSelected;
            f.IdsAll = idsAll;
            f.ShowDialog();

        }
        public void CreateActivitiesReport()
        {

            Tracked.TrackerLastActivity = DateTime.Now;


            var f = new ActivityReportWizard
            {
                SelectedProject = SelectedProject,
                SelectedActivities = SelectedActivities
            };

            f.ShowDialog();
        }


        public void StartTimeTracking()
        {
            try
            {
                Tracked.HandlerPartent = 0;

                try
                {
                    if (_viewContent.Value != null && _viewContent.Value.Parent != null)
                        _viewContent.Value.Parent.Cursor = Cursors.Default;
                }
                catch
                {
                    // ignored
                }


                ProjectActivityStartTrackerEnabled = false;
                ProjectActivityStopTrackerEnabled = true;

                if (CheckEnabledObjectsEvent != null)
                {
                    CheckEnabledObjectsEvent(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                TrackedActions.start_tracking(GetEditorController(), Timer4ProjectArea, true);
            }





        }
        public void StopTimeTracking()
        {
            try
            {
                Tracked.HandlerPartent = 0;

                ProjectActivityStartTrackerEnabled = true;
                ProjectActivityStopTrackerEnabled = false;

                if (CheckEnabledObjectsEvent != null)
                    CheckEnabledObjectsEvent(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                TrackedActions.stop_tracking(GetEditorController(), Timer4ProjectArea);
            }
        }


        private static void FillActivityDataView(IEnumerable<Activity> projectActivities)
        {
            _viewContent.Value.objectListView1.BeginUpdate();
            try
            {

                var projectStatus = _viewNavigation.Value.comboBox_project_status.SelectedItem.ToString().Trim();
                var activityStatus = _viewNavigation.Value.comboBox_activity_status.SelectedItem.ToString().Trim();
                var filterName = _viewNavigation.Value.comboBox_filter_name.SelectedItem.ToString().Trim();
                var nameSearch = _viewNavigation.Value.textBox_filter_name.Text.Trim();


                if (_viewNavigation.Value.comboBox_activity_status.SelectedIndex != 0
                    || (_viewNavigation.Value.comboBox_filter_name.SelectedIndex != 0 && nameSearch.Trim() != string.Empty))
                {

                    var projectActivitiesClone = new List<Activity>();
                    foreach (var tpa in projectActivities)
                    {
                        var filterStatusActivity = true;
                        if (_viewNavigation.Value.comboBox_activity_status.SelectedIndex > 0)
                        {
                            if (activityStatus.IndexOf("New", StringComparison.Ordinal) > -1)
                            {
                                if (tpa.ActivityStatus != Activity.Status.New)
                                    filterStatusActivity = false;
                            }
                            else if (activityStatus.IndexOf("Confirmed", StringComparison.Ordinal) > -1)
                            {
                                if (tpa.ActivityStatus != Activity.Status.Confirmed)
                                    filterStatusActivity = false;
                            }
                        }

                        var filterSearch = true;
                        if (_viewNavigation.Value.comboBox_filter_name.SelectedIndex == 1 && nameSearch.Trim() != string.Empty)
                        {
                            if (tpa.Name.Trim().ToLower().IndexOf(nameSearch.ToLower(), StringComparison.Ordinal) <= -1)
                                filterSearch = false;
                        }

                        if (filterStatusActivity && filterSearch)
                        {
                            projectActivitiesClone.Add(tpa);
                        }
                    }
                    _viewContent.Value.objectListView1.SetObjects(projectActivitiesClone);
                }
                else
                {

                    _viewContent.Value.objectListView1.SetObjects(projectActivities);
                }


                if (_viewContent.Value.objectListView1.Items.Count > 0)
                    _viewContent.Value.objectListView1.SelectedIndex = 0;


                var projects = new List<int>();
                var activities = new List<int>();
                foreach (OLVListItem listViewItem in _viewContent.Value.objectListView1.Items)
                {
                    var tpa = (Activity)listViewItem.RowObject;

                    if (!projects.Contains(tpa.ProjectId))
                        projects.Add(tpa.ProjectId);

                    if (!activities.Contains(tpa.Id))
                        activities.Add(tpa.Id);
                }

                _viewContent.Value.label_TOTAL_PROJECTS.Text = projects.Count.ToString();
                _viewContent.Value.label_TOTAL_PROJECT_ACTIVITIES.Text = activities.Count.ToString();


                if (activities.Count != 0) return;
                //reset the items in the document reports area  
                QualitivityViewTrackChangesController.NavigationTreeView = _viewNavigation.Value.treeView_navigation;
                QualitivityViewTrackChangesController.ObjectListView = _viewContent.Value.objectListView1;
                QualitivityViewTrackChangesController.UpdateReportsArea(new List<DocumentActivity>(), new Activity());
                QualitivityViewActivityRecordsController.UpdateReportsArea(new List<Activity>());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
            finally
            {
                _viewContent.Value.objectListView1.EndUpdate();
            }
        }
        private static void FilterViewerControl(int selectedActivityId)
        {
            try
            {
                var projects = ProjectsController.GetProjects().ToList();
                var studioProjectListIds = new List<string>();
                foreach (var proj in projects)
                {
                    var pi = proj.GetProjectInfo();
                    if (!studioProjectListIds.Contains(pi.Id.ToString()))
                        studioProjectListIds.Add(pi.Id.ToString());
                }

                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {
                    if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                        && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Project))
                    {
                        #region  |  by project  |
                        var tp = (Project)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                        _viewContent.Value.label_viewer_header.Text = string.Format(PluginResources.Project_0, tp.Name);

                        var addToList = !(!studioProjectListIds.Contains(tp.StudioProjectId) && !_viewNavigation.Value.checkBox_include_unlisted_projects.Checked);

                        if (addToList)
                        {
                            FillActivityDataView(tp.Activities);
                        }


                        #endregion
                    }
                    else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null
                        || _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(CompanyProfile))
                    {
                        #region  |  by client  |
                        var cpiId = -1;
                        if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null)
                        {
                            _viewContent.Value.label_viewer_header.Text = @"[no client]";
                        }
                        else
                        {

                            var cpi = (CompanyProfile)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                            _viewContent.Value.label_viewer_header.Text = string.Format(PluginResources.Client_0, cpi.Name);
                            cpiId = cpi.Id;
                        }
                        var tps = GetFilteredProjects();


                        var activities = new List<Activity>();
                        foreach (var tp in tps)
                        {
                            var addToList = !(!studioProjectListIds.Contains(tp.StudioProjectId) && !_viewNavigation.Value.checkBox_include_unlisted_projects.Checked);

                            if (!addToList) continue;
                            if (tp.CompanyProfileId != cpiId) continue;
                            activities.AddRange(tp.Activities);
                        }


                        FillActivityDataView(activities);



                        #endregion
                    }
                    else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                        && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Dictionary<string, List<Project>>))
                    {
                        #region  |  by date  |
                        switch (_viewNavigation.Value.treeView_navigation.SelectedNode.Level)
                        {
                            case 0:
                                _viewContent.Value.label_viewer_header.Text = string.Format(PluginResources.Year_0, _viewNavigation.Value.treeView_navigation.SelectedNode.Text);
                                break;
                            case 1:
                                _viewContent.Value.label_viewer_header.Text = string.Format(PluginResources.Year_0_, _viewNavigation.Value.treeView_navigation.SelectedNode.Parent.Text);
                                _viewContent.Value.label_viewer_header.Text += string.Format(PluginResources.Month_0, _viewNavigation.Value.treeView_navigation.SelectedNode.Text);
                                break;
                        }

                        var tps = GetFilteredProjects();
                        var activities = new List<Activity>();
                        foreach (var tp in tps)
                        {
                            var addToList = !(!studioProjectListIds.Contains(tp.StudioProjectId) && !_viewNavigation.Value.checkBox_include_unlisted_projects.Checked);

                            if (!addToList) continue;
                            var filterDate = true;

                            switch (_viewNavigation.Value.treeView_navigation.SelectedNode.Level)
                            {
                                case 0:
                                    if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project created", StringComparison.Ordinal) > -1)
                                    {
                                        if (tp.Created != null && tp.Created.Value.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim())
                                        {
                                            filterDate = false;
                                        }
                                    }
                                    else if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project due", StringComparison.Ordinal) > -1)
                                    {
                                        if (tp.Due != null && tp.Due.Value.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim())
                                        {
                                            filterDate = false;
                                        }
                                    }
                                    break;
                                case 1:
                                    if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project created", StringComparison.Ordinal) > -1)
                                    {
                                        if (tp.Created != null && tp.Created.Value.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Parent.Text.Trim())
                                        {
                                            filterDate = false;
                                        }
                                        else
                                        {
                                            if (tp.Created != null)
                                            {
                                                var monthProject = Helper.GetMonthName(tp.Created.Value);
                                                if (string.Compare(monthProject, _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                                                {
                                                    filterDate = false;
                                                }
                                            }
                                        }
                                    }
                                    else if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project due", StringComparison.Ordinal) > -1)
                                    {
                                        if (tp.Due != null && tp.Due.Value.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Parent.Text.Trim())
                                        {
                                            filterDate = false;

                                        }
                                        else
                                        {
                                            if (tp.Due != null)
                                            {
                                                var monthProject = Helper.GetMonthName(tp.Due.Value);
                                                if (string.Compare(monthProject, _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                                                {
                                                    filterDate = false;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                            if (filterDate)
                            {
                                activities.AddRange(tp.Activities);
                            }
                        }

                        FillActivityDataView(activities);



                        #endregion
                    }
                }
                else
                {
                    FillActivityDataView(new List<Activity>());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName);
            }
        }


        #endregion


        #region  |  Revision Quality Metrics  |

        public void AddToRevisionMetrics()
        {
            if (Tracked.ActiveDocument.Selection == null || Tracked.ActiveDocument.Selection.Current == null) return;
            var qm = new QualityMetric { Content = Tracked.ActiveDocument.Selection.Current.ToString() };
            QualitivityRevisionController.AddNewQualityMetric(qm);
        }

        public void UpdateQualityMetricList()
        {
            #region  |  set the default current metric group what is specified for the company  |

            if ((Tracked.TrackingState == Tracked.TimerState.Started
                    || Tracked.TrackingState == Tracked.TimerState.Paused)
                    && Tracked.ActiveDocument != null)
            {
                var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (firstOrDefault != null && Tracked.DictCacheDocumentItems.ContainsKey(firstOrDefault.Id.ToString()))
                {
                    var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                    if (projectFile != null)
                    {
                        var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

                        var project = Helper.GetProjectFromId(trackedDocuments.ProjectId);
                        if (project.CompanyProfileId > -1)
                        {
                            var ci = Helper.GetClientFromId(project.CompanyProfileId);
                            if (ci != null && ci.Id > -1)
                            {
                                if (ci.MetricGroup.Id > -1)
                                {
                                    Tracked.Settings.QualityMetricGroup = ci.MetricGroup;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            QualitivityRevisionController.InitializeQualityQuickInsertMenu();
        }

        public static void CleanQualityMetricsDataContainer()
        {
            QualitivityRevisionController.CleanQualityMetricsDataContainer();
        }

        #endregion


        #region  |  DQF Project  |

        public bool DqfProjectEnabled { get; set; }
        public bool DqfProjectImportEnabled { get; set; }
        public bool DqfProjectSaveEnabled { get; set; }
        public bool DqfProjectTaskEnabled { get; set; }


        public void ClearDqfProjectList()
        {
            QualitivityViewDqfController.Control.Value.ClearDqfProjectList();
        }

        private void InitializeDqfControl()
        {
            QualitivityViewDqfController.Control.Value.ProjectSelectionChanged += Value_projectSelectionChanged;
        }

        private void Value_projectSelectionChanged()
        {
            CheckEnabledObjects();
        }

        private void addDQFProjectTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewDqfProjectTask();
        }


        public void CreateNewDqfProject()
        {

            if (Tracked.Settings.DqfSettings.UserKey.Trim() == string.Empty)
            {
                MessageBox.Show(PluginResources.The_DQF_API_key_cannot_be_null + "\r\n\r\n"
                                + PluginResources.To_create_a_TAUS_DQF_Project_you_must_first_save_your_DQF_API_key_in_the_plugin_setting_area
                    , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            CurrentSelectedProject = ProjectsController.SelectedProjects.FirstOrDefault();
            if (CurrentSelectedProject != null)
            {
                CurrentProjectInfo = CurrentSelectedProject.GetProjectInfo();
            }
            if (CurrentSelectedProject == null || CurrentProjectInfo == null) return;
            var project = Tracked.TrackingProjects.TrackerProjects.Find(a => a.StudioProjectId == CurrentProjectInfo.Id.ToString());
            if (project == null)
            {
                project = new Project
                {
                    Name = CurrentProjectInfo.Name,
                    Path = CurrentProjectInfo.LocalProjectFolder,
                    Description = CurrentProjectInfo.Description ?? string.Empty,
                    CompanyProfileId = -1,
                    Created = CurrentProjectInfo.CreatedAt,
                    Started = DateTime.Now,
                    Due = CurrentProjectInfo.DueDate ?? DateTime.Now.AddDays(7),
                    ProjectStatus = CurrentProjectInfo.IsCompleted ? @"Completed" : @"In progress"
                };
                project.Completed = project.Due;


                project.StudioProjectId = CurrentProjectInfo.Id.ToString();
                project.StudioProjectName = CurrentProjectInfo.Name;
                project.StudioProjectPath = CurrentProjectInfo.LocalProjectFolder;
                project.SourceLanguage = CurrentProjectInfo.SourceLanguage.CultureInfo.Name;

                var query = new Query();
                project.Id = query.CreateProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, project);
                Tracked.TrackingProjects.TrackerProjects.Add(project);
                UpdateNavigationControl(project);
            }

            var f = new DqfProjectCreate { DqfProject = new DqfProject { Name = project.Name } };
            f.ShowDialog();
            if (!f.Saved) return;

            try
            {
                var dqfProject = f.DqfProject;
                dqfProject.ProjectId = project.Id;
                dqfProject.ProjectIdStudio = project.StudioProjectId;
                dqfProject.SourceLanguage = project.SourceLanguage;
                dqfProject.Created = DateTime.Now;
                dqfProject.DqfPmanagerKey = Tracked.Settings.DqfSettings.UserKey;
                dqfProject.DqfProjectId = -1;
                dqfProject.DqfProjectKey = string.Empty;
                dqfProject.Imported = false;

                var processor = new global::Sdl.Community.DQF.Processor();
                var productivityProject = new ProductivityProject
                {
                    Name = dqfProject.Name,
                    QualityLevel = dqfProject.QualityLevel,
                    SourceLanguage = dqfProject.SourceLanguage,
                    Process = dqfProject.Process,
                    ContentType = dqfProject.ContentType,
                    Industry = dqfProject.Industry
                };
                productivityProject = processor.PostDqfProject(Tracked.Settings.DqfSettings.UserKey, productivityProject);
                dqfProject.DqfProjectId = productivityProject.ProjectId;
                dqfProject.DqfProjectKey = productivityProject.ProjectKey;

                var query = new Query();
                dqfProject.Id = query.CreateDqfProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, dqfProject);

                var tn = QualitivityViewDqfController.Control.Value.treeView_dqf_projects.Nodes.Add(dqfProject.Name);
                tn.ImageIndex = 0;
                tn.SelectedImageIndex = tn.ImageIndex;
                tn.Tag = dqfProject;

                project.DqfProjects.Add(dqfProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void CreateNewDqfProject2()
        {
            QualitivityViewDqfController.Control.Value.NewDqfProject();
        }
        public void ImportDqfProjectSettings()
        {
            QualitivityViewDqfController.Control.Value.ImportDqfProjectSettings();
        }
        public void SaveDqfProjectSettings()
        {
            QualitivityViewDqfController.Control.Value.SaveDqfProjectSettings();
        }
        public void AddNewDqfProjectTask()
        {
            if (QualitivityViewDqfController.Control.Value.treeView_dqf_projects.SelectedNode == null) return;
            if (Tracked.Settings.DqfSettings.TranslatorKey.Trim() == string.Empty)
            {
                MessageBox.Show(PluginResources.The_Translator_DQF_API_key_cannot_be_null + "\r\n\r\n"
                                + PluginResources.To_create_TAUS_DQF_Project_Task_you_must_first_save_your_Translator_DQF_API_key_
                    , Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var dqfProject = QualitivityViewDqfController.Control.Value.treeView_dqf_projects.SelectedNode.Tag as DqfProject;

                var selectedActivity = (Activity)_viewContent.Value.objectListView1.SelectedObjects[0];

                var f = new DqfAddTasks();
                f.DqfProject = dqfProject;
                f.Project = SelectedProject;
                f.Activity = selectedActivity;
                f.DocumentActivities = Helper.GetDocumentActivityObjects(selectedActivity);
                f.ShowDialog();
                if (f.Finished)
                {
                    QualitivityViewDqfController.Control.Value.treeView_dqf_projects_AfterSelect(null, null);
                }
            }
        }

        #endregion


        #region  |  Settings  |

        public void LoadSettings(int index = 0)
        {

            Tracked.TrackerLastActivity = DateTime.Now;


            var f = new Dialogs.Settings { settings = (Settings)Tracked.Settings.Clone() };
            f.treeView_main.SelectedNode = f.treeView_main.Nodes[index];

            f.ShowDialog();

            if (f.Saved)
            {

                var query = new Query();
                Tracked.Settings = f.settings;

                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.BackUpSettings))
                {
                    query.SaveBackupSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.BackupSettings.BackupProperties);
                }

                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.GeneralSettings))
                {
                    query.SaveGeneralSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.GeneralSettings.GeneralProperties);
                }

                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.TrackingSettings))
                {
                    query.SaveTrackerSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.TrackingSettings.TrackingProperties);
                }

                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.DqfSettings))
                {
                    query.SaveDqfSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.DqfSettings);
                }




                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.QualityMetricsGroup))
                {
                    query.SaveQualityMetricGroupSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.QualityMetricGroupSettings.QualityMetricGroups);
                    UpdateQualityMetricList();
                }



                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.UserProfile))
                    query.SaveUserProfiles(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, new List<UserProfile> { f.settings.UserProfile });



                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.languageRateGroup))
                {
                    foreach (var changedItem in f.ChangedItems.FindAll(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.languageRateGroup))
                    {
                        switch (changedItem.Action)
                        {
                            case Dialogs.Settings.ChangedItem.ItemAction.Updated:
                                query.UpdateLanguageRateGroup(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.LanguageRateGroups.Find(a => { return a.Id == changedItem.Id; }));
                                break;
                            case Dialogs.Settings.ChangedItem.ItemAction.Deleted:
                                query.DeleteLanguageRateGroup(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, changedItem.Id);
                                break;
                        }
                    }
                    if (f.ChangedItems.Exists(a => (a.Type == Dialogs.Settings.ChangedItem.ItemType.languageRateGroup) && (a.Id == -1)))
                        query.SaveLanguageRateGroups(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.LanguageRateGroups.FindAll(a => { return a.Id == -1; }));

                }

                if (f.ChangedItems.Exists(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.CompanyProfile))
                {

                    foreach (var changedItem in f.ChangedItems.FindAll(a => a.Type == Dialogs.Settings.ChangedItem.ItemType.CompanyProfile))
                    {
                        switch (changedItem.Action)
                        {
                            case Dialogs.Settings.ChangedItem.ItemAction.Updated:
                                query.UpdateCompanyProfile(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.CompanyProfiles.Find(a => { return a.Id == changedItem.Id; }));
                                break;
                            case Dialogs.Settings.ChangedItem.ItemAction.Deleted:
                                query.DeleteCompanyProfile(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, changedItem.Id);
                                break;
                        }
                    }
                    if (f.ChangedItems.Exists(a => (a.Type == Dialogs.Settings.ChangedItem.ItemType.CompanyProfile) && (a.Id == -1)))
                        query.SaveCompanyProfiles(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, f.settings.CompanyProfiles.FindAll(a => { return a.Id == -1; }));
                }


                var groupBy = _viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().Trim();
                #region  |  ensure that the current node get updated  |
                if (groupBy.IndexOf(@"Client name", StringComparison.Ordinal) > -1)
                {
                    foreach (TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes)
                    {
                        if (tn.Tag == null || tn.Tag.GetType() != typeof(CompanyProfile)) continue;
                        var cpi = (CompanyProfile)tn.Tag;
                        foreach (var _cpi in Tracked.Settings.CompanyProfiles)
                        {
                            if (cpi.Id != _cpi.Id) continue;
                            tn.Text = _cpi.Name;
                            tn.Tag = _cpi;
                            break;
                        }
                    }
                }
                #endregion


                //ensure some independant cache settings are updated
                Viewer.IsTracking = Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("recordKeyStokes").Value);
                if (Viewer.IsTracking)
                    Viewer.StartTracking();
                else
                    Viewer.StopTracking();


                //refresh the naviagation area
                treeView_navigation_AfterSelect(null, null);
            }
        }

        #endregion


        public void ViewOnlineHelp()
        {
            MessageBox.Show("No help file found!");
        }
        public void ViewAboutInfo()
        {
            Tracked.TrackerLastActivity = DateTime.Now;
            var about = new About();
            about.ShowDialog();
        }


        private static void LoadCurrencies()
        {
            Tracked.Currencies = new List<Currency>();
            var index = new List<string>();
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(cultureInfo.Name);
                if (index.Contains(regionInfo.ISOCurrencySymbol)) continue;
                index.Add(regionInfo.ISOCurrencySymbol);
                Tracked.Currencies.Add(new Currency(regionInfo.ISOCurrencySymbol, regionInfo.CurrencySymbol, regionInfo.CurrencyEnglishName));
            }
        }

        private void Empty(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles()) file.Delete();
            foreach (var subDirectory in directory.GetDirectories()) subDirectory.Delete(true);

            directory.Delete(true);
        }
    }


}