using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PostEdit.Compare;
using PostEdit.Compare.Model;
using Sdl.Community.PostEdit.Versions.Automation;
using Sdl.Community.PostEdit.Versions.Dialogs;
using Sdl.Community.PostEdit.Versions.Structures;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using About = Sdl.Community.PostEdit.Versions.Dialogs.About;
using SettingsSerializer = Sdl.Community.PostEdit.Versions.Structures.SettingsSerializer;

namespace Sdl.Community.PostEdit.Versions
{


    [View(
        Id = "PostEdit.Versions",
        Name = "Post-Edit Versions",
        Description = "Post-Edit Versions",
        Icon = "PostEditVersions_Icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation),
        AllowViewParts = true
        )]
    public class PostEditCompareViewController : AbstractViewController
    {
        private readonly Lazy<PostEditCompareViewControl> _viewContent = new Lazy<PostEditCompareViewControl>();
        private readonly Lazy<PostEditCompareNavigationControl> _viewNavigation = new Lazy<PostEditCompareNavigationControl>();

        public ProjectsController ProjectsController { get; set; }
        public FileBasedProject CurrentSelectedProject { get; set; }
        public ProjectInfo CurrentProjectInfo { get; set; }
        public Settings Settings { get; set; }


        private bool IsUpdatingNavigationView { get; set; }

        public event EventHandler CheckEnabledObjectsEvent;

        protected override Control GetContentControl()
        {
            return _viewContent.Value;
        }
        protected override Control GetExplorerBarControl()
        {
            return _viewNavigation.Value;
        }

        protected override void Initialize(IViewContext context)
        {

            IsUpdatingNavigationView = false;


            IsEnabledCompare = false;
            IsEnabledCreateNewProjectVersion = false;
            IsEnabledEditProjectVersion = false;
            IsEnabledDeleteProjectVersion = false;
            IsEnabledDeleteProject = false;
            IsEnabledRestoreProjectVersion = false;


            Settings = SettingsSerializer.ReadSettings();

            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();


            ProjectsController.SelectedProjectsChanged += projectsController_SelectedProjectsChanged;


            _viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndexChanged += listView_postEditCompareProjectVersions_SelectedIndexChanged;
            _viewContent.Value.listView_postEditCompareProjectVersions.KeyUp += listView_postEditCompareProjectVersions_KeyUp;
            _viewContent.Value.compareWithPostEditCompareToolStripMenuItem.Click += compareWithPostEditCompareToolStripMenuItem_Click;
            _viewContent.Value.saveNewProjectVersionToolStripMenuItem.Click += saveNewProjectVersionToolStripMenuItem_Click;
            _viewContent.Value.editSelectedProjectVersionToolStripMenuItem.Click += editSelectedProjectVersionToolStripMenuItem_Click;
            _viewContent.Value.removeSelectedProjectVersionsToolStripMenuItem.Click += removeSelectedProjectVersionsToolStripMenuItem_Click;
            _viewContent.Value.viewProjectVersionInWindowsExplorerToolStripMenuItem.Click += viewProjectVersionInWindowsExplorerToolStripMenuItem_Click;
            _viewContent.Value.listView_postEditCompareProjectVersions.DoubleClick += listView_postEditCompareProjectVersions_DoubleClick;
            _viewContent.Value.restoreProjectVersionToolStripMenuItem.Click += restoreProjectVersionToolStripMenuItem_Click;

            _viewNavigation.Value.treeView_navigation.AfterSelect += treeView_navigation_AfterSelect;
            _viewNavigation.Value.treeView_navigation.KeyUp += treeView_navigation_KeyUp;
            _viewNavigation.Value.createNewProjectVersionToolStripMenuItem.Click += createNewProjectVersionToolStripMenuItem_Click;
            _viewNavigation.Value.removeAllVersionsForTheSelectedProjectToolStripMenuItem.Click += removeAllVersionsForTheSelectedProjectToolStripMenuItem_Click;
            _viewNavigation.Value.viewProjectInWindowsExplorerToolStripMenuItem.Click += viewProjectInWindowsExplorerToolStripMenuItem_Click;
            _viewNavigation.Value.linkLabel_removeProjectVersions.Click += linkLabel_removeProjectVersions_Click;
            _viewNavigation.Value.treeView_navigation.MouseUp += treeView_navigation_MouseUp;
            _viewNavigation.Value.contextMenuStrip_navControl.Opening += contextMenuStrip_navControl_Opening;
            _viewNavigation.Value.treeView_navigation.DoubleClick += treeView_navigation_DoubleClick;
            _viewNavigation.Value.textBox_view.TextChanged += textBox_view_TextChanged;

            _viewContent.Value.listView_postEditCompareProjectVersions.Items.Clear();
            _viewContent.Value.label_PROJECT_VERSIONS_MESSAGE.Text = @"To compare two versions of the project, select them from the list and then click on the button 'Compare with Post-Edit Compare'";
            _viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = @"0";
            _viewContent.Value.label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = @"0";
            _viewContent.Value.label_SELECTED_PROJECT_NAME.Text = string.Empty;


            _viewNavigation.Value.label_NAVIGATION_MESSAGE.Text = @"Projects 0";



            InitializeNavigationControl(Settings.projects);


            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

        }

