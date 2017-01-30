using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Studio.Time.Tracker.Panels.Properties;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.Community.Studio.Time.Tracker.Tracking;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Main
{


    [View(
        Id = "Studio.Time.Tracker",
        Name = "Studio Time Tracker",
        Description = "Studio Time Tracker",
        Icon = "StudioTimeTrackerApp_Icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation),
        AllowViewParts = true
        )]
    public class StudioTimeTrackerViewController : AbstractViewController
    {

        private Timer Timer4ProjectArea { get; set; }

        private readonly Lazy<StudioTimeTrackerViewControl> _viewContent = new Lazy<StudioTimeTrackerViewControl>();
        private readonly Lazy<StudioTimeTrackerNavigationControl> _viewNavigation = new Lazy<StudioTimeTrackerNavigationControl>();


        public ProjectsController ProjectsController { get; set; }

        public ProjectInfo CurrentProjectInfo { get; set; }

        private bool IsLoading { get; set; }


        protected override Control GetContentControl()
        {
            return _viewContent.Value;
        }
        protected override Control GetExplorerBarControl()
        {
            return _viewNavigation.Value;
        }


        public event EventHandler CheckEnabledObjectsEvent;
        private void CheckEnabledObjects()
        {
            try
            {
                _viewNavigation.Value.newTimeTrackerProjectToolStripMenuItem.Enabled = TimeTrackerProjectNewEnabled;
                _viewNavigation.Value.editTimeTrackerProjectToolStripMenuItem.Enabled = TimeTrackerProjectEditEnabled;
                _viewNavigation.Value.removeTimeTrackerProjectToolStripMenuItem.Enabled = TimeTrackerProjectRemoveEnabled;


                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null && _viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
                {
                    if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                        && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                    {
                        ProjectActivityNewEnabled = true;
                        _viewNavigation.Value.newProjectActivityToolStripMenuItem.Enabled = true;
                        _viewContent.Value.newProjectActivityToolStripMenuItem.Enabled = true;

                    }
                    else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null
                        || _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(ClientProfileInfo))
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
                    FillActivityDataView(new List<TrackerProjectActivity>());

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

                    ProjectActivityDuplicateEnabled = false;
                    _viewContent.Value.duplicateTheProjectActivityToolStripMenuItem.Enabled = false;
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


                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {
                    ProjectActivityCreateReportEnabled = false;
                    ProjectActivityExportExcelEnabled = true;

                    _viewContent.Value.createAnActivitiesReportToolStripMenuItem.Enabled = false;
                    _viewContent.Value.exportActivitiesToExcelToolStripMenuItem.Enabled = true;
                }
                else
                {
                    ProjectActivityCreateReportEnabled = false;
                    ProjectActivityExportExcelEnabled = false;

                    _viewContent.Value.createAnActivitiesReportToolStripMenuItem.Enabled = false;
                    _viewContent.Value.exportActivitiesToExcelToolStripMenuItem.Enabled = false;
                }


                if (CheckEnabledObjectsEvent != null)
                {
                    CheckEnabledObjectsEvent(this, EventArgs.Empty);
                }
            }
            finally
            {
                UpdateActivityPropertiesViewer();
            }
        }


        private bool IsActive { get; set; }

        protected override void Initialize(IViewContext context)
        {
            ActivationChanged += StudioTimeTrackerViewController_ActivationChanged;

            IsLoading = true;

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



            #region  |  initialize the tracker cache  |


            Tracked.TrackingState = Tracked.TimerState.None;
            Tracked.TrackingTimer = new Stopwatch();
            Tracked.TrackingStart = Common.DateNull;
            Tracked.TrackingEnd = Common.DateNull;
            Tracked.TrackingPaused = new Stopwatch();


            Tracked.TrackerProjectIdStudio = string.Empty;
            Tracked.TrackerProjectNameStudio = string.Empty;
            Tracked.TrackerProjectId = string.Empty;
            Tracked.TrackerProjectName = string.Empty;

            Tracked.TrackerActivityName = string.Empty;
            Tracked.TrackerActivityDescription = string.Empty;
            Tracked.TrackerActivityType = string.Empty;

            Tracked.TrackerClientId = string.Empty;
            Tracked.TrackerClientName = string.Empty;


            #endregion


            LoadCurrencies();



            Tracked.Preferences = SettingsSerializer.ReadSettings();



            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

            InitializeNavigationControl();
            InitializeViewControl();


            CheckEnabledObjects();



            InitializeDocumentTrackingEvents();


            Timer4ProjectArea = new Timer { Interval = 1000 };
            Timer4ProjectArea.Tick += Timer4ProjectArea_Tick;
            Timer4ProjectArea.Start();


            IsLoading = false;
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
            try
            {
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

                        if (Tracked.TrackingState == Tracked.TimerState.Started
                            && Tracked.TrackerDocumentId.Trim() == string.Empty)
                        {
                            CheckStartTracker();
                        }

                        if (Tracked.HandlerPartent != 0)
                            treeView_navigation_AfterSelect(null, null);

                        #endregion
                    }

                    if (!Tracked.TarckerCheckNewProjectAdded && !Tracked.TarckerCheckNewActivityAdded) return;

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
                        FilterViewerControl();
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
            catch (Exception)
            {
                // ignored
            }
        }

        private void CheckRefreshNavigationNewProject()
        {

            TrackerProject selectedProject = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                {
                    selectedProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                }
                else
                {
                    foreach (var trackerProject in Tracked.Preferences.TrackerProjects)
                    {
                        if (trackerProject.Id != Tracked.TarckerCheckNewProjectId) continue;
                        selectedProject = trackerProject;
                        break;
                    }
                }
            }

            Tracked.TarckerCheckNewProjectAdded = false;
            Tracked.TarckerCheckNewProjectId = string.Empty;

            UpdateNavigationControl(selectedProject);



        }


        #region  |  tracking document activities  |

        private static EditorController GetEditorController()
        {
            return SdlTradosStudio.Application.GetController<EditorController>();
        }
        private static void InitializeDocumentTrackingEvents()
        {
            TrackedEditorEvents.InitializeDocumentTrackingEvents(GetEditorController());
        }

        private static void CheckStartTracker()
        {
            if (Tracked.TrackingState != Tracked.TimerState.Started || Tracked.TrackerDocumentId.Trim() != string.Empty)
                return;
            try
            {
                var doc = GetEditorController().ActiveDocument;
                if (doc == null)
                    return;

                var trackerProject = Helper.GetTrackerProjectFromDocument(doc);
                var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                #region  |  start new activity tracking  |


                #region  |  get activity type  |

                var activitiesType = Tracked.Preferences.ActivitiesTypes[0];
                foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
                {
                    if (string.Compare(activityType.Name, doc.Mode.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    activitiesType = activityType;
                    break;
                }
                #endregion

                Tracked.TrackerDocumentId = doc.ActiveFile.Id.ToString();
                Tracked.TrackingState = Tracked.TimerState.Started;
                Tracked.TrackingTimer = new Stopwatch();
                Tracked.TrackingTimer.Start();


                Tracked.TrackingStart = DateTime.Now;
                Tracked.TrackingEnd = Common.DateNull;

                Tracked.TrackingPaused = new Stopwatch();


                Tracked.TrackerProjectIdStudio = trackerProject.IdStudio;
                Tracked.TrackerProjectNameStudio = doc.Project.GetProjectInfo().Name;
                Tracked.TrackerProjectId = trackerProject.Id;
                Tracked.TrackerProjectName = trackerProject.Name;


                Tracked.TrackerActivityName = doc.ActiveFile.Name;
                Tracked.TrackerActivityDescription = string.Empty;
                Tracked.TrackerActivityType = activitiesType.Name;

                if (clientProfileInfo == null)
                    return;

                Tracked.TrackerClientId = clientProfileInfo.Id;
                Tracked.TrackerClientName = clientProfileInfo.ClientName;

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Tracked.TrackingIsDirtyC0 = true;
                Tracked.TrackingIsDirtyC1 = true;
                Tracked.TrackingIsDirtyC2 = true;
            }
        }



        #endregion

        #region  |  navigation control  |

        public bool TimeTrackerProjectNewEnabled { get; set; }
        public bool TimeTrackerProjectEditEnabled { get; set; }
        public bool TimeTrackerProjectRemoveEnabled { get; set; }



        private void InitializeNavigationControl()
        {
            try
            {


                _viewNavigation.Value.comboBox_project_status.BeginUpdate();
                _viewNavigation.Value.comboBox_project_status.SelectedItem = Tracked.Preferences.DefaultFilterProjectStatus;
                _viewNavigation.Value.comboBox_project_status.EndUpdate();

                _viewNavigation.Value.comboBox_groupBy.BeginUpdate();
                _viewNavigation.Value.comboBox_groupBy.SelectedItem = Tracked.Preferences.DefaultFilterGroupBy;
                _viewNavigation.Value.comboBox_groupBy.EndUpdate();

                _viewNavigation.Value.button_project_search.Click += button_project_search_Click;
                _viewNavigation.Value.textBox_project_name.KeyUp += textBox_project_name_KeyUp;
                _viewNavigation.Value.button_auto_expand_treeview.Click += button_auto_expand_treeview_Click;


                _viewNavigation.Value.newTimeTrackerProjectToolStripMenuItem.Click += newTimeTrackerProjectToolStripMenuItem_Click;
                _viewNavigation.Value.editTimeTrackerProjectToolStripMenuItem.Click += editTimeTrackerProjectToolStripMenuItem_Click;
                _viewNavigation.Value.removeTimeTrackerProjectToolStripMenuItem.Click += removeTimeTrackerProjectToolStripMenuItem_Click;
                _viewNavigation.Value.newProjectActivityToolStripMenuItem.Click += newProjectActivityToolStripMenuItem_Click;

                _viewNavigation.Value.comboBox_project_status.SelectedIndexChanged += comboBox_project_status_SelectedIndexChanged;
                _viewNavigation.Value.comboBox_groupBy.SelectedIndexChanged += comboBox_groupBy_SelectedIndexChanged;

                _viewNavigation.Value.treeView_navigation.AfterExpand += treeView_navigation_AfterExpand;
                _viewNavigation.Value.treeView_navigation.AfterCollapse += treeView_navigation_AfterCollapse;
                _viewNavigation.Value.treeView_navigation.DoubleClick += treeView_navigation_DoubleClick;
                _viewNavigation.Value.treeView_navigation.AfterSelect += treeView_navigation_AfterSelect;
                _viewNavigation.Value.treeView_navigation.KeyUp += treeView_navigation_KeyUp;

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
            var timeTrackerProject = new Dialogs.TimeTrackerProject();

            var selectedProject = new TrackerProject
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now,
                DateStart = DateTime.Now,
                DateDue = DateTime.Now.AddMonths(1)
            };



            timeTrackerProject.TrackerProject = selectedProject;
            timeTrackerProject.Clients = Tracked.Preferences.Clients;
            timeTrackerProject.IsEdit = false;

            timeTrackerProject.ProjectsController = ProjectsController;

            timeTrackerProject.ShowDialog();
            if (!timeTrackerProject.Saved)
                return;

            Tracked.Preferences.TrackerProjects.Add(selectedProject);

            SettingsSerializer.SaveSettings(Tracked.Preferences);

            UpdateNavigationControl(selectedProject);
        }
        public void EditTimeTrackerProject()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null ||
                _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() !=
                typeof(TrackerProject)) return;

            var trackerProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

            var clientIdRef = trackerProject.ClientId;

            var projectNameRef = trackerProject.Name;
            var projectStatusRef = trackerProject.ProjectStatus;

            var timeTrackerProject = new Dialogs.TimeTrackerProject
            {
                TrackerProject = trackerProject,
                Clients = Tracked.Preferences.Clients,
                IsEdit = true,
                ProjectsController = ProjectsController
            };




            timeTrackerProject.ShowDialog();
            if (!timeTrackerProject.Saved)
                return;

            if (timeTrackerProject.TrackerProject.ClientId != clientIdRef)
            {
                var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                foreach (var trackerProjectActivity in trackerProject.ProjectActivities)
                {
                    if (clientProfileInfo != null)
                    {
                        trackerProjectActivity.ClientId = clientProfileInfo.Id;
                        trackerProjectActivity.ClientName = clientProfileInfo.ClientName;

                        foreach (var cat in clientProfileInfo.ClientActivities)
                        {
                            if (cat.IdActivity != trackerProjectActivity.ActivityTypeId)
                                continue;

                            trackerProjectActivity.ActivityTypeClientId = cat.Id;
                            break;
                        }
                    }
                    else
                    {
                        trackerProjectActivity.ClientId = string.Empty;
                        trackerProjectActivity.ClientName = string.Empty;
                        trackerProjectActivity.ActivityTypeClientId = string.Empty;
                    }
                }
            }

            if (timeTrackerProject.TrackerProject.Name != projectNameRef
                || timeTrackerProject.TrackerProject.ProjectStatus != projectStatusRef)
            {
                foreach (var trackerProjectActivity in trackerProject.ProjectActivities)
                {
                    trackerProjectActivity.TrackerProjectName = timeTrackerProject.TrackerProject.Name;
                    trackerProjectActivity.TrackerProjectStatus = timeTrackerProject.TrackerProject.ProjectStatus;
                }
            }

            SettingsSerializer.SaveSettings(Tracked.Preferences);

            UpdateNavigationControl(trackerProject);
        }
        public void RemoveTimeTrackerProject()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null ||
                _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() !=
                typeof(TrackerProject)) return;
            var selectedNodeTag = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

            var dr = MessageBox.Show(@"Are you sure that you want to remove this project and all its data?" + "\r\n\r\n"
                                     + @"Note: you will not be able to recover this data if you continue!" + "\r\n\r\n"
                                     + @"Click 'Yes' to continue and remove the project" + "\r\n"
                                     + @"Click 'No' to cancel"
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) 
                return;

            for (var i = 0; i < Tracked.Preferences.TrackerProjects.Count; i++)
            {
                var trackerProject = Tracked.Preferences.TrackerProjects[i];

                if (selectedNodeTag.Id != trackerProject.Id) 
                    continue;

                Tracked.Preferences.TrackerProjects.RemoveAt(i);
                break;
            }
            _viewNavigation.Value.treeView_navigation.SelectedNode.Remove();

            SettingsSerializer.SaveSettings(Tracked.Preferences);
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


        public void treeView_navigation_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (IsLoading)
                return;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                {
                    TimeTrackerProjectEditEnabled = true;
                    TimeTrackerProjectRemoveEnabled = true;

                }
                else
                {
                    TimeTrackerProjectEditEnabled = false;
                    TimeTrackerProjectRemoveEnabled = false;
                }
            }
            else
            {
                TimeTrackerProjectEditEnabled = false;
                TimeTrackerProjectRemoveEnabled = false;
            }

            FilterViewerControl();
            CheckEnabledObjects();
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
            if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"In progress", StringComparison.Ordinal) > -1)
            {
                _viewNavigation.Value.button_auto_expand_treeview.Enabled = false;
            }
            else
            {
                var isAllExpanded = true;
                foreach (TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes)
                {
                    if (tn.Nodes.Count <= 0)
                        continue;

                    if (!tn.IsExpanded)
                    {
                        isAllExpanded = false;
                        break;
                    }
                    if (tn.Nodes.Cast<TreeNode>().Where(treeNode => treeNode.Nodes.Count > 0).Any(treeNode => !treeNode.IsExpanded))
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

        private void textBox_project_name_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsLoading)
                return;

            if (e.KeyCode != Keys.Return)
                return;

            TrackerProject selectedProject = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                {
                    selectedProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

                }
            }
            UpdateNavigationControl(selectedProject);
            CheckEnabledObjects();
        }

        private void button_project_search_Click(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            TrackerProject selectedProject = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                {
                    selectedProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

                }
            }
            UpdateNavigationControl(selectedProject);
            CheckEnabledObjects();
        }

        private void comboBox_groupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            TrackerProject selectedProject = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                {
                    selectedProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;

                }
            }
            UpdateNavigationControl(selectedProject);
            CheckEnabledObjects();
        }

        private void comboBox_project_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            TrackerProject selectedProject = null;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                    && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                {
                    selectedProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                }
            }
            UpdateNavigationControl(selectedProject);
            CheckEnabledObjects();
        }


        private IEnumerable<TrackerProject> GetFilteredProjects()
        {
            var filteredProjects = new List<TrackerProject>();


            //Show all projects
            //In progress
            //Completed
            var projectStatus = _viewNavigation.Value.comboBox_project_status.SelectedItem.ToString().Trim();
            var projectSearch = _viewNavigation.Value.textBox_project_name.Text.Trim();




            foreach (var trackerProject in Tracked.Preferences.TrackerProjects)
            {
                #region  |  filter?  |

                var filterStatus = true;
                if (projectStatus.Trim() != string.Empty)
                {
                    if (projectStatus.IndexOf("In progress", StringComparison.Ordinal) > -1)
                    {
                        if (trackerProject.ProjectStatus.IndexOf("In progress", StringComparison.Ordinal) <= -1)
                            filterStatus = false;
                    }
                    else if (projectStatus.IndexOf("Completed", StringComparison.Ordinal) > -1)
                    {
                        if (trackerProject.ProjectStatus.IndexOf("Completed", StringComparison.Ordinal) <= -1)
                            filterStatus = false;
                    }
                }

                var filterSearch = true;
                if (projectSearch.Trim() != string.Empty)
                {
                    if (trackerProject.Name.Trim().ToLower().IndexOf(projectSearch.ToLower(), StringComparison.Ordinal) <= -1)
                        filterSearch = false;
                }

                #endregion

                if (filterStatus && filterSearch)
                {
                    filteredProjects.Add(trackerProject);
                }
            }

            return filteredProjects;
        }

        public void UpdateNavigationControl(TrackerProject selectedProject)
        {
            ////Show all projects
            ////In progress
            ////Completed
            //string project_status = this._viewNavigation.Value.comboBox_project_status.SelectedItem.ToString().Trim();
            //string project_search = this._viewNavigation.Value.textBox_project_name.Text.Trim();



            ////Client name
            ////Project name
            ////Date (year/month)
            var groupBy = _viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().Trim();


            try
            {
                _viewNavigation.Value.treeView_navigation.BeginUpdate();

                _viewNavigation.Value.treeView_navigation.Nodes.Clear();

                #region  |  get index  |
                var cpiNameDict = new Dictionary<string, ClientProfileInfo>();
                var cpiProjects = new Dictionary<string, List<TrackerProject>>();
                var nameKeys = new List<string>();
                var cpiDateCreated = new Dictionary<string, Dictionary<string, List<TrackerProject>>>();
                var cpiDateDue = new Dictionary<string, Dictionary<string, List<TrackerProject>>>();

                var filteredProjects = GetFilteredProjects();
                foreach (var trackerProject in filteredProjects)
                {

                    #region  |  build the structures  |

                    #region  |  by date created  |

                    if (cpiDateCreated.ContainsKey(trackerProject.DateCreated.Year.ToString()))
                    {
                        var tpCreatedYear = cpiDateCreated[trackerProject.DateCreated.Year.ToString()];

                        if (tpCreatedYear.ContainsKey(trackerProject.DateCreated.Month.ToString().PadLeft(2, '0')))
                        {
                            var tpCreatedMonth = tpCreatedYear[trackerProject.DateCreated.Month.ToString().PadLeft(2, '0')];
                            tpCreatedMonth.Add(trackerProject);
                            tpCreatedYear[trackerProject.DateCreated.Month.ToString().PadLeft(2, '0')] = tpCreatedMonth;
                        }
                        else
                        {
                            var tpCreatedMonth = new List<TrackerProject> { trackerProject };
                            tpCreatedYear.Add(trackerProject.DateCreated.Month.ToString().PadLeft(2, '0'), tpCreatedMonth);
                        }

                        cpiDateCreated[trackerProject.DateCreated.Year.ToString()] = tpCreatedYear;
                    }
                    else
                    {

                        var tpCreatedYear = new Dictionary<string, List<TrackerProject>>();

                        var tpCreatedMonth = new List<TrackerProject> { trackerProject };
                        tpCreatedYear.Add(trackerProject.DateCreated.Month.ToString().PadLeft(2, '0'), tpCreatedMonth);


                        cpiDateCreated.Add(trackerProject.DateCreated.Year.ToString(), tpCreatedYear);
                    }
                    #endregion


                    #region  |  by date due  |

                    if (cpiDateDue.ContainsKey(trackerProject.DateDue.Year.ToString()))
                    {
                        var tpDueYear = cpiDateDue[trackerProject.DateDue.Year.ToString()];

                        if (tpDueYear.ContainsKey(trackerProject.DateDue.Month.ToString().PadLeft(2, '0')))
                        {
                            var tpDueMonth = tpDueYear[trackerProject.DateDue.Month.ToString().PadLeft(2, '0')];
                            tpDueMonth.Add(trackerProject);
                            tpDueYear[trackerProject.DateDue.Month.ToString().PadLeft(2, '0')] = tpDueMonth;
                        }
                        else
                        {
                            var tpDueMonth = new List<TrackerProject> { trackerProject };
                            tpDueYear.Add(trackerProject.DateDue.Month.ToString().PadLeft(2, '0'), tpDueMonth);
                        }

                        cpiDateDue[trackerProject.DateDue.Year.ToString()] = tpDueYear;
                    }
                    else
                    {

                        var tpDueYear = new Dictionary<string, List<TrackerProject>>();

                        var tpDueMonth = new List<TrackerProject> { trackerProject };
                        tpDueYear.Add(trackerProject.DateDue.Month.ToString().PadLeft(2, '0'), tpDueMonth);


                        cpiDateDue.Add(trackerProject.DateDue.Year.ToString(), tpDueYear);
                    }
                    #endregion


                    if (trackerProject.ClientId.Trim() != string.Empty)
                    {
                        #region  |  has client  |
                        var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                        if (!cpiNameDict.ContainsKey(clientProfileInfo.ClientName.Trim()))
                        {
                            cpiNameDict.Add(clientProfileInfo.ClientName.Trim(), clientProfileInfo);
                            nameKeys.Add(clientProfileInfo.ClientName.Trim());
                            cpiProjects.Add(clientProfileInfo.ClientName, new List<TrackerProject> { trackerProject });
                        }
                        else
                        {
                            var trackerProjects = cpiProjects[clientProfileInfo.ClientName];
                            trackerProjects.Add(trackerProject);
                            cpiProjects[clientProfileInfo.ClientName] = trackerProjects;
                        }
                        #endregion
                    }
                    else
                    {
                        #region  |  [no client]  |
                        if (!cpiNameDict.ContainsKey("[no client]"))
                        {
                            cpiNameDict.Add("[no client]", null);
                            nameKeys.Add("[no client]");
                            cpiProjects.Add("[no client]", new List<TrackerProject> { trackerProject });
                        }
                        else
                        {
                            var trackerProjects = cpiProjects["[no client]"];
                            trackerProjects.Add(trackerProject);
                            cpiProjects["[no client]"] = trackerProjects;
                        }
                        #endregion
                    }

                    #endregion

                }
                nameKeys.Sort();
                #endregion

                var treeviewKeyIndex = new Dictionary<string, string>();
                var itreeviewKeyIndex = 0;

                if (groupBy.IndexOf("Client name", StringComparison.Ordinal) > -1)
                {
                    #region  |  group by Client name  |
                    foreach (var companyName in nameKeys)
                    {
                        var clientProfileInfo = cpiNameDict[companyName];


                        var tnCompany = _viewNavigation.Value.treeView_navigation.Nodes.Add(companyName);


                        var iStatusCompleted = 0;
                        var iStatusInProgress = 0;

                        tnCompany.Tag = clientProfileInfo;
                        foreach (var tpProject in cpiProjects[companyName])
                        {
                            itreeviewKeyIndex++;
                            var tnProejct = tnCompany.Nodes.Add(itreeviewKeyIndex.ToString(), tpProject.Name);
                            treeviewKeyIndex.Add(tpProject.Id, itreeviewKeyIndex.ToString());
                            if (tpProject.ProjectStatus.IndexOf("In progress", StringComparison.Ordinal) > -1)
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
                            tnProejct.Tag = tpProject;
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
                    foreach (var keyValuePair in cpiDateCreated)
                    {
                        if (!yearsIndex.Contains(keyValuePair.Key))
                            yearsIndex.Add(keyValuePair.Key);

                        foreach (var valuePair in keyValuePair.Value)
                        {
                            if (!monthsIndex.Contains(valuePair.Key))
                                monthsIndex.Add(valuePair.Key);
                        }
                    }
                    yearsIndex.Sort();
                    monthsIndex.Sort();

                    foreach (var yearIndex in yearsIndex)
                    {

                        foreach (var keyValuePair in cpiDateCreated)
                        {
                            if (keyValuePair.Key != yearIndex)
                                continue;

                            var tnYear = _viewNavigation.Value.treeView_navigation.Nodes.Add(keyValuePair.Key);
                            tnYear.ImageKey = @"Year";
                            tnYear.SelectedImageKey = @"Year";
                            tnYear.Tag = keyValuePair.Value;

                            foreach (var monthIndex in monthsIndex)
                            {
                                foreach (var valuePair in keyValuePair.Value)
                                {
                                    if (valuePair.Key != monthIndex)
                                        continue;

                                    var tnMonth = tnYear.Nodes.Add(Common.GetMonthName(new DateTime(Convert.ToInt32(keyValuePair.Key), Convert.ToInt32(valuePair.Key), 1)));
                                    tnMonth.ImageKey = @"MonthBlue";
                                    tnMonth.SelectedImageKey = @"MonthBlue";
                                    tnMonth.Tag = keyValuePair.Value;

                                    foreach (var tpProject in valuePair.Value)
                                    {
                                        itreeviewKeyIndex++;
                                        var tnProejct = tnMonth.Nodes.Add(itreeviewKeyIndex.ToString(), tpProject.Name);
                                        treeviewKeyIndex.Add(tpProject.Id, itreeviewKeyIndex.ToString());
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
                                        tnProejct.Tag = tpProject;
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
                    foreach (var keyValuePair in cpiDateDue)
                    {
                        if (!yearsIndex.Contains(keyValuePair.Key))
                            yearsIndex.Add(keyValuePair.Key);

                        foreach (var valuePair in keyValuePair.Value)
                        {
                            if (!monthsIndex.Contains(valuePair.Key))
                                monthsIndex.Add(valuePair.Key);
                        }
                    }
                    yearsIndex.Sort();
                    monthsIndex.Sort();

                    foreach (var yearIndex in yearsIndex)
                    {
                        foreach (var keyValuePair in cpiDateDue)
                        {
                            if (keyValuePair.Key != yearIndex) continue;
                            var tnYear = _viewNavigation.Value.treeView_navigation.Nodes.Add(keyValuePair.Key);
                            tnYear.ImageKey = @"Year";
                            tnYear.SelectedImageKey = @"Year";
                            tnYear.Tag = keyValuePair.Value;

                            foreach (var monthIndex in monthsIndex)
                            {
                                foreach (var valuePair in keyValuePair.Value)
                                {
                                    if (valuePair.Key != monthIndex)
                                        continue;

                                    var tnMonth = tnYear.Nodes.Add(Common.GetMonthName(new DateTime(Convert.ToInt32(keyValuePair.Key), Convert.ToInt32(valuePair.Key), 1)));
                                    tnMonth.ImageKey = @"MonthBlue";
                                    tnMonth.SelectedImageKey = @"MonthBlue";
                                    tnMonth.Tag = keyValuePair.Value;

                                    foreach (var trackerProject in valuePair.Value)
                                    {
                                        itreeviewKeyIndex++;
                                        var tnProejct = tnMonth.Nodes.Add(itreeviewKeyIndex.ToString(), trackerProject.Name);
                                        treeviewKeyIndex.Add(trackerProject.Id, itreeviewKeyIndex.ToString());
                                        if (trackerProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                                        {
                                            tnProejct.ImageKey = @"ProjectInProgress";
                                            tnProejct.SelectedImageKey = @"ProjectInProgress";
                                        }
                                        else
                                        {
                                            tnProejct.ImageKey = @"ProjectCompleted";
                                            tnProejct.SelectedImageKey = @"ProjectCompleted";
                                        }
                                        tnProejct.Tag = trackerProject;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (groupBy.IndexOf(@"Project name", StringComparison.Ordinal) > -1)
                {
                    #region  |  group by Project name  |
                    foreach (var companyName in nameKeys)
                    {
                        foreach (var trackerProject in cpiProjects[companyName])
                        {
                            itreeviewKeyIndex++;
                            var tnProejct = _viewNavigation.Value.treeView_navigation.Nodes.Add(itreeviewKeyIndex.ToString(), trackerProject.Name);
                            treeviewKeyIndex.Add(trackerProject.Id, itreeviewKeyIndex.ToString());
                            if (trackerProject.ProjectStatus.IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                            {
                                tnProejct.ImageKey = @"ProjectInProgress";
                                tnProejct.SelectedImageKey = @"ProjectInProgress";
                            }
                            else
                            {
                                tnProejct.ImageKey = @"ProjectCompleted";
                                tnProejct.SelectedImageKey = @"ProjectCompleted";
                            }
                            tnProejct.Tag = trackerProject;
                        }
                    }
                    #endregion
                }

                checkEnableExpandAll_Navigation_treeview();

                if (selectedProject != null)
                {
                    if (treeviewKeyIndex.ContainsKey(selectedProject.Id))
                    {
                        var treeNodes = _viewNavigation.Value.treeView_navigation.Nodes.Find(treeviewKeyIndex[selectedProject.Id], true);
                        if (treeNodes.Length > 0)
                        {
                            _viewNavigation.Value.treeView_navigation.SelectedNode = treeNodes[0];
                            treeNodes[0].EnsureVisible();
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

        #region  |  viewerer control  |

        private void InitializeViewControl()
        {
            _viewContent.Value.olvColumn_activity_description.IsVisible = false;
            _viewContent.Value.objectListView1.RebuildColumns();


            _viewContent.Value.objectListView1.SelectionChanged += objectListView1_SelectionChanged;
            _viewContent.Value.objectListView1.DoubleClick += objectListView1_DoubleClick;
            _viewContent.Value.objectListView1.KeyUp += objectListView1_KeyUp;


            _viewContent.Value.newProjectActivityToolStripMenuItem.Click += newProjectActivityToolStripMenuItem_Click;
            _viewContent.Value.editProjectActivityToolStripMenuItem.Click += editProjectActivityToolStripMenuItem_Click;
            _viewContent.Value.removeProjectActivityToolStripMenuItem.Click += removeProjectActivityToolStripMenuItem_Click;


            _viewContent.Value.duplicateTheProjectActivityToolStripMenuItem.Click += duplicateTheProjectActivityToolStripMenuItem_Click;

            _viewContent.Value.exportActivitiesToExcelToolStripMenuItem.Click += exportActivitesToExcelToolStripMenuItem_Click;

            _viewContent.Value.mergeProjectActivitiesToolStripMenuItem.Click += mergeProjectActivitiesToolStripMenuItem_Click;


        }

        private void mergeProjectActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeProjectActivities();
        }

        private void exportActivitesToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportActivitesToExcel();
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
        }


        public bool ProjectActivityNewEnabled { get; set; }
        public bool ProjectActivityEditEnabled { get; set; }
        public bool ProjectActivityRemoveEnabled { get; set; }



        public bool ProjectActivityStartTrackerEnabled { get; set; }
        public bool ProjectActivityStopTrackerEnabled { get; set; }

        public bool ProjectActivityDuplicateEnabled { get; set; }
        public bool ProjectActivityMergeEnabled { get; set; }

        public bool ProjectActivityCreateReportEnabled { get; set; }
        public bool ProjectActivityExportExcelEnabled { get; set; }



        public void NewProjectActivity()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            TrackerProject trackerProject = null;
            var trackerProjects = new List<TrackerProject>();

            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
            {
                trackerProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                trackerProjects.Add(trackerProject);
            }
            else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null
                     || _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(ClientProfileInfo))
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Nodes.Count > 0)
                {
                    trackerProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Nodes[0].Tag;
                    trackerProjects.AddRange(from TreeNode treeNode in _viewNavigation.Value.treeView_navigation.SelectedNode.Nodes
                                             select (TrackerProject)treeNode.Tag);
                }
            }
            else if (_viewContent.Value.objectListView1.SelectedObjects.Count > 0)
            {
                var selectedObject = (TrackerProjectActivity)_viewContent.Value.objectListView1.SelectedObjects[0];

                trackerProject = Common.GetProjectFromId(selectedObject.TrackerProjectId);
                trackerProjects.Add(trackerProject);
            }


            if (trackerProject != null)
            {
                var trackProjectActivity = new Dialogs.TrackProjectActivity();



                var trackerProjectActivity = new TrackerProjectActivity
                {
                    Id = Guid.NewGuid().ToString(),
                    TrackerProjectId = trackerProject.Id,
                    TrackerProjectName = trackerProject.Name,
                    TrackerProjectStatus = trackerProject.ProjectStatus
                };



                if (trackerProject.ClientId.Trim() != string.Empty)
                {
                    var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);
                    if (clientProfileInfo != null)
                    {
                        trackerProjectActivity.ClientId = clientProfileInfo.Id;
                        trackerProjectActivity.ClientName = clientProfileInfo.ClientName;
                    }
                }


                trackProjectActivity.Project = trackerProject;
                trackProjectActivity.Activity = trackerProjectActivity;
                trackProjectActivity.Projects = trackerProjects;
                trackProjectActivity.IsEdit = false;
                trackProjectActivity.ShowDialog();
                if (!trackProjectActivity.Saved)
                    return;

                var project = trackerProjects.FirstOrDefault(tpTemp => tpTemp.Id == trackProjectActivity.Activity.TrackerProjectId);

                if (project != null)
                    project.ProjectActivities.Add(trackProjectActivity.Activity);


                SettingsSerializer.SaveSettings(Tracked.Preferences);

                FilterViewerControl();

                _viewContent.Value.objectListView1.SelectedObject = trackerProjectActivity;
            }
        }
        public void EditProjectActivity()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            if (_viewContent.Value.objectListView1.SelectedObjects.Count <= 0)
                return;

            var trackProjectActivity = new Dialogs.TrackProjectActivity();

            if (_viewContent.Value.objectListView1.SelectedObjects.Count <= 0)
                return;

            var selectedObject = (TrackerProjectActivity)_viewContent.Value.objectListView1.SelectedObjects[0];

            var trackerProject = Common.GetProjectFromId(selectedObject.TrackerProjectId);
            var trackerProjects = new List<TrackerProject> { trackerProject };


            selectedObject.TrackerProjectId = trackerProject.Id;
            selectedObject.TrackerProjectName = trackerProject.Name;
            selectedObject.TrackerProjectStatus = trackerProject.ProjectStatus;


            trackProjectActivity.Project = trackerProject;
            trackProjectActivity.Projects = trackerProjects;
            trackProjectActivity.Activity = selectedObject;


            trackProjectActivity.IsEdit = true;
            trackProjectActivity.ShowDialog();

            if (!trackProjectActivity.Saved)
                return;

            selectedObject.ActivityTypeClientId = trackProjectActivity.Activity.ActivityTypeClientId;
            selectedObject.ActivityTypeId = trackProjectActivity.Activity.ActivityTypeId;
            selectedObject.ActivityTypeName = trackProjectActivity.Activity.ActivityTypeName;

            selectedObject.Billable = trackProjectActivity.Activity.Billable;
            selectedObject.ClientId = trackProjectActivity.Activity.ClientId;
            selectedObject.ClientName = trackProjectActivity.Activity.ClientName;

            selectedObject.Currency = trackProjectActivity.Activity.Currency;
            selectedObject.DateEnd = trackProjectActivity.Activity.DateEnd;
            selectedObject.DateStart = trackProjectActivity.Activity.DateStart;

            selectedObject.Description = trackProjectActivity.Activity.Description;

            selectedObject.HourlyRate = trackProjectActivity.Activity.HourlyRate;
            selectedObject.HourlyRateAdjustment = trackProjectActivity.Activity.HourlyRateAdjustment;

            selectedObject.Invoiced = trackProjectActivity.Activity.Invoiced;
            selectedObject.InvoicedDate = trackProjectActivity.Activity.InvoicedDate;

            selectedObject.Name = trackProjectActivity.Activity.Name;
            selectedObject.Quantity = trackProjectActivity.Activity.Quantity;

            selectedObject.TrackerProjectId = trackProjectActivity.Activity.TrackerProjectId;
            selectedObject.TrackerProjectName = trackProjectActivity.Activity.TrackerProjectName;
            selectedObject.TrackerProjectStatus = trackProjectActivity.Activity.TrackerProjectStatus;

            selectedObject.Total = trackProjectActivity.Activity.Total;

            SettingsSerializer.SaveSettings(Tracked.Preferences);

            FilterViewerControl();

            _viewContent.Value.objectListView1.SelectedObject = selectedObject;
        }
        public void RemoveProjectActivity()
        {
            if (_viewContent.Value.objectListView1.Items.Count <= 0)
                return;

            var dr = MessageBox.Show(@"Are you sure that you want to remove the selected project activities?" + "\r\n\r\n"
                                     + @"Note: you will not be able to recover this data if you continue!" + "\r\n\r\n"
                                     + @"Click 'Yes' to continue and remove the project activities" + "\r\n"
                                     + @"Click 'No' to cancel"
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes)
                return;

            var trackerProjectActivities = (
                from OLVListItem listViewItem in _viewContent.Value.objectListView1.SelectedItems
                select (TrackerProjectActivity)listViewItem.RowObject).ToList();

            _viewContent.Value.objectListView1.RemoveObjects(trackerProjectActivities);

            foreach (var trackerProject in Tracked.Preferences.TrackerProjects)
            {
                foreach (var trackerProjectActivity in trackerProjectActivities)
                {
                    for (var i = 0; i < trackerProject.ProjectActivities.Count; i++)
                    {
                        if (trackerProject.ProjectActivities[i].Id != trackerProjectActivity.Id)
                            continue;

                        trackerProject.ProjectActivities.RemoveAt(i);
                        break;
                    }
                }
            }
            SettingsSerializer.SaveSettings(Tracked.Preferences);
            treeView_navigation_AfterSelect(null, null);
        }

        public void DuplicateProjectActivity()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            if (_viewContent.Value.objectListView1.SelectedItems.Count != 1)
                return;

            var selectedItem = (OLVListItem)_viewContent.Value.objectListView1.SelectedItems[0];
            var trackerProjectActivity = (TrackerProjectActivity)selectedItem.RowObject;


            var trackerProject = Common.GetProjectFromId(trackerProjectActivity.TrackerProjectId);

            var tpaClone = (TrackerProjectActivity)trackerProjectActivity.Clone();
            tpaClone.Id = Guid.NewGuid().ToString();

            trackerProject.ProjectActivities.Add(tpaClone);


            SettingsSerializer.SaveSettings(Tracked.Preferences);

            FilterViewerControl();

            _viewContent.Value.objectListView1.SelectedObject = tpaClone;
        }

        public void ExportActivitesToExcel()
        {
            var exportActivitesToExcel = new Dialogs.ExportActivitesToExcel();

            var idsSelected = new List<string>();
            var idsAll = new List<string>();
            if (_viewContent.Value.objectListView1.Items.Count > 0)
            {
                foreach (OLVListItem itmx in _viewContent.Value.objectListView1.Items)
                {
                    var itmxRowObject = (TrackerProjectActivity)itmx.RowObject;
                    if (itmx.Selected)
                        idsSelected.Add(itmxRowObject.Id);

                    idsAll.Add(itmxRowObject.Id);
                }
            }

            exportActivitesToExcel.IdsSelected = idsSelected;
            exportActivitesToExcel.IdsAll = idsAll;
            exportActivitesToExcel.ShowDialog();

        }


        public void StartTimeTracking()
        {

            try
            {
                Timer4ProjectArea.Stop();

                Tracked.HandlerPartent = 0;
                Tracked.TrackingState = Tracked.TimerState.Started;

                _viewContent.Value.Parent.Cursor = Cursors.Default;


                ProjectActivityStartTrackerEnabled = false;
                ProjectActivityStopTrackerEnabled = true;


                if (CheckEnabledObjectsEvent != null)
                {
                    CheckEnabledObjectsEvent(this, EventArgs.Empty);
                }

                if (Tracked.TrackingState == Tracked.TimerState.Started
                    && Tracked.TrackerDocumentId.Trim() == string.Empty)
                {
                    CheckStartTracker();
                }
            }
            finally
            {

                Tracked.TrackingIsDirtyC0 = true;
                Tracked.TrackingIsDirtyC1 = true;
                Tracked.TrackingIsDirtyC2 = true;

                TrackedActions.start_tracking(GetEditorController(), Timer4ProjectArea, true);
            }

        }
        public void StopTimeTracking()
        {
            try
            {
                Timer4ProjectArea.Stop();

                Tracked.HandlerPartent = 0;
                Tracked.TrackingState = Tracked.TimerState.Stopped;
                Tracked.TrackingTimer.Stop();

                ProjectActivityStartTrackerEnabled = true;
                ProjectActivityStopTrackerEnabled = false;


                if (CheckEnabledObjectsEvent != null)
                {
                    CheckEnabledObjectsEvent(this, EventArgs.Empty);
                }

                try
                {
                    if (Tracked.HandlerPartent != 0)
                        return;

                    if (Tracked.TrackerActivityName.Trim() == string.Empty)
                        return;

                    var trackerProject = Common.GetProjectFromId(Tracked.TrackerProjectId);
                    var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                    #region  |  add existing activity   |

                    var trackerProjectActivity = new TrackerProjectActivity
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = Tracked.TrackerActivityName
                    };


                    #region  |  get activity type  |

                    var settingsActivitiesType = Tracked.Preferences.ActivitiesTypes[0];
                    ClientActivityType clientActivityType = null;
                    foreach (var activitiesType in Tracked.Preferences.ActivitiesTypes)
                    {
                        if (string.Compare(activitiesType.Name, Tracked.TrackerActivityType, StringComparison.OrdinalIgnoreCase) != 0)
                            continue;

                        settingsActivitiesType = activitiesType;
                        if (clientProfileInfo != null)
                        {
                            foreach (var activityType in clientProfileInfo.ClientActivities)
                            {
                                if (settingsActivitiesType.Id != activityType.IdActivity)
                                    continue;

                                if (activityType.Activated)
                                {
                                    clientActivityType = activityType;
                                }
                                break;
                            }
                        }
                        break;
                    }
                    #endregion

                    trackerProjectActivity.ActivityTypeId = settingsActivitiesType.Id;
                    trackerProjectActivity.ActivityTypeName = settingsActivitiesType.Name;
                    if (clientActivityType != null)
                        trackerProjectActivity.ActivityTypeClientId = clientActivityType.Id;

                    trackerProjectActivity.Billable = settingsActivitiesType.Billable;
                    if (clientProfileInfo != null)
                    {
                        trackerProjectActivity.ClientId = clientProfileInfo.Id;
                        trackerProjectActivity.ClientName = clientProfileInfo.ClientName;
                    }
                    trackerProjectActivity.Currency = settingsActivitiesType.Currency;
                    trackerProjectActivity.DateStart = Tracked.TrackingStart;
                    Tracked.TrackingEnd = DateTime.Now;
                    trackerProjectActivity.DateEnd = Tracked.TrackingEnd;

                    trackerProjectActivity.Description = settingsActivitiesType.Description;
                    trackerProjectActivity.HourlyRate = settingsActivitiesType.HourlyRate;
                    trackerProjectActivity.HourlyRateAdjustment = 0;
                    trackerProjectActivity.Invoiced = false;
                    trackerProjectActivity.InvoicedDate = Common.DateNull;


                    trackerProjectActivity.Quantity = Convert.ToDecimal(trackerProjectActivity.DateEnd.Subtract(trackerProjectActivity.DateStart).TotalHours);
                    var quantityElapsed = Convert.ToDecimal(Math.Round(Tracked.TrackingTimer.Elapsed.TotalHours, 3));
                    if (quantityElapsed < trackerProjectActivity.Quantity)
                        trackerProjectActivity.Quantity = quantityElapsed;


                    trackerProjectActivity.Status = "New";
                    trackerProjectActivity.Total = Math.Round(trackerProjectActivity.Quantity * trackerProjectActivity.HourlyRate, 2);
                    trackerProjectActivity.TrackerProjectId = trackerProject.Id;
                    trackerProjectActivity.TrackerProjectName = trackerProject.Name;
                    trackerProjectActivity.TrackerProjectStatus = trackerProject.ProjectStatus;

                    if (Tracked.Preferences.TrackerConfirmActivities)
                    {
                        var trackProjectActivity = new Dialogs.TrackProjectActivity();
                        var trackerProjects = new List<TrackerProject> { trackerProject };


                        trackerProjectActivity.TrackerProjectId = trackerProject.Id;
                        trackerProjectActivity.TrackerProjectName = trackerProject.Name;
                        trackerProjectActivity.TrackerProjectStatus = trackerProject.ProjectStatus;


                        trackProjectActivity.Project = trackerProject;
                        trackProjectActivity.Projects = trackerProjects;
                        trackProjectActivity.Activity = trackerProjectActivity;


                        trackProjectActivity.IsEdit = true;
                        trackProjectActivity.ShowDialog();

                        if (!trackProjectActivity.Saved)
                            return;


                        trackerProjectActivity.ActivityTypeClientId = trackProjectActivity.Activity.ActivityTypeClientId;
                        trackerProjectActivity.ActivityTypeId = trackProjectActivity.Activity.ActivityTypeId;
                        trackerProjectActivity.ActivityTypeName = trackProjectActivity.Activity.ActivityTypeName;

                        trackerProjectActivity.Billable = trackProjectActivity.Activity.Billable;
                        trackerProjectActivity.ClientId = trackProjectActivity.Activity.ClientId;
                        trackerProjectActivity.ClientName = trackProjectActivity.Activity.ClientName;

                        trackerProjectActivity.Currency = trackProjectActivity.Activity.Currency;
                        trackerProjectActivity.DateEnd = trackProjectActivity.Activity.DateEnd;
                        trackerProjectActivity.DateStart = trackProjectActivity.Activity.DateStart;

                        trackerProjectActivity.Description = trackProjectActivity.Activity.Description;

                        trackerProjectActivity.HourlyRate = trackProjectActivity.Activity.HourlyRate;
                        trackerProjectActivity.HourlyRateAdjustment = trackProjectActivity.Activity.HourlyRateAdjustment;

                        trackerProjectActivity.Invoiced = trackProjectActivity.Activity.Invoiced;
                        trackerProjectActivity.InvoicedDate = trackProjectActivity.Activity.InvoicedDate;

                        trackerProjectActivity.Name = trackProjectActivity.Activity.Name;
                        trackerProjectActivity.Quantity = trackProjectActivity.Activity.Quantity;

                        trackerProjectActivity.TrackerProjectId = trackProjectActivity.Activity.TrackerProjectId;
                        trackerProjectActivity.TrackerProjectName = trackProjectActivity.Activity.TrackerProjectName;
                        trackerProjectActivity.TrackerProjectStatus = trackProjectActivity.Activity.TrackerProjectStatus;

                        trackerProjectActivity.Total = trackProjectActivity.Activity.Total;

                        trackerProject.ProjectActivities.Add(trackerProjectActivity);

                        SettingsSerializer.SaveSettings(Tracked.Preferences);

                        Tracked.TarckerCheckNewActivityAdded = true;
                        Tracked.TarckerCheckNewActivityId = trackerProjectActivity.Id;
                    }
                    else
                    {
                        trackerProject.ProjectActivities.Add(trackerProjectActivity);

                        SettingsSerializer.SaveSettings(Tracked.Preferences);

                        Tracked.TarckerCheckNewActivityAdded = true;
                        Tracked.TarckerCheckNewActivityId = trackerProjectActivity.Id;
                    }

                    #endregion
                }
                finally
                {
                    Tracked.Reset();
                }


            }
            finally
            {

                ProjectActivityStartTrackerEnabled = true;
                ProjectActivityStopTrackerEnabled = false;

                TrackedActions.stop_tracking(GetEditorController(), Timer4ProjectArea);
            }
        }



        public void MergeProjectActivities()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            var dateStart = DateTime.Now.AddYears(200);
            var dateEnd = Common.DateNull;
            decimal totalHours = 0;

            var typeId = string.Empty;
            var typeIdClient = string.Empty;

            var projectId = string.Empty;
            var activityType = string.Empty;

            var projectIdError = false;
            var activityTypeWarning = false;

            var projectActivities = new List<TrackerProjectActivity>();
            foreach (OLVListItem itmx in _viewContent.Value.objectListView1.SelectedItems)
            {
                var trackerProjectActivity = (TrackerProjectActivity)itmx.RowObject;
                projectActivities.Add(trackerProjectActivity);


                if (trackerProjectActivity.ActivityTypeId.Trim() != string.Empty)
                {
                    typeId = trackerProjectActivity.ActivityTypeId;
                    typeIdClient = trackerProjectActivity.ActivityTypeClientId;
                }

                totalHours += trackerProjectActivity.Quantity;

                if (trackerProjectActivity.DateStart < dateStart)
                    dateStart = trackerProjectActivity.DateStart;

                if (trackerProjectActivity.DateEnd > dateEnd)
                    dateEnd = trackerProjectActivity.DateEnd;

                #region  |  check for errors/warnings  |
                if (projectId.Trim() != string.Empty)
                {
                    if (string.Compare(projectId, trackerProjectActivity.TrackerProjectId, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        projectIdError = true;
                        break;
                    }
                }
                else
                {
                    projectId = trackerProjectActivity.TrackerProjectId.Trim();
                }


                if (activityType.Trim() != string.Empty)
                {
                    if (string.Compare(activityType, trackerProjectActivity.ActivityTypeName, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        activityTypeWarning = true;
                    }
                }
                else
                {
                    activityType = trackerProjectActivity.ActivityTypeName.Trim();
                }
                #endregion
            }

            if (projectIdError)
            {
                MessageBox.Show(@"Unable to merge activities that belong to different projects!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var continueMerge = true;
                if (activityTypeWarning)
                {
                    var dr = MessageBox.Show(@"Located multiple activity types from the selected records" + "\r\n\r\n"
                                             + @"Are you sure that you want to continue merging these records?" + "\r\n\r\n"
                                             + @"Click 'Yes' to continue" + "\r\n"
                                             + @"Click 'No' to cancel", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.No)
                        continueMerge = false;
                }
                if (!continueMerge)
                    return;

                var trackerProject = Common.GetProjectFromId(projectId);
                var trackerProjects = new List<TrackerProject> { trackerProject };


                if (trackerProject == null)
                    return;

                var trackProjectActivity = new Dialogs.TrackProjectActivity();



                var trackerProjectActivity = new TrackerProjectActivity
                {
                    Id = Guid.NewGuid().ToString(),
                    DateStart = dateStart,
                    DateEnd = dateEnd,
                    Quantity = totalHours,
                    TrackerProjectId = trackerProject.Id,
                    TrackerProjectName = trackerProject.Name,
                    TrackerProjectStatus = trackerProject.ProjectStatus,
                    Name = projectActivities[0].Name,
                    Billable = projectActivities[0].Billable,
                    Currency = projectActivities[0].Currency,
                    ActivityTypeId = typeId,
                    ActivityTypeClientId = typeIdClient
                };


                if (trackerProject.ClientId.Trim() != string.Empty)
                {
                    var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);
                    if (clientProfileInfo != null)
                    {
                        trackerProjectActivity.ClientId = clientProfileInfo.Id;
                        trackerProjectActivity.ClientName = clientProfileInfo.ClientName;
                    }
                }



                trackProjectActivity.Project = trackerProject;
                trackProjectActivity.Activity = trackerProjectActivity;
                trackProjectActivity.Projects = trackerProjects;
                trackProjectActivity.IsEdit = true;
                trackProjectActivity.IsMerge = true;

                trackProjectActivity.ShowDialog();
                if (!trackProjectActivity.Saved)
                    return;

                var trackerProjectActivities = (
                    from OLVListItem listViewItem in _viewContent.Value.objectListView1.SelectedItems
                    select (TrackerProjectActivity)listViewItem.RowObject).ToList();

                _viewContent.Value.objectListView1.RemoveObjects(trackerProjectActivities);

                foreach (var project in Tracked.Preferences.TrackerProjects)
                {
                    foreach (var projectActivity in trackerProjectActivities)
                    {
                        for (var i = 0; i < project.ProjectActivities.Count; i++)
                        {
                            if (project.ProjectActivities[i].Id != projectActivity.Id)
                                continue;

                            project.ProjectActivities.RemoveAt(i);
                            break;
                        }
                    }
                }




                var tpCurrent = trackerProjects.FirstOrDefault(tpTemp =>
                        tpTemp.Id == trackProjectActivity.Activity.TrackerProjectId);

                if (tpCurrent != null)
                    tpCurrent.ProjectActivities.Add(trackProjectActivity.Activity);


                SettingsSerializer.SaveSettings(Tracked.Preferences);
                treeView_navigation_AfterSelect(null, null);


                _viewContent.Value.objectListView1.SelectedObject = trackerProjectActivity;
            }
        }



        private void FillActivityDataView(List<TrackerProjectActivity> projectActivities)
        {

            _viewContent.Value.objectListView1.BeginUpdate();
            try
            {
                _viewContent.Value.objectListView1.SetObjects(projectActivities);

                if (_viewContent.Value.objectListView1.Items.Count > 0)
                {
                    _viewContent.Value.objectListView1.SelectedIndex = 0;
                }

                var projects = new List<string>();
                var activities = new List<string>();
                foreach (OLVListItem listViewItem in _viewContent.Value.objectListView1.Items)
                {
                    var tpa = (TrackerProjectActivity)listViewItem.RowObject;

                    if (!projects.Contains(tpa.TrackerProjectId))
                        projects.Add(tpa.TrackerProjectId);

                    if (!activities.Contains(tpa.Id))
                        activities.Add(tpa.Id);
                }

                _viewContent.Value.label_TOTAL_PROJECTS.Text = projects.Count.ToString();
                _viewContent.Value.label_TOTAL_PROJECT_ACTIVITIES.Text = activities.Count.ToString();

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

        private void FilterViewerControl()
        {
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
            {
                #region  |  by project  |

                var trackerProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                _viewContent.Value.label_viewer_header.Text = @"Project: " + trackerProject.Name;


                FillActivityDataView(trackerProject.ProjectActivities);


                #endregion
            }
            else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null
                     || _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(ClientProfileInfo))
            {
                #region  |  by client  |

                var clientProfileInfoId = string.Empty;
                if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag == null)
                {
                    _viewContent.Value.label_viewer_header.Text = @"[no client]";
                }
                else
                {

                    var clientProfileInfo = (ClientProfileInfo)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                    _viewContent.Value.label_viewer_header.Text = @"Client: " + clientProfileInfo.ClientName;
                    clientProfileInfoId = clientProfileInfo.Id;
                }
                var filteredProjects = GetFilteredProjects();


                var activities = filteredProjects.Where(tp => tp.ClientId == clientProfileInfoId).SelectMany(tp => tp.ProjectActivities).ToList();


                FillActivityDataView(activities);



                #endregion

            }
            else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                     && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(Dictionary<string, List<TrackerProject>>))
            {
                #region  |  by date  |
                switch (_viewNavigation.Value.treeView_navigation.SelectedNode.Level)
                {
                    case 0:

                        _viewContent.Value.label_viewer_header.Text = @"Year: " + _viewNavigation.Value.treeView_navigation.SelectedNode.Text + "";
                        break;
                    case 1:
                        _viewContent.Value.label_viewer_header.Text = @"Year: " + _viewNavigation.Value.treeView_navigation.SelectedNode.Parent.Text + ", ";
                        _viewContent.Value.label_viewer_header.Text += @"Month: " + _viewNavigation.Value.treeView_navigation.SelectedNode.Text + "";
                        break;
                }

                var filteredProjects = GetFilteredProjects();
                var activities = new List<TrackerProjectActivity>();
                foreach (var trackerProject in filteredProjects)
                {
                    var filterDate = true;

                    switch (_viewNavigation.Value.treeView_navigation.SelectedNode.Level)
                    {
                        case 0:
                            if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project created", StringComparison.Ordinal) > -1)
                            {
                                if (trackerProject.DateCreated.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim())
                                {
                                    filterDate = false;
                                }
                            }
                            else if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project due", StringComparison.Ordinal) > -1)
                            {
                                if (trackerProject.DateDue.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim())
                                {
                                    filterDate = false;
                                }
                            }
                            break;
                        case 1:
                            if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project created", StringComparison.Ordinal) > -1)
                            {
                                if (trackerProject.DateCreated.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Parent.Text.Trim())
                                {
                                    filterDate = false;
                                }
                                else
                                {
                                    var monthProject = Common.GetMonthName(trackerProject.DateCreated);
                                    if (string.Compare(monthProject, _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        filterDate = false;
                                    }
                                }
                            }
                            else if (_viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().IndexOf(@"Project due", StringComparison.Ordinal) > -1)
                            {
                                if (trackerProject.DateDue.Year.ToString() != _viewNavigation.Value.treeView_navigation.SelectedNode.Parent.Text.Trim())
                                {
                                    filterDate = false;

                                }
                                else
                                {
                                    var monthProject = Common.GetMonthName(trackerProject.DateDue);
                                    if (string.Compare(monthProject, _viewNavigation.Value.treeView_navigation.SelectedNode.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        filterDate = false;
                                    }
                                }
                            }
                            break;
                    }
                    if (!filterDate) continue;
                    activities.AddRange(trackerProject.ProjectActivities);
                }

                FillActivityDataView(activities);


                #endregion
            }
        }

        #endregion


        private void UpdateActivityPropertiesViewer()
        {
            try
            {
                if (IsLoading)
                    return;

                var viewPartsDetails = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewPropertiesController>();

                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {

                    if (_viewContent.Value.objectListView1.SelectedObjects.Count > 0)
                    {
                        #region  |  ActivityPropertiesView  |

                        var activityPropertiesView = new ActivityPropertiesView();

                        var trackerProjectActivity = (TrackerProjectActivity)_viewContent.Value.objectListView1.SelectedObjects[0];


                        var trackerProject = Common.GetProjectFromId(trackerProjectActivity.TrackerProjectId);


                        if (trackerProject.ClientId.Trim() != string.Empty)
                        {
                            var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);

                            activityPropertiesView.clientId = clientProfileInfo.Id;
                            activityPropertiesView.clientName = clientProfileInfo.ClientName;

                            activityPropertiesView.clientAddress = clientProfileInfo.AddressStreet + "\r\n"
                                 + clientProfileInfo.AddressZip + " "
                                 + clientProfileInfo.AddressCity
                                 + (clientProfileInfo.AddressState != string.Empty ? " (" + clientProfileInfo.AddressState + ")" : string.Empty) + "\r\n"
                                 + clientProfileInfo.AddressCountry;

                            activityPropertiesView.clientEMail = clientProfileInfo.Email;
                            activityPropertiesView.clientFAX = clientProfileInfo.FaxNumber;
                            activityPropertiesView.clientPhone = clientProfileInfo.PhoneNumber;
                            activityPropertiesView.clientFAX = clientProfileInfo.FaxNumber;

                            activityPropertiesView.clientVAT = clientProfileInfo.VatCode;
                            activityPropertiesView.clientTAX = clientProfileInfo.TaxCode;

                            activityPropertiesView.clientWebPage = clientProfileInfo.WebPage;
                        }


                        activityPropertiesView.activityId = trackerProjectActivity.Id;
                        activityPropertiesView.activityName = trackerProjectActivity.Name;

                        activityPropertiesView.activityDescription = trackerProjectActivity.Description;

                        activityPropertiesView.activityStatus = trackerProjectActivity.Status;

                        activityPropertiesView.activityBillable = trackerProjectActivity.Billable.ToString();

                        activityPropertiesView.activityDateStart = trackerProjectActivity.DateStart.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.activityDateEnd = trackerProjectActivity.DateEnd.ToString(CultureInfo.InvariantCulture);

                        activityPropertiesView.activityType = trackerProjectActivity.ActivityTypeName;

                        activityPropertiesView.activityInvoiced = trackerProjectActivity.Invoiced.ToString();
                        activityPropertiesView.activityInvoicedDate = trackerProjectActivity.InvoicedDate != Common.DateNull ? trackerProjectActivity.InvoicedDate.ToString(CultureInfo.InvariantCulture) : string.Empty;

                        activityPropertiesView.activityHours = trackerProjectActivity.Quantity.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.activityRate = trackerProjectActivity.HourlyRate.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.activityRateAdjustment = trackerProjectActivity.HourlyRateAdjustment.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.activityTotal = trackerProjectActivity.Total.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.activityCurrency = trackerProjectActivity.Currency;


                        activityPropertiesView.projectId = trackerProject.Id;
                        activityPropertiesView.projectName = trackerProject.Name;
                        activityPropertiesView.projectDescription = trackerProject.Description;

                        activityPropertiesView.projectStatus = trackerProject.ProjectStatus;



                        activityPropertiesView.projectDateCreated = trackerProject.DateCreated.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.projectDateDue = trackerProject.DateDue.ToString(CultureInfo.InvariantCulture);
                        activityPropertiesView.projectDateComplated = trackerProject.DateCompleted != Common.DateNull ? trackerProject.DateCompleted.ToString(CultureInfo.InvariantCulture) : string.Empty;

                        activityPropertiesView.projectActivitesCount = trackerProject.ProjectActivities.Count.ToString();



                        viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = activityPropertiesView;

                        #endregion
                    }
                    else
                    {
                        if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                            && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(TrackerProject))
                        {

                            var trackerProject = (TrackerProject)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;
                            if (trackerProject.ClientId.Trim() != string.Empty)
                            {
                                #region  | ClientProjectPropertiesView  |

                                var clientProfileInfo = Common.GetClientFromId(trackerProject.ClientId);


                                var clientProjectPropertiesView = new ClientProjectPropertiesView
                                {
                                    ClientId = clientProfileInfo.Id,
                                    ClientName = clientProfileInfo.ClientName,
                                    ClientAddress = clientProfileInfo.AddressStreet + "\r\n"
                                                    + clientProfileInfo.AddressZip + " "
                                                    + clientProfileInfo.AddressCity
                                                    +
                                                    (clientProfileInfo.AddressState != string.Empty
                                                        ? " (" + clientProfileInfo.AddressState + ")"
                                                        : string.Empty) + "\r\n"
                                                    + clientProfileInfo.AddressCountry,
                                    ClientEMail = clientProfileInfo.Email,
                                    ClientFax = clientProfileInfo.FaxNumber,
                                    ClientPhone = clientProfileInfo.PhoneNumber
                                };

                                clientProjectPropertiesView.ClientFax = clientProfileInfo.FaxNumber;

                                clientProjectPropertiesView.ClientVat = clientProfileInfo.VatCode;
                                clientProjectPropertiesView.ClientTax = clientProfileInfo.TaxCode;

                                clientProjectPropertiesView.ClientWebPage = clientProfileInfo.WebPage;

                                clientProjectPropertiesView.ProjectId = trackerProject.Id;
                                clientProjectPropertiesView.ProjectName = trackerProject.Name;
                                clientProjectPropertiesView.ProjectDescription = trackerProject.Description;

                                clientProjectPropertiesView.ProjectStatus = trackerProject.ProjectStatus;



                                clientProjectPropertiesView.ProjectDateCreated = trackerProject.DateCreated.ToString(CultureInfo.InvariantCulture);
                                clientProjectPropertiesView.ProjectDateDue = trackerProject.DateDue.ToString(CultureInfo.InvariantCulture);
                                clientProjectPropertiesView.ProjectDateComplated = trackerProject.DateCompleted != Common.DateNull ? trackerProject.DateCompleted.ToString(CultureInfo.InvariantCulture) : string.Empty;

                                clientProjectPropertiesView.ProjectActivitesCount = trackerProject.ProjectActivities.Count.ToString();

                                viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = clientProjectPropertiesView;

                                #endregion
                            }
                            else
                            {
                                #region  |  ProjectPropertiesView  |

                                var projectPropertiesView = new ProjectPropertiesView
                                {
                                    ProjectId = trackerProject.Id,
                                    ProjectName = trackerProject.Name,
                                    ProjectDescription = trackerProject.Description,
                                    ProjectStatus = trackerProject.ProjectStatus,
                                    ProjectDateCreated =
                                        trackerProject.DateCreated.ToString(CultureInfo.InvariantCulture),
                                    ProjectDateDue = trackerProject.DateDue.ToString(CultureInfo.InvariantCulture),
                                    ProjectDateComplated =
                                        trackerProject.DateCompleted != Common.DateNull
                                            ? trackerProject.DateCompleted.ToString(CultureInfo.InvariantCulture)
                                            : string.Empty,
                                    ProjectActivitesCount = trackerProject.ProjectActivities.Count.ToString()
                                };







                                viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = projectPropertiesView;

                                #endregion
                            }

                        }
                        else if (_viewNavigation.Value.treeView_navigation.SelectedNode.Tag != null
                            && _viewNavigation.Value.treeView_navigation.SelectedNode.Tag.GetType() == typeof(ClientProfileInfo))
                        {
                            #region  |  ClientPropertiesView  |

                            var clientProfileInfo = (ClientProfileInfo)_viewNavigation.Value.treeView_navigation.SelectedNode.Tag;


                            var clientPropertiesView = new ClientPropertiesView
                            {
                                ClientId = clientProfileInfo.Id,
                                ClientName = clientProfileInfo.ClientName,
                                ClientAddress = clientProfileInfo.AddressStreet + "\r\n"
                                                + clientProfileInfo.AddressZip + " "
                                                + clientProfileInfo.AddressCity
                                                +
                                                (clientProfileInfo.AddressState != string.Empty
                                                    ? " (" + clientProfileInfo.AddressState + ")"
                                                    : string.Empty) + "\r\n"
                                                + clientProfileInfo.AddressCountry,
                                ClientEMail = clientProfileInfo.Email,
                                ClientFax = clientProfileInfo.FaxNumber,
                                ClientPhone = clientProfileInfo.PhoneNumber
                            };




                            clientPropertiesView.ClientFax = clientProfileInfo.FaxNumber;

                            clientPropertiesView.ClientVat = clientProfileInfo.VatCode;
                            clientPropertiesView.ClientTax = clientProfileInfo.TaxCode;

                            clientPropertiesView.ClientWebPage = clientProfileInfo.WebPage;

                            viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = clientPropertiesView;
                            #endregion

                        }
                        else
                        {
                            viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = null;
                        }
                    }
                }
                else
                {
                    viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = null;
                }
            }
            catch
            {
                // ignored
            }
        }


 
        public void ViewAboutInfo()
        {
            var f = new Dialogs.About();
            f.ShowDialog();
        }


        private static void LoadCurrencies()
        {

            Tracked.Currencies = new List<Currency>();
            var index = new List<string>();
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var regionInfo = new RegionInfo(cultureInfo.Name);
                if (index.Contains(regionInfo.ISOCurrencySymbol))
                    continue;

                index.Add(regionInfo.ISOCurrencySymbol);
                Tracked.Currencies.Add(new Currency(regionInfo.ISOCurrencySymbol, regionInfo.CurrencySymbol, regionInfo.CurrencyEnglishName));
            }
        }




        public void LoadSettings(int index = 0)
        {

            var settings = new Dialogs.Settings { settings = (Settings)Tracked.Preferences.Clone() };

            settings.treeView_main.SelectedNode = settings.treeView_main.Nodes[index];
            settings.ShowDialog();

            if (!settings.Saved)
                return;

            Tracked.Preferences = settings.settings;


            var groupBy = _viewNavigation.Value.comboBox_groupBy.SelectedItem.ToString().Trim();
            if (groupBy.IndexOf(@"Client name", StringComparison.Ordinal) > -1)
            {
                foreach (TreeNode treeNode in _viewNavigation.Value.treeView_navigation.Nodes)
                {
                    if (treeNode.Tag == null || treeNode.Tag.GetType() != typeof(ClientProfileInfo))
                        continue;

                    var clientProfileInfo = (ClientProfileInfo)treeNode.Tag;
                    foreach (var profileInfo in Tracked.Preferences.Clients)
                    {
                        if (clientProfileInfo.Id != profileInfo.Id)
                            continue;

                        treeNode.Text = profileInfo.ClientName;
                        treeNode.Tag = profileInfo;
                        break;
                    }
                }
            }
            SettingsSerializer.SaveSettings(Tracked.Preferences);

            treeView_navigation_AfterSelect(null, null);
        }
    }
}