using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Controls;
using Sdl.Community.StudioMigrationUtility.Model;
using Sdl.Community.StudioMigrationUtility.Properties;
using Sdl.Community.StudioMigrationUtility.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioMigrationUtility
{
    public partial class MigrateUtility : Form
    {
        private readonly StudioVersionService _studioVersionService;
        private BackgroundWorker _bw;
        private List<Project> _projects=new List<Project>();
        private List<PluginInfo> _pluginsToBeMigrated = new List<PluginInfo>(); 
        public MigrateUtility(StudioVersionService studioVersionService)
        {
            InitializeComponent();

            _studioVersionService = studioVersionService;
        }

        public ProjectsController GetProjectsController()
        {
            return SdlTradosStudio.Application.GetController<ProjectsController>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var installedStudioVersions = _studioVersionService.GetInstalledStudioVersions();

            Closing += MigrateUtility_Closing;
            
            sourceStudioVersionColumn.AspectGetter = delegate(object rowObject)
            {
                var studioVersion = (StudioVersion) rowObject;
                return string.Format("{0} - {1}", studioVersion.PublicVersion, studioVersion.ExecutableVersion);
            };

            chkSourceStudioVersions.SetObjects(installedStudioVersions);

            destinationStudioVersionColumn.AspectGetter = delegate(object rowObject)
            {
                var studioVersion = (StudioVersion) rowObject;
                return string.Format("{0} - {1}", studioVersion.PublicVersion, studioVersion.ExecutableVersion);
            };

            chkDestinationStudioVersion.SetObjects(installedStudioVersions);

            projectsNameColumn.AspectGetter = delegate(object rowObject)
            {
                var project = (Project)rowObject;
                return project.Name;
            };

            pluginsColumn.AspectGetter = delegate(object rowObject)
            {
                var plugin = (PluginInfo) rowObject;
                return plugin.PluginName;
            };
            _bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

            installedPluginsListView.CellToolTipShowing += InstalledPluginsListView_CellToolTipShowing;
        }

        private void InstalledPluginsListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            var index = ((ObjectListView)sender).HotRowIndex;
            var plugin = _pluginsToBeMigrated[index];
            e.Text = plugin.PluginName;
        }

        private void MigrateUtility_Closing(object sender, CancelEventArgs e)
        {
            var controllerProjects = GetProjectsController().GetProjects().ToList();

            if (_projects.Count > 0)
            {
                foreach (var project in _projects)
                {
                    var exist = controllerProjects.Any(p => p.FilePath.Contains(project.Name));
                    if (!exist)
                    {
                        GetProjectsController().Add(project.ProjectFilePath);
                    }
                    
                }
                GetProjectsController().RefreshProjects();
            }

        }

        private void projectMigrationWizzard_BeforeSwitchPages(object sender,
            CristiPotlog.Controls.Wizard.BeforeSwitchPagesEventArgs e)
        {
            // get wizard page already displayed
            WizardPage oldPage = projectMigrationWizzard.Pages[e.OldIndex];

            // check if we're going forward from options page
            if (oldPage == fromStudioVersion && e.NewIndex > e.OldIndex)
            {
                var selectedStudioVersionsGeneric = chkSourceStudioVersions.CheckedObjects;
                if (selectedStudioVersionsGeneric.Count == 0)
                {
                    MessageBox.Show(this,
                        Resources
                            .MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Please_select_a_Studio_version_,
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Select_a_studio_version,
                        MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }
                if (selectedStudioVersionsGeneric.Count > 1)
                {
                    MessageBox.Show(this,
                        Resources
                            .MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Please_select_only_one_version,
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Only_one_version
                        , MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }

               
            }

            if (oldPage == toStudioVersion && e.NewIndex > e.OldIndex)
            {
                
                var selectedDestinationStudioVersionsGeneric = chkDestinationStudioVersion.CheckedObjects;
                if (selectedDestinationStudioVersionsGeneric.Count == 0)
                {
                    MessageBox.Show(this,
                        Resources
                            .MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Please_select_a_Studio_version_,
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Select_a_studio_version,
                        MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }
                if (selectedDestinationStudioVersionsGeneric.Count > 1)
                {
                    MessageBox.Show(this,
                        Resources
                            .MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Please_select_only_one_version,
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Only_one_version
                        , MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }
                var selectedSourceStudioVersionsGeneric = chkSourceStudioVersions.CheckedObjects;
                var destinationStudioVersion = (StudioVersion) selectedDestinationStudioVersionsGeneric[0];
                var sourceStudioVersion = (StudioVersion) selectedSourceStudioVersionsGeneric[0];

                if (destinationStudioVersion.Equals(selectedSourceStudioVersionsGeneric[0]))
                {
                    MessageBox.Show(this,
                        string.Format("Destination version ({0}) must be different than the source version ({1})",
                            destinationStudioVersion.PublicVersion, sourceStudioVersion.PublicVersion),
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Same_version
                        , MessageBoxButtons.OK);
                    e.Cancel = true;
                    return;
                }

                var migrateProjects = new MigrateProjectsService(sourceStudioVersion,
                    destinationStudioVersion);

                var selectAllProjects = new Project {Name = "Select all projects"};

                var projects = migrateProjects.GetProjectsToBeMigrated();

                if (projects.Count != 0 && projects.Count != 1)
                {
                    projects.Insert(0, selectAllProjects);
                }


                projectsToBeMoved.SetObjects(projects);

                foreach (OLVListItem item in from OLVListItem item in projectsToBeMoved.Items
                    let project = (Project) item.RowObject
                    where !Path.IsPathRooted(project.ProjectFilePath)
                    select item)
                {
                    if (item.Text != @"Select all projects")
                    {
                        item.Checked = true;
                    }

                }

                projectsToBeMoved.ItemChecked += ProjectsToBeMoved_ItemChecked;

                if (projectsToBeMoved.Items.Count > 0)
                {
                    if (projectsToBeMoved.Items[0].Text == @"Select all projects")
                    {
                        projectsToBeMoved.Items[0].ForeColor = ColorTranslator.FromHtml("#6887B2");
                        projectsToBeMoved.Items[0].Font = new Font(projectsToBeMoved.Items[0].Font, FontStyle.Bold);

                    }
                }
              
            }


            #region Plugins page
            if (oldPage == moveProjects && e.NewIndex > e.OldIndex)
            {

                //call Sdl.Community.PluginInfo.dll in order to get the plugin name
                var currentAssembly = Assembly.GetAssembly(typeof(MigrateProjectsService));
                var path = Path.GetDirectoryName(currentAssembly.Location);
                var pluginInfoDll = Assembly.LoadFrom(Path.Combine(path, "Sdl.Community.PluginInfo"));
                var type = pluginInfoDll.GetType("Sdl.Community.PluginInfo.PluginPackageInfo");

                var selectedSourceStudioVersionsGeneric = chkSourceStudioVersions.CheckedObjects;
                var sourceStudioVersion = (StudioVersion)selectedSourceStudioVersionsGeneric[0];

                var selectedDestinationStudioVersionsGeneric = chkDestinationStudioVersion.CheckedObjects;
                var destinationStudioVersion = (StudioVersion)selectedDestinationStudioVersionsGeneric[0];

                //get plugins list for source selected studio  version
                var sourcePluginsList = GetInstallledPluginsForSpecificStudioVersion(sourceStudioVersion);

                //get plugins list for destination selected studio  version
                var destinationPluginsPathList = GetInstallledPluginsForSpecificStudioVersion(destinationStudioVersion);

                //check if there are any plugins to be migrated for selected verions 
                var pluginsList =
                    sourcePluginsList.Where(p => destinationPluginsPathList.All(d => d.PluginName != p.PluginName));

                //call "GetPluginName" method in order to get the plugin name
                var method = type.GetMethod("GetPluginName");
                var constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, Type.EmptyTypes, null);
                var instance = constructor.Invoke(new object[] { });

                foreach (var plugin in pluginsList)
                {
                    var name = method.Invoke(instance, new object[] { plugin.Path });
                    var pluginToBeMoved = new PluginInfo
                    {
                        Path = plugin.Path,
                        PluginName = name as string
                    };

                    if (!_pluginsToBeMigrated.Exists(p => p.PluginName == pluginToBeMoved.PluginName))
                    {
                        _pluginsToBeMigrated.Add(pluginToBeMoved);
                    }

                }

                var selectAllPlugins = new PluginInfo
                {
                    PluginName = "Select all plugins"
                };

                if (_pluginsToBeMigrated.Count > 1&&!_pluginsToBeMigrated.Exists(p => p.PluginName == selectAllPlugins.PluginName))
                {
                    _pluginsToBeMigrated.Insert(0,selectAllPlugins);
                }

                installedPluginsListView.SetObjects(_pluginsToBeMigrated);


                installedPluginsListView.ItemChecked += InstalledPluginsListView_ItemChecked;

                if (installedPluginsListView.Items[0].Text == @"Select all plugins")
                {
                    installedPluginsListView.Items[0].ForeColor = ColorTranslator.FromHtml("#6887B2");
                    installedPluginsListView.Items[0].Font = new Font(installedPluginsListView.Items[0].Font,FontStyle.Bold);
                }
            }
            #endregion
        }

        private void InstalledPluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var selectedItem = e.Item;

            if (selectedItem.Text.Equals("Select all plugins") && selectedItem.Checked)
            {
                SelectPlugins(true);
            }else if(selectedItem.Text.Equals("Select all plugins") && !selectedItem.Checked)
            {
                SelectPlugins(false);
            }
        }

        private void SelectPlugins(bool check)
        {
            foreach (var plugin in installedPluginsListView.Items)
            {
                ((OLVListItem)plugin).Checked = check;
            }
        }
        private List<PluginInfo> GetInstallledPluginsForSpecificStudioVersion(StudioVersion studioVersion)
        {
            var pluginService = new PluginService();

            var pluginsList = new List<PluginInfo>();
            foreach (var path in pluginService.GetInstalledPlugins(studioVersion))
            {
                var plugin = new PluginInfo
                {
                    Path = path,
                    PluginName = Path.GetFileNameWithoutExtension(path)
                };
                pluginsList.Add(plugin);
            }

            return pluginsList;
        } 

        private void ProjectsToBeMoved_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var selectedItem = e.Item;
            
            if (selectedItem.Text.Equals("Select all projects")&&selectedItem.Checked)
            {
                SelectProjects(true);
            }else if (selectedItem.Text.Equals("Select all projects") && !selectedItem.Checked)
            {
                SelectProjects(false);
            }
          
        }

        private void SelectProjects(bool check)
        {
            foreach (var item in projectsToBeMoved.Items)
            {
                ((OLVListItem)item).Checked = check;
            }
        }

        private void projectMigrationWizzard_AfterSwitchPages(object sender, CristiPotlog.Controls.Wizard.AfterSwitchPagesEventArgs e)
        {
            // get wizard page to be displayed
            WizardPage newPage = projectMigrationWizzard.Pages[e.NewIndex];
            var selectedSourceStudioVersionsGeneric = chkSourceStudioVersions.CheckedObjects;
            var sourceStudioVersion= new StudioVersion();
            var destinationStudioVersion = new StudioVersion();
            var skipPluginsPage=true;

            if (selectedSourceStudioVersionsGeneric.Count > 0)
            {
                 sourceStudioVersion = (StudioVersion)selectedSourceStudioVersionsGeneric[0];
            }
 
            var selectedDestinationStudioVersionsGeneric = chkDestinationStudioVersion.CheckedObjects;
            if (selectedDestinationStudioVersionsGeneric.Count > 0)
            {
                 destinationStudioVersion = (StudioVersion)selectedDestinationStudioVersionsGeneric[0];
            }

            if (destinationStudioVersion.Version != null && (sourceStudioVersion.Version != null && (sourceStudioVersion.Version.Equals("Studio4") && destinationStudioVersion.Version.Equals("Studio5"))))
            {
                skipPluginsPage = false;
            }

            if (newPage == taskRunnerPage)
            {
                StartTask();
            }
            //this will skip the plugins page if selected versions are different from 2015-2017
            if (newPage == pluginsPage && e.OldIndex==3&&skipPluginsPage)
            {
                projectMigrationWizzard.Next();
               
            }
            if (newPage == pluginsPage && e.OldIndex == 5&&skipPluginsPage)
            {
                projectMigrationWizzard.Back();
               
            }
        }

        private void StartTask()
        {
            progressLongTask.Value = progressLongTask.Minimum;
            projectMigrationWizzard.NextEnabled = false;
            projectMigrationWizzard.BackEnabled = false;

            var selectedDestinationStudioVersionsGeneric = chkDestinationStudioVersion.CheckedObjects;
            var selectedSourceStudioVersionsGeneric = chkSourceStudioVersions.CheckedObjects;
            var selectedProjectsToBeMoved = projectsToBeMoved.CheckedObjects;
            var projects = projectsToBeMoved.Objects;
            var selectedPluginsToBeMoved = installedPluginsListView.CheckedObjects;

            var taskArgument = new TaskArgument
            {
                Projects =  projects.Cast<Project>().ToList(),
                ProjectToBeMoved = selectedProjectsToBeMoved.Cast<Project>().ToList(),
                DestinationStudioVersion = (StudioVersion) selectedDestinationStudioVersionsGeneric[0],
                SourceStudioVersion = (StudioVersion) selectedSourceStudioVersionsGeneric[0],
                MigrateTranslationMemories = chkTranslationMemories.Checked,
                MigrateCustomers = chkCustomers.Checked,
                PluginsToBeMoved = selectedPluginsToBeMoved.Cast<PluginInfo>().ToList()
            };

            _bw.RunWorkerAsync(taskArgument);
        }

        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            projectMigrationWizzard.NextEnabled =true;
            projectMigrationWizzard.BackEnabled = true;
            if (e.Cancelled)
            {
                projectMigrationWizzard.Back();
            }
            else
            {
                projectMigrationWizzard.Next();
            }
           
        }

        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument == null) return;

            var taskArgument = (TaskArgument)e.Argument;

            
            var mpService = new MigrateProjectsService(taskArgument.SourceStudioVersion, taskArgument.DestinationStudioVersion);

            //remove the Select al project object which coresponds with to the first line in the list view
            var selectAllProjectsObj = taskArgument.Projects.FirstOrDefault(s => s.Name == @"Select all projects");
            if (selectAllProjectsObj != null)
            {
                taskArgument.Projects.Remove(selectAllProjectsObj);
            }

            var selectAllProj = taskArgument.ProjectToBeMoved.FirstOrDefault(s => s.Name == @"Select all projects");
            if (selectAllProj != null)
            {
                taskArgument.ProjectToBeMoved.Remove(selectAllProj);
            }
            _projects = taskArgument.ProjectToBeMoved;

            mpService.MigrateProjects(taskArgument.Projects, taskArgument.ProjectToBeMoved, taskArgument.MigrateCustomers, _bw.ReportProgress);
            _bw.ReportProgress(95);

            if (taskArgument.MigrateTranslationMemories)
            {
                mpService.MigrateTranslationMemories();
            }

            if (taskArgument.PluginsToBeMoved.Count > 0)
            {
                var selectAllPlugins = taskArgument.PluginsToBeMoved.FirstOrDefault(s => s.PluginName == @"Select all plugins");
                if (selectAllPlugins != null)
                {
                    taskArgument.PluginsToBeMoved.Remove(selectAllPlugins);
                }
                MovePlugins(taskArgument.PluginsToBeMoved, taskArgument.SourceStudioVersion.ExecutableVersion.Major);
            }
           
            _bw.ReportProgress(100);

        }

        private void MovePlugins(List<PluginInfo> pluginsToBeMoved,int sourceStudioVersion)
        {
            var selectedDestinationStudioVersionsGeneric = chkDestinationStudioVersion.CheckedObjects;
            var destinationStudioVersion = (StudioVersion)selectedDestinationStudioVersionsGeneric[0];

            var majorVersion = destinationStudioVersion.ExecutableVersion.Major;

            foreach (var plugin in pluginsToBeMoved)
            {
                var destinationPath = plugin.Path.Replace(sourceStudioVersion.ToString(), majorVersion.ToString());
                File.Copy(plugin.Path,destinationPath,true);
            }
        }
        void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressLongTask.Value = e.ProgressPercentage;
        }
    }
}