        private void restoreProjectVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreProjectVersion();
        }

        private void textBox_view_TextChanged(object sender, EventArgs e)
        {
            NavigationViewTextChanged();
        }

        private void treeView_navigation_DoubleClick(object sender, EventArgs e)
        {
            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }
            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
                return;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null)
                return;

            var tn = _viewNavigation.Value.treeView_navigation.SelectedNode;
            var project = (Project)tn.Tag;

            if (tn.ImageIndex != 0)
                return;

            var sdlProjects = ProjectsController.GetProjects().ToList();
            var sdlProject = sdlProjects.FirstOrDefault(proj => string.Compare(project.id, proj.GetProjectInfo().Id.ToString(), StringComparison.OrdinalIgnoreCase) == 0);

            if (sdlProject == null)
                return;

            ProjectsController.Activate();
            ProjectsController.Open(sdlProject);
        }

        private void contextMenuStrip_navControl_Opening(object sender, CancelEventArgs e)
        {
            CheckEnabledObjects();
        }

        private void treeView_navigation_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                _viewNavigation.Value.treeView_navigation.SelectedNode = _viewNavigation.Value.treeView_navigation.GetNodeAt(e.X, e.Y);
            }
            CheckEnabledObjects();
        }

        private void createNewProjectVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProjectVersion(false);
        }

        private void compareWithPostEditCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareProjectVersions();
        }

        private void projectsController_SelectedProjectsChanged(object sender, EventArgs e)
        {
            SelectedProjectChanged();
        }

        private void listView_postEditCompareProjectVersions_DoubleClick(object sender, EventArgs e)
        {
            EditProjectVersion();
        }

        private void linkLabel_removeProjectVersions_Click(object sender, EventArgs e)
        {
            RemoveAllProjectVersions();
        }

        private void viewProjectInWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewProjectInWindowsExplorer();
        }

        private void viewProjectVersionInWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewProjectVersionInWindowsExplorer();
        }

        private void treeView_navigation_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
                RemoveAllProjectVersions();

        }

        private void listView_postEditCompareProjectVersions_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveProjectVersions();
        }

        private void removeAllVersionsForTheSelectedProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllProjectVersions();
        }

        private void removeSelectedProjectVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveProjectVersions();
        }

        private void editSelectedProjectVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditProjectVersion();
        }

        private void saveNewProjectVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProjectVersion(false);
        }

        private void listView_postEditCompareProjectVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewContent.Value.label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count.ToString();
            _viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Count.ToString();

            UpdateviewPartsDetails();

        }

        private void treeView_navigation_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                {
                    var selectedNode = _viewNavigation.Value.treeView_navigation.SelectedNode;
                    var project = (Project)selectedNode.Tag;

                    _viewContent.Value.label_SELECTED_PROJECT_NAME.Text = string.Format(PluginResources.Project_Versions_0, project.name);


                    InitializeViewControl(project);

                    if (_viewContent.Value.listView_postEditCompareProjectVersions.Items.Count > 0)
                        _viewContent.Value.listView_postEditCompareProjectVersions.Items[0].Selected = true;
                }
            }

            CheckEnabledObjects();
        }


        public void NavigationViewTextChanged()
        {
            if (CurrentSelectedProject == null || CurrentProjectInfo == null) return;
            var projects = new List<Project>();
            foreach (var project in Settings.projects)
            {
                if (_viewNavigation.Value.textBox_view.Text.Trim() != string.Empty)
                {
                    if (project.name.ToLower().IndexOf(_viewNavigation.Value.textBox_view.Text.ToLower().Trim(), StringComparison.Ordinal) > -1)
                    {
                        projects.Add(project);
                    }
                }
                else
                {
                    projects.Add(project);
                }
            }



            try
            {
                IsUpdatingNavigationView = true;

                _viewNavigation.Value.treeView_navigation.BeginUpdate();

                _viewNavigation.Value.treeView_navigation.Nodes.Clear();

                TreeNode tnSelected = null;
                foreach (var proj in projects)
                {
                    var project = Settings.projects.FirstOrDefault(project1 => project1.id == proj.id);


                    if (project == null)
                        continue;

                    var tn = _viewNavigation.Value.treeView_navigation.Nodes.Add(project.id, project.name);
                    tn.Tag = project;
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                }

                if (_viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
                    tnSelected = _viewNavigation.Value.treeView_navigation.Nodes[0];

                if (tnSelected != null)
                    _viewNavigation.Value.treeView_navigation.SelectedNode = tnSelected;
                else
                {
                    InitializeViewControl(new Project());
                    UpdateviewPartsDetails();
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _viewNavigation.Value.treeView_navigation.EndUpdate();

                IsUpdatingNavigationView = false;


                _viewNavigation.Value.label_NAVIGATION_MESSAGE.Text = string.Format(PluginResources.Projects_0, 0);


            }
        }

        public void ViewOnlineHelp()
        {
            Process.Start("http://posteditcompare.wiki-site.com/index.php/Main_Page");
        }
        public void ViewAboutInfo()
        {
            var about = new About();
            about.ShowDialog();
        }

        public void LoadConfigurationSettings()
        {

            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

            var defaultSettings = new DefaultSettings
            {
                Saved = false,
                CreateSubFolderProject = Settings.create_subfolder_projects,
                CreateShallowCopy = Settings.create_shallow_copy,
                VersionsFolderFullPath = Settings.versions_folder_path
            };

            defaultSettings.ShowDialog();

            if (!defaultSettings.Saved)
                return;

            Settings.create_subfolder_projects = defaultSettings.CreateSubFolderProject;
            Settings.create_shallow_copy = defaultSettings.CreateShallowCopy;
            Settings.versions_folder_path = defaultSettings.VersionsFolderFullPath;

            SettingsSerializer.SaveSettings(Settings);
        }


        public void LoadPostEditCompare()
        {

            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

            IModel mModel = new Model();
            var postEditCompare = new FormMain(mModel);
            postEditCompare.ShowDialog();



        }
        public void CompareProjectVersions()
        {

            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count != 2) return;
            try
            {
                var selectedItem01 = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0];
                var selectedItem02 = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[1];

                var projectVersion01 = (ProjectVersion)selectedItem01.Tag;
                var projectVersion02 = (ProjectVersion)selectedItem02.Tag;

                var dateTimeCreated01 = Helper.GetDateTimeFromString(projectVersion01.createdAt);
                var dateTimeCreated02 = Helper.GetDateTimeFromString(projectVersion02.createdAt);



                var settings = new AutomationComunicationSettings();


                if (dateTimeCreated01 < dateTimeCreated02)
                {
                    settings.folderPathLeft = projectVersion01.location;
                    settings.folderPathRight = projectVersion02.location;
                }
                else
                {
                    settings.folderPathLeft = projectVersion02.location;
                    settings.folderPathRight = projectVersion01.location;
                }


                Automation.SettingsSerializer.SaveSettings(settings);
                IModel mModel = new Model();
                var postEditCompare = new FormMain(mModel);
                postEditCompare.ShowDialog();
   }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void CreateNewProjectVersion(bool fromSdlProjectSelection)
        {

            if (CurrentSelectedProject == null || CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

            try
            {
                var newProjectVersion = new NewProjectVersion { ProjectsController = ProjectsController };

                if (CurrentProjectInfo != null) newProjectVersion.SelectedProjectId = CurrentProjectInfo.Id.ToString();
                if (!fromSdlProjectSelection)
                {

                    if (_viewNavigation.Value.treeView_navigation.SelectedNode != null)
                    {
                        var tn = _viewNavigation.Value.treeView_navigation.SelectedNode;

                        if (tn.ImageIndex == 0)
                        {
                            newProjectVersion.SelectedProjectId = ((Project)tn.Tag).id;
                        }
                    }
                }
                newProjectVersion.Settings = (Settings)Settings.Clone();

                newProjectVersion.ShowDialog();
                if (!newProjectVersion.Saved)
                    return;


                var currentProject = Settings.projects.FirstOrDefault(project => string.Compare(project.id, newProjectVersion.SelectedProjectId, StringComparison.OrdinalIgnoreCase) == 0);

                #region  |  get settings project reference  |

                if (currentProject == null)
                {
                    currentProject = newProjectVersion.CurrentProject;
                    Settings.projects.Add(currentProject);
                }

                currentProject.projectVersions.Add(newProjectVersion.ProjectVersion);


                #endregion
                #region  |  get treenode project reference  |
                var tnProject = (from TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes
                                 let project = (Project)tn.Tag
                                 where string.Compare(project.id, newProjectVersion.SelectedProjectId, StringComparison.OrdinalIgnoreCase) == 0
                                 select tn).FirstOrDefault();

                #endregion

                if (tnProject == null)
                {
                    var tn = _viewNavigation.Value.treeView_navigation.Nodes.Add(currentProject.id, currentProject.name);
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                    tn.Tag = currentProject;
                    _viewNavigation.Value.treeView_navigation.SelectedNode = tn;
                }
                else
                {
                    tnProject.Tag = currentProject;
                    tnProject.ImageIndex = 0;
                    tnProject.SelectedImageIndex = 0;
                    _viewNavigation.Value.treeView_navigation.SelectedNode = tnProject;
                }



                SettingsSerializer.SaveSettings(Settings);

                InitializeViewControl(currentProject);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _viewNavigation.Value.label_NAVIGATION_MESSAGE.Text = string.Format(PluginResources.Projects_0, _viewNavigation.Value.treeView_navigation.Nodes.Count);
                CheckEnabledObjects();
            }


        }

        public void ViewProjectInWindowsExplorer()
        {
            if (CurrentSelectedProject == null) return;
            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) return;
            try
            {
                if (CurrentProjectInfo == null)
                {
                    SelectedProjectChanged();
                }

                var tnProject = _viewNavigation.Value.treeView_navigation.SelectedNode;
                var project = (Project)tnProject.Tag;


                var sPath = project.location;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }
                if (Directory.Exists(sPath))
                {
                    Process.Start(sPath);
                }
                else
                {
                    MessageBox.Show(PluginResources.Invalid_directory, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        public void ViewProjectVersionInWindowsExplorer()
        {
            if (CurrentSelectedProject == null) 
                return;

            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count <= 0) return;
            try
            {
                if (CurrentProjectInfo == null)
                {
                    SelectedProjectChanged();
                }

                var item = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0];
                var projectVersion = (ProjectVersion)item.Tag;

                var sPath = projectVersion.location;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }
                if (Directory.Exists(sPath))
                {
                    Process.Start(sPath);
                }
                else
                {
                    MessageBox.Show(PluginResources.Invalid_directory, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        public void EditProjectVersion()
        {
            if (CurrentSelectedProject == null) 
                return;

            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count <= 0) 
                return;

            try
            {
                if (CurrentProjectInfo == null)
                {
                    SelectedProjectChanged();
                }


                var editProjectVersion = new EditProjectVersion();

                #region  |  get parent Id  |

                var item = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0];
                var projectVersion = (ProjectVersion)item.Tag;
                var partentProjectId = projectVersion.parentId;

                #endregion


                editProjectVersion.ProjectVersion = (ProjectVersion)projectVersion.Clone();

                editProjectVersion.ShowDialog();

                if (!editProjectVersion.Saved)
                    return;

                projectVersion.name = editProjectVersion.ProjectVersion.name;
                projectVersion.description = editProjectVersion.ProjectVersion.description;
                item.Tag = projectVersion;

                item.SubItems[1].Text = projectVersion.name;
                item.SubItems[2].Text = projectVersion.description;


                SettingsSerializer.SaveSettings(Settings);

                listView_postEditCompareProjectVersions_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void RemoveProjectVersions()
        {
            if (CurrentSelectedProject == null) 
                return;

            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count <= 0) 
                return;

            var dialogResult = MessageBox.Show(
                PluginResources.RemoveProjectVersions
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            if (CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

            try
            {

                #region  |  get parent Id  |

                var selectedItem = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0];
                var projectVersion = (ProjectVersion)selectedItem.Tag;
                var partentProjectId = projectVersion.parentId;



                #endregion

                #region  |  get treenode project reference  |
                var tnProject = (from TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes
                                 let project = (Project)tn.Tag
                                 where string.Compare(project.id, partentProjectId, StringComparison.OrdinalIgnoreCase) == 0
                                 select tn).FirstOrDefault();

                #endregion

                #region  |  get settings project reference  |

                var currentProject = Settings.projects.FirstOrDefault(project => string.Compare(project.id, partentProjectId, StringComparison.OrdinalIgnoreCase) == 0);

                #endregion

                if (currentProject != null)
                {
                    currentProject.projectVersions = new List<ProjectVersion>();

                    foreach (ListViewItem item in _viewContent.Value.listView_postEditCompareProjectVersions.Items)
                    {
                        var version = (ProjectVersion)item.Tag;

                        if (!item.Selected)
                        {

                            currentProject.projectVersions.Add(version);
                        }
                        else
                        {
                            try
                            {
                                if (string.Compare(version.location.TrimEnd('\\'), Settings.versions_folder_path.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    Empty(new DirectoryInfo(version.location));
                                }

                            }
                            catch
                            {
                                // ignored
                            }

                            item.Remove();
                        }
                    }

                    if (tnProject != null)
                    {
                        var projectFromTn = (Project)tnProject.Tag;
                        projectFromTn.projectVersions = currentProject.projectVersions;
                        tnProject.Tag = projectFromTn;
                    }
                }

                SettingsSerializer.SaveSettings(Settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                _viewContent.Value.label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count.ToString();
                _viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Count.ToString();
                CheckEnabledObjects();
            }
        }
        public void RemoveAllProjectVersions()
        {
            if (CurrentSelectedProject == null)
                return;

            if (_viewNavigation.Value.treeView_navigation.SelectedNode == null) 
                return;

            var dr = MessageBox.Show(
                PluginResources.RemoveAllProjectVersions
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) 
                return;

            if (CurrentProjectInfo == null)
            {
                SelectedProjectChanged();
            }

            try
            {

                var tnProject = _viewNavigation.Value.treeView_navigation.SelectedNode;
                var project = (Project)tnProject.Tag;



                for (var i = 0; i < Settings.projects.Count; i++)
                {
                    var settingsProject = Settings.projects[i];
                    if (string.Compare(project.id, settingsProject.id, StringComparison.OrdinalIgnoreCase) != 0) 
                        continue;

                    foreach (var projectVersion in settingsProject.projectVersions)
                    {
                        try
                        {
                            if (string.Compare(projectVersion.location.TrimEnd('\\'), Settings.versions_folder_path.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                Empty(new DirectoryInfo(projectVersion.location));
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    Settings.projects.RemoveAt(i);
                    break;
                }
                _viewContent.Value.listView_postEditCompareProjectVersions.Items.Clear();
                _viewNavigation.Value.treeView_navigation.Nodes.Remove(tnProject);

                SettingsSerializer.SaveSettings(Settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _viewNavigation.Value.label_NAVIGATION_MESSAGE.Text = string.Format(PluginResources.Projects_0, _viewNavigation.Value.treeView_navigation.Nodes.Count);
                _viewContent.Value.label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count.ToString();
                _viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Count.ToString();

                CheckEnabledObjects();
            }
        }
        public void RestoreProjectVersion()
        {
            if (CurrentSelectedProject == null) return;
            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count <= 0) return;
            try
            {
                if (CurrentProjectInfo == null)
                {
                    SelectedProjectChanged();
                }


                var restoreProjectVersion = new RestoreProjectVersion { ProjectsController = ProjectsController };



                #region  |  get parent Id  |

                var item = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0];
                var projectVersion = (ProjectVersion)item.Tag;
                var partentProjectId = projectVersion.parentId.Trim();

                #endregion



                if (CurrentProjectInfo != null && CurrentProjectInfo.Id.ToString().Trim() != partentProjectId)
                {
                    var projects = ProjectsController.GetProjects().ToList();
                    foreach (var proj in projects)
                    {
                        if (
                            string.Compare(partentProjectId, proj.GetProjectInfo().Id.ToString(),
                                StringComparison.OrdinalIgnoreCase) != 0) continue;
                        restoreProjectVersion.CurrentProjectInfo = proj.GetProjectInfo();
                        break;
                    }
                }
                else
                {
                    restoreProjectVersion.CurrentProjectInfo = CurrentProjectInfo;
                }


                #region  |  get settings project reference  |
                
                var currentProject = Settings.projects.FirstOrDefault(project => string.Compare(project.id, partentProjectId, StringComparison.OrdinalIgnoreCase) == 0);

                #endregion

                #region  |  get treenode project reference  |
                var tnProject = (
                    from TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes
                    let project = (Project)tn.Tag
                    where currentProject != null && string.Compare(project.id, currentProject.id, StringComparison.OrdinalIgnoreCase) == 0
                    select tn).FirstOrDefault();

                #endregion

                var version = (ProjectVersion)item.Tag;


                restoreProjectVersion.ProjectVersion = (ProjectVersion)version.Clone();
                restoreProjectVersion.CurrentProject = currentProject;
                restoreProjectVersion.Settings = (Settings)Settings.Clone();

                restoreProjectVersion.ShowDialog();

                if (!restoreProjectVersion.Saved) 
                    return;

                if (currentProject == null) 
                    return;

                currentProject.projectVersions.Add(restoreProjectVersion.ProjectVersionNew);


                if (tnProject != null)
                {
                    tnProject.Tag = currentProject;
                    tnProject.ImageIndex = 0;
                    tnProject.SelectedImageIndex = 0;
                    _viewNavigation.Value.treeView_navigation.SelectedNode = tnProject;
                }


                SettingsSerializer.SaveSettings(Settings);

                InitializeViewControl(currentProject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public bool IsEnabledCompare { get; set; }
        public bool IsEnabledCreateNewProjectVersion { get; set; }
        public bool IsEnabledEditProjectVersion { get; set; }
        public bool IsEnabledDeleteProjectVersion { get; set; }
        public bool IsEnabledDeleteProject { get; set; }
        public bool IsEnabledRestoreProjectVersion { get; set; }


        private void CheckEnabledObjects()
        {
            CurrentSelectedProject = ProjectsController.SelectedProjects.FirstOrDefault() ??
                                     ProjectsController.CurrentProject;

            if (CurrentSelectedProject != null)
            {
                CurrentProjectInfo = CurrentSelectedProject.GetProjectInfo();
            }


            if (CurrentSelectedProject == null || CurrentProjectInfo == null) return;



            IsEnabledCreateNewProjectVersion = CurrentSelectedProject != null;
            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count == 2)
            {
                var projectVersion01 = (ProjectVersion)_viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0].Tag;
                var projectVersion02 = (ProjectVersion)_viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[1].Tag;

                if (projectVersion01.filesCopiedCount > 0 && projectVersion01.filesCopiedCount > 0)
                    IsEnabledCompare = true;
                else
                    IsEnabledCompare = false;
            }
            else
            {
                IsEnabledCompare = false;
            }

            if (_viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count == 1)
            {
                IsEnabledEditProjectVersion = true;
                IsEnabledRestoreProjectVersion = true;
            }
            else
            {
                IsEnabledEditProjectVersion = false;
                IsEnabledRestoreProjectVersion = false;
            }

            IsEnabledDeleteProjectVersion = _viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count > 0;

            IsEnabledDeleteProject = _viewNavigation.Value.treeView_navigation.SelectedNode != null;





            _viewContent.Value.compareWithPostEditCompareToolStripMenuItem.Enabled = IsEnabledCompare;
            _viewContent.Value.saveNewProjectVersionToolStripMenuItem.Enabled = IsEnabledCreateNewProjectVersion;
            _viewContent.Value.editSelectedProjectVersionToolStripMenuItem.Enabled = IsEnabledEditProjectVersion;
            _viewContent.Value.removeSelectedProjectVersionsToolStripMenuItem.Enabled = IsEnabledDeleteProjectVersion;
            _viewContent.Value.viewProjectVersionInWindowsExplorerToolStripMenuItem.Enabled = IsEnabledDeleteProjectVersion;
            _viewContent.Value.restoreProjectVersionToolStripMenuItem.Enabled = IsEnabledRestoreProjectVersion;

            _viewNavigation.Value.createNewProjectVersionToolStripMenuItem.Enabled = IsEnabledCreateNewProjectVersion;
            _viewNavigation.Value.removeAllVersionsForTheSelectedProjectToolStripMenuItem.Enabled = IsEnabledDeleteProject;
            _viewNavigation.Value.linkLabel_removeProjectVersions.Enabled = IsEnabledDeleteProject;
            _viewNavigation.Value.viewProjectInWindowsExplorerToolStripMenuItem.Enabled = IsEnabledDeleteProject;

            if (CheckEnabledObjectsEvent != null)
            {
                CheckEnabledObjectsEvent(this, EventArgs.Empty);
            }
        }


        private void InitializeNavigationControl(List<Project> projects)
        {
            try
            {
                _viewNavigation.Value.treeView_navigation.BeginUpdate();



                TreeNode tnSelected = null;
                foreach (var project in projects)
                {
                    var tn = _viewNavigation.Value.treeView_navigation.Nodes.Add(project.id, project.name);
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                    tn.Tag = project;

                    if (CurrentProjectInfo != null
                        && string.Compare(project.id, CurrentProjectInfo.Id.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        tnSelected = tn;
                    }
                }

                if (tnSelected == null
                    && _viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
                    tnSelected = _viewNavigation.Value.treeView_navigation.Nodes[0];


                _viewNavigation.Value.treeView_navigation.SelectedNode = tnSelected;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _viewNavigation.Value.treeView_navigation.EndUpdate();

                _viewNavigation.Value.label_NAVIGATION_MESSAGE.Text = string.Format(PluginResources.Projects_0, _viewNavigation.Value.treeView_navigation.Nodes.Count);
            }
        }
        private void InitializeViewControl(Project project)
        {

            var refreshList = true;
            if (IsUpdatingNavigationView)
            {
                if (_viewContent.Value.listView_postEditCompareProjectVersions.Items.Count > 0)
                {
                    var firstItem = _viewContent.Value.listView_postEditCompareProjectVersions.Items[0];
                    var projectVersion = (ProjectVersion)firstItem.Tag;

                    if (projectVersion.parentId == project.id)
                        refreshList = false;
                }
            }

            if (!refreshList) return;
            {
                try
                {

                    _viewContent.Value.listView_postEditCompareProjectVersions.BeginUpdate();

                    _viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = "0";
                    _viewContent.Value.label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = "0";
                    _viewContent.Value.listView_postEditCompareProjectVersions.Items.Clear();

					#region  |  add project versions to list  |
					AddProjectVersionsToList(project.projectVersions);
					//foreach (var projectVersion in project.projectVersions)
					//{
					//    var item = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Add(projectVersion.id);
					//    item.SubItems.Add(projectVersion.name);
					//    item.SubItems.Add(projectVersion.description);
					//    item.SubItems.Add(projectVersion.createdAt);

					//    var sourceLanguage = projectVersion.sourceLanguage.name;
					//    var targetLanguages = projectVersion.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);

					//    item.SubItems.Add(sourceLanguage);
					//    item.SubItems.Add(targetLanguages);
					//    item.SubItems.Add(projectVersion.filesCopiedCount.ToString());
					//    item.SubItems.Add(projectVersion.shallowCopy.ToString());

					//    var fileCount = string.Format(PluginResources._0_translatable_1_reference, projectVersion.translatableCount, projectVersion.referenceCount);
					//    item.SubItems.Add(fileCount);


					//    item.SubItems.Add(projectVersion.location);
					//    item.Tag = projectVersion;


					//    if (Directory.Exists(projectVersion.location))
					//    {
					//        if (projectVersion.filesCopiedCount == 0)
					//        {
					//            item.ImageKey = @"Warning";
					//        }
					//        else if (projectVersion.translatableCount > 0)
					//        {
					//            item.ImageKey = projectVersion.shallowCopy ? "Blue" : "Green";
					//        }
					//        else
					//        {
					//            item.ImageKey = @"Yellow";
					//        }
					//    }
					//    else
					//    {
					//        item.ImageKey = @"Red";
					//    }


					//}
					#endregion

					_viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Count.ToString();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _viewContent.Value.listView_postEditCompareProjectVersions.EndUpdate();
                }
            }
        }


		 public void AddProjectVersionsToList(List<ProjectVersion> projectVersions)
		{
			foreach (var projectVersion in projectVersions)
			{
				var item = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Add(projectVersion.id);
				item.SubItems.Add(projectVersion.name);
				item.SubItems.Add(projectVersion.description);
				item.SubItems.Add(projectVersion.createdAt);

				var sourceLanguage = projectVersion.sourceLanguage.name;
				var targetLanguages = projectVersion.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);

				item.SubItems.Add(sourceLanguage);
				item.SubItems.Add(targetLanguages);
				item.SubItems.Add(projectVersion.filesCopiedCount.ToString());
				item.SubItems.Add(projectVersion.shallowCopy.ToString());

				var fileCount = string.Format(PluginResources._0_translatable_1_reference, projectVersion.translatableCount, projectVersion.referenceCount);
				item.SubItems.Add(fileCount);


				item.SubItems.Add(projectVersion.location);
				item.Tag = projectVersion;


				if (Directory.Exists(projectVersion.location))
				{
					if (projectVersion.filesCopiedCount == 0)
					{
						item.ImageKey = @"Warning";
					}
					else if (projectVersion.translatableCount > 0)
					{
						item.ImageKey = projectVersion.shallowCopy ? "Blue" : "Green";
					}
					else
					{
						item.ImageKey = @"Yellow";
					}
				}
				else
				{
					item.ImageKey = @"Red";
				}


			}
		}
        private void ViewControlAdd(ProjectVersion projectVersion)
        {
            try
            {
                _viewContent.Value.listView_postEditCompareProjectVersions.BeginUpdate();


                #region  |  add project versions to list  |


                var item = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Add(projectVersion.name);
                item.SubItems.Add(projectVersion.description);
                item.SubItems.Add(projectVersion.createdAt);

                var sourceLanguage = projectVersion.sourceLanguage.name;
                var targetLanguages = projectVersion.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);

                item.SubItems.Add(sourceLanguage);
                item.SubItems.Add(targetLanguages);

                item.SubItems.Add(projectVersion.shallowCopy.ToString());

                var fileCount = string.Format(PluginResources._0_translatable_1_reference, projectVersion.translatableCount, projectVersion.referenceCount);
                item.SubItems.Add(fileCount);

                item.SubItems.Add(projectVersion.projectType);
                item.SubItems.Add(projectVersion.location);
                item.Tag = projectVersion;


                if (Directory.Exists(projectVersion.location))
                {
                    if (projectVersion.translatableCount > 0)
                    {
                        item.ImageKey = projectVersion.shallowCopy ? @"Blue" : @"Green";
                    }
                    else
                    {
                        item.ImageKey = @"Yellow";
                    }
                }
                else
                {
                    item.ImageKey = @"Red";
                }

                #endregion

                _viewContent.Value.label_TOTAL_PROJECT_VERSIONS.Text = _viewContent.Value.listView_postEditCompareProjectVersions.Items.Count.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _viewContent.Value.listView_postEditCompareProjectVersions.EndUpdate();
            }


        }

        public void SelectedProjectChanged()
        {
            try
            {

                CurrentSelectedProject = ProjectsController.SelectedProjects.FirstOrDefault() ??
                                         ProjectsController.CurrentProject;

                if (CurrentSelectedProject != null)
                {
                    CurrentProjectInfo = CurrentSelectedProject.GetProjectInfo();
                }

                if (CurrentSelectedProject == null || CurrentProjectInfo == null) return;
                var projects = ProjectsController.GetProjects().ToList();
                var currentProjectListIds = projects.Select(proj => proj.GetProjectInfo().Id.ToString()).ToList();


                if (CurrentSelectedProject == null) return;
                TreeNode tnSelected = null;
                foreach (TreeNode tn in _viewNavigation.Value.treeView_navigation.Nodes)
                {
                    var project = (Project)tn.Tag;
                    if (string.Compare(project.id, CurrentProjectInfo.Id.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        tnSelected = tn;
                    }

                    if (currentProjectListIds.Contains(project.id))
                    {
                        tn.ImageIndex = 0;
                        tn.SelectedImageIndex = 0;
                    }
                    else
                    {
                        tn.ImageIndex = 1;
                        tn.SelectedImageIndex = 1;
                    }
                }


                if (tnSelected == null && _viewNavigation.Value.treeView_navigation.Nodes.Count > 0)
                {
                    tnSelected = _viewNavigation.Value.treeView_navigation.Nodes[0];
                }


                _viewNavigation.Value.treeView_navigation.SelectedNode = tnSelected;
            }
            finally
            {
                CheckEnabledObjects();
            }
        }

        private void UpdateviewPartsDetails()
        {
            var viewPartsDetails = SdlTradosStudio.Application.GetController<PostEditCompareViewPartController>();

            try
            {
                if (_viewNavigation.Value.treeView_navigation.SelectedNode != null
                    && _viewContent.Value.listView_postEditCompareProjectVersions.SelectedIndices.Count > 0)
                {

                    var tnSelected = _viewNavigation.Value.treeView_navigation.SelectedNode;
                    var project = (Project)tnSelected.Tag;


                    var projectVersion = (ProjectVersion)_viewContent.Value.listView_postEditCompareProjectVersions.SelectedItems[0].Tag;


                    #region  |  update properties view part  |

                    project.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);


                    var projectVersionTargetLanguages = projectVersion.targetLanguages.Aggregate(string.Empty, (current, language) => current + (current.Trim() != string.Empty ? ", " : string.Empty) + language.name);


                    var pvd = new ProjectVersionDetails
                    {
                        ProjectId = project.id,
                        ProjectName = project.name,
                        ProjectDescription = project.description,
                        ProjectLocation = project.location,
                        ProjectCreatedAt =
                            Helper.GetDateTimeFromString(project.createdAt).ToString(CultureInfo.InvariantCulture),
                        ProjectCreatedBy = project.createdBy,
                        ProjectVersionId = projectVersion.id,
                        ProjectVersionName = projectVersion.name,
                        ProjectVersionDescription = projectVersion.description,
                        ProjectVersionLocation = projectVersion.location,
                        ProjectVersionCreatedAt =
                            Helper.GetDateTimeFromString(projectVersion.createdAt).ToString(CultureInfo.InvariantCulture),
                        ProjectVersionCreatedBy = projectVersion.createdBy,
                        ProjectVersionSourceLanguage = projectVersion.sourceLanguage.name,
                        ProjectVersionTargetLanguages = projectVersionTargetLanguages,
                        ProjectVersionShallowCopy = projectVersion.shallowCopy.ToString(),
                        ProjectVersionTotalFiles = projectVersion.filesCopiedCount.ToString(),
                        ProjectVersionTranslatableCount = projectVersion.translatableCount.ToString(),
                        ProjectVersionLocalizableCount = projectVersion.localizableCount.ToString(),
                        ProjectVersionReferenceCount = projectVersion.referenceCount.ToString(),
                        ProjectVersionUnknownCount = projectVersion.unKnownCount.ToString()
                    };


                    viewPartsDetails.Control.Value.propertyGrid1.SelectedObject = pvd;


                    #endregion
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
            finally
            {

                CheckEnabledObjects();
            }
        }

        private static void Empty(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles()) file.Delete();
            foreach (var subDirectory in directory.GetDirectories()) subDirectory.Delete(true);

            directory.Delete(true);
        }
    }
}