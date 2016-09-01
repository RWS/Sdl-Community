using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Controls;
using Sdl.Community.StudioMigrationUtility.Model;
using Sdl.Community.StudioMigrationUtility.Properties;
using Sdl.Community.StudioMigrationUtility.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioMigrationUtility
{
    public partial class MigrateUtility : Form
    {
        private readonly StudioVersionService _studioVersionService;
        private BackgroundWorker _bw;
        private List<Project> _projects=new List<Project>();

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
            _bw = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.DoWork += _bw_DoWork;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;

        }

     

        private void MigrateUtility_Closing(object sender, CancelEventArgs e)
        {
            if (_projects.Count > 0)
            {
                foreach (var project in _projects)
                {
                    GetProjectsController().Add(project.ProjectFilePath);
                }
                GetProjectsController().RefreshProjects();
            }
           
        }

        private void projectMigrationWizzard_BeforeSwitchPages(object sender, CristiPotlog.Controls.Wizard.BeforeSwitchPagesEventArgs e)
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
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Please_select_a_Studio_version_,
                        Resources.MigrateProjects_projectMigrationWizzard_BeforeSwitchPages_Select_a_studio_version, MessageBoxButtons.OK);
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

                foreach (OLVListItem item in from OLVListItem item in projectsToBeMoved.Items let project = (Project)item.RowObject where !Path.IsPathRooted(project.ProjectFilePath) select item)
                {
                    if (item.Text != @"Select all projects")
                    {
                        item.Checked = true;
                    }

                }

                projectsToBeMoved.ItemChecked += ProjectsToBeMoved_ItemChecked;
             
            }

        }

        private void ProjectsToBeMoved_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var selectedItem = e.Item;

            if (selectedItem.Text.Equals("Select all projects")&&selectedItem.Checked)
            {
                foreach (var item in projectsToBeMoved.Items)
                {
                    ((OLVListItem) item).Checked = true;
                }
            }else if (selectedItem.Text.Equals("Select all projects") && !selectedItem.Checked)
            {
                foreach (var item in projectsToBeMoved.Items)
                {
                    ((OLVListItem)item).Checked = false;
                }
            }
          
        }


        private void projectMigrationWizzard_AfterSwitchPages(object sender, CristiPotlog.Controls.Wizard.AfterSwitchPagesEventArgs e)
        {
            // get wizard page to be displayed
            WizardPage newPage = projectMigrationWizzard.Pages[e.NewIndex];

            if (newPage == taskRunnerPage)
            {
                StartTask();
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

            var taskArgument = new TaskArgument
            {
                Projects =  projects.Cast<Project>().ToList(),
                ProjectToBeMoved = selectedProjectsToBeMoved.Cast<Project>().ToList(),
                DestinationStudioVersion = (StudioVersion) selectedDestinationStudioVersionsGeneric[0],
                SourceStudioVersion = (StudioVersion) selectedSourceStudioVersionsGeneric[0],
                MigrateTranslationMemories = chkTranslationMemories.Checked,
                MigrateCustomers = chkCustomers.Checked
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
            _bw.ReportProgress(100);

        }

        void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressLongTask.Value = e.ProgressPercentage;
        }
    }
}
