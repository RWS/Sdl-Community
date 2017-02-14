using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Qualitivity.Panels.Main
{


    [RibbonGroup("QualitivityProfessionalMainProjectRibbonGroup", "QualitivityProfessionalMainProjectRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 0)]
    internal class QualitivityProfessionalMainProjectRibbonGroup : AbstractRibbonGroup
    {
    }


    [Action("QualitivityProfessionalCreateQualitivityProject",
      typeof(ProjectsController),
      Name = "New Qualitivity Project",
      Description = "Create a new Qualitivity Project",
      Icon = "QualitivityCreateProjectAction_Icon")]
    [ActionLayout(typeof(QualitivityProfessionalMainProjectRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Large)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.Q)]
    public class QualitivityProfessionalCreateQualitivityProject : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewActivityProject;
        }
        protected override void Execute()
        {
            Controller.NewTimeTrackerProject();
        }
    }



    [Action("QualitivityProfessionalViewQualitivityProject",
      typeof(ProjectsController),
      Name = "View Projects",
      Description = "View Qualitivity Projects",
      Icon = "QualitivityApp_Icon")]
    [ActionLayout(typeof(QualitivityProfessionalMainProjectRibbonGroup), ZIndex =2, DisplayType = DisplayType.Normal)]
    public class QualitivityProfessionalViewQualitivityProject : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = true;
        }


        protected override void Execute()
        {
            Controller.ViewQualitivityProjects();
        }
    }


    [Action("QualitivityProfessionalCreateDQFProject",
        typeof(ProjectsController),
        Name = "New DQF Project",
        Description = "Create a new TAUS DQF Project",
        Icon = "QualitivityDQFController_Icon")]
    [ActionLayout(typeof(QualitivityProfessionalMainProjectRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D)]
    public class QualitivityProfessionalCreateDqfProject : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewDqfProject;
        }
        protected override void Execute()
        {
            Controller.CreateNewDqfProject();
        }
    }








    [Action("QualitivityContextCreateDQFProject", typeof(QualitivityViewController)
        , Name = "New DQF Project", Description = "Create a new TAUS DQF Project", Icon = "QualitivityDQFController_Icon")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 3, DisplayType.Default, "", true)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.D)]
    public class QualitivityContextCreateDqfProject : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewDqfProject;
        }
        protected override void Execute()
        {
            Controller.CreateNewDqfProject();
        }
    }

    [Action("QualitivityContextCreateQualitivityProject", typeof(QualitivityViewController)
        , Name = "New Qualitivity Project", Description = "Create a new Qualitivity Project", Icon = "QualitivityCreateProjectAction_Icon")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 3, DisplayType.Default, "", true)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.Q)]
    public class QualitivityContextCreateQualitivityProject : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewActivityProject;
        }
        protected override void Execute()
        {
            Controller.NewTimeTrackerProject();
        }
    }












    [Action("QualitivityRevisionQualityMetrics", typeof(EditorController), Icon = "QualitivityRevisionController_Icon", Name = "Add new Quality Metric")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation)
        , 1, DisplayType.Default, "Add new Quality Metric", true)]
    public class QualitivityRevisionQualityMetrics : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewQualityMetric;
        }
        protected override void Execute()
        {
            Controller.AddToRevisionMetrics();
        }
    }




    [RibbonGroup("QualitivityContactRibbonGroup", "QualitivityContactRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 0)]
    internal class QualitivityContactRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("QualitivityContactRibbonGroupAbout", typeof(QualitivityViewController), Name = "QualitivityContactRibbonGroupAbout_Name", Description = "QualitivityContactRibbonGroupAbout_Description", Icon = "QualitivityAbout_Icon")]
    [ActionLayout(typeof(QualitivityContactRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Large)]
    public class QualitivityContactAbout : AbstractViewControllerAction<QualitivityViewController>
    {
        protected override void Execute()
        {
            Controller.ViewAboutInfo();
        }
    }

    [Action("QualitivityContactRibbonGroupHelp", typeof(QualitivityViewController), Name = "QualitivityContactRibbonGroupHelp_Name", Description = "QualitivityContactRibbonGroupHelp_Description", Icon = "QualitivityHelp_Icon")]
    [ActionLayout(typeof(QualitivityContactRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class QualitivityContactHelp : AbstractViewControllerAction<QualitivityViewController>
    {
        protected override void Execute()
        {
            Controller.ViewOnlineHelp();
        }
    }






    [RibbonGroup("QualitivityTAUSDQFGroup", "TAUS DQF")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 1)]
    internal class QualitivityTausdqfGroup : AbstractRibbonGroup
    {
    }


    [Action("QualitivityTAUSDQFNewProject", typeof(QualitivityViewController), Name = "New DQF Project", Description = "Create a new DQF Project", Icon = "QualitivityDQFController_Icon")]
    [ActionLayout(typeof(QualitivityTausdqfGroup), ZIndex = 3, DisplayType = DisplayType.Large)]
    public class QualitivityTausdqfNewProject : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.DqfProjectEnabled;
        }
        protected override void Execute()
        {
            Controller.CreateNewDqfProject2();
        }
    }
    [Action("QualitivityTAUSDQFImportSettings", typeof(QualitivityViewController), Name = "Import Settings", Description = "Import DQF Project Settings", Icon = "QualitivityDQFImportProjectController_Icon")]
    [ActionLayout(typeof(QualitivityTausdqfGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class QualitivityTausdqfImportProjectSettings : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.DqfProjectImportEnabled;
        }
        protected override void Execute()
        {
            Controller.ImportDqfProjectSettings();
        }
    }
    [Action("QualitivityTAUSDQFSaveSettings", typeof(QualitivityViewController), Name = "Save Settings", Description = "Save DQF Project Settings", Icon = "QualitivityDQFSaveProjectController_Icon")]
    [ActionLayout(typeof(QualitivityTausdqfGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    public class QualitivityTausdqfSaveProjectSettings : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.DqfProjectSaveEnabled;
        }
        protected override void Execute()
        {
            Controller.SaveDqfProjectSettings();
        }
    }
    [Action("QualitivityTAUSDQFNewProjectTask", typeof(QualitivityViewController), Name = "New DQF Task", Description = "Create a new DQF Project Task", Icon = "QualitivityDQFNewProjectController_Icon")]
    [ActionLayout(typeof(QualitivityTausdqfGroup), ZIndex = 0, DisplayType = DisplayType.Normal)]
    public class QualitivityTausdqfNewProjectTask : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.DqfProjectTaskEnabled;
        }
        protected override void Execute()
        {
            Controller.AddNewDqfProjectTask();
        }
    }









    [RibbonGroup("QualitivityToolsRibbonGroup", "QualitivityToolsRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 2)]
    internal class QualitivityToolsRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("QualitivityToolsRibbonGroupCreateActivitiesReport", typeof(QualitivityViewController), Name = "QualitivityToolsRibbonGroupCreateActivitiesReport_Name", Description = "QualitivityToolsRibbonGroupCreateActivitiesReport_Description", Icon = "QualitivityToolsRibbonGroupCreateActivitiesReport_Icon")]
    [ActionLayout(typeof(QualitivityToolsRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Large)]
    public class QualitivityToolsRibbonGroupCreateActivitiesReport : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityCreateReportEnabled;
        }
        protected override void Execute()
        {
            Controller.CreateActivitiesReport();
        }
      
    }

    [Action("QualitivityToolsRibbonGroupExoprtActivitiesToExcel", typeof(QualitivityViewController), Name = "QualitivityToolsRibbonGroupExoprtActivitiesToExcel_Name", Description = "QualitivityToolsRibbonGroupExoprtActivitiesToExcel_Description", Icon = "QualitivityToolsRibbonGroupExoprtActivitiesToExcel_Icon")]
    [ActionLayout(typeof(QualitivityToolsRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Large)]
    public class QualitivityToolsRibbonGroupExoprtActivitiesToExcel : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityExportExcelEnabled;
        }
        protected override void Execute()
        {
            Controller.ExportActivitesToExcel();
        }
        
    }

















    [RibbonGroup("QualitivityTimeTrackingRibbonGroup", "QualitivityTimeTrackingRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 3)]
    internal class QualitivityTimeTrackingRibbonGroup : AbstractRibbonGroup
    {

    }


    [Action("QualitivityStartTimer", typeof(QualitivityViewController), Name = "QualitivityStartTimer_Name", Description = "QualitivityStartTimer_Description", Icon = "QualitivityStartTimer_Icon")]
    [ActionLayout(typeof(QualitivityTimeTrackingRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Large)]
    public class QualitivityStartTimer : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = QualitivityViewController.ProjectActivityStartTrackerEnabled;
        }
        protected override void Execute()
        {
            Controller.StartTimeTracking();
        }

    }


    [Action("QualitivityStopTimer", typeof(QualitivityViewController), Name = "QualitivityStopTimer_Name", Description = "QualitivityStopTimer_Description", Icon = "QualitivityStopTimer_Icon")]
    [ActionLayout(typeof(QualitivityTimeTrackingRibbonGroup), ZIndex = 0, DisplayType = DisplayType.Large)]
    public class QualitivityStopTimer : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = QualitivityViewController.ProjectActivityStopTrackerEnabled;
        }
        protected override void Execute()
        {
            Controller.StopTimeTracking();
        }

    }






    [RibbonGroup("QualitivityProjectTasksRibbonGroup", "QualitivityProjectTasksRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 4)]
    internal class QualitivityProjectTasksRibbonGroup : AbstractRibbonGroup
    {

    }


    [Action("QualitivityCreateProjectTaskAction", typeof(QualitivityViewController), Name = "QualitivityCreateProjectTaskAction_Name", Description = "QualitivityCreateProjectTaskAction_Description", Icon = "QualitivityCreateProjectTaskAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectTasksRibbonGroup), ZIndex = 5, DisplayType = DisplayType.Large)]
    public class QualitivityCreateProjectTaskAction : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityNewEnabled;
        }
        protected override void Execute()
        {
            Controller.NewProjectActivity();
        }
     
    }

    [Action("QualitivityEditProjectTaskAction", typeof(QualitivityViewController), Name = "QualitivityEditProjectTaskAction_Name", Description = "QualitivityEditProjectTaskAction_Description", Icon = "QualitivityEditProjectTaskAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectTasksRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Normal)]
    public class QualitivityEditProjectTaskAction : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityEditEnabled;
        }
        protected override void Execute()
        {
            Controller.EditProjectActivity();
        }
      
    }


    [Action("QualitivityRemoveProjectTaskAction", typeof(QualitivityViewController), Name = "QualitivityRemoveProjectTaskAction_Name", Description = "QualitivityRemoveProjectTaskAction_Description", Icon = "QualitivityRemoveProjectTaskAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectTasksRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class QualitivityRemoveProjectTaskAction : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityRemoveEnabled;
        }
        protected override void Execute()
        {
            Controller.RemoveProjectActivity();
        }
        
    }

    [Action("QualitivityMergeProjectTaskAction", typeof(QualitivityViewController), Name = "QualitivityMergeProjectTaskAction_Name", Description = "QualitivityMergeProjectTaskAction_Description", Icon = "QualitivityMergeProjectTaskAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectTasksRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class QualitivityMergeProjectTaskAction : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityMergeEnabled;
        }
        protected override void Execute()
        {
            Controller.MergeProjectActivities();
        }

    }


















    [RibbonGroup("QualitivityProjectsRibbonGroup", "QualitivityProjectsRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 5)]
    internal class QualitivityProjectsRibbonGroup : AbstractRibbonGroup
    {

    }


    [Action("QualitivityCreateProjectAction", typeof(QualitivityViewController), Name = "QualitivityCreateProjectAction_Name", Description = "QualitivityCreateProjectAction_Description", Icon = "QualitivityCreateProjectAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectsRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Large)]
    public class QualitivityCreateProject : AbstractViewControllerAction<QualitivityViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.TimeTrackerProjectNewEnabled;
        }
        protected override void Execute()
        {
            Controller.NewTimeTrackerProject();
        }

    }

    [Action("QualitivityEditProjectAction", typeof(QualitivityViewController), Name = "QualitivityEditProjectAction_Name", Description = "QualitivityEditProjectAction_Description", Icon = "QualitivityEditProjectAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectsRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class QualitivityEditProject : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.TimeTrackerProjectEditEnabled;
        }
        protected override void Execute()
        {
            Controller.EditTimeTrackerProject();
        }

    }


    [Action("QualitivityRemoveProjectAction", typeof(QualitivityViewController), Name = "QualitivityRemoveProjectAction_Name", Description = "QualitivityRemoveProjectAction_Description", Icon = "QualitivityRemoveProjectAction_Icon")]
    [ActionLayout(typeof(QualitivityProjectsRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class QualitivityRemoveProject : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.TimeTrackerProjectRemoveEnabled;
        }
        protected override void Execute()
        {
            Controller.RemoveTimeTrackerProject();
        }

    }








    [RibbonGroup("QualitivityConfigurationRibbonGroup", "QualitivityConfigurationRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 6)]
    internal class QualitivityConfigurationRibbonGroup : AbstractRibbonGroup
    {
    }




    [Action("QualitivityConfiguration", typeof(QualitivityViewController), Name = "QualitivityConfiguration_Name", Description = "QualitivityConfiguration_Description", Icon = "QualitivitySettings_Icon")]
    [ActionLayout(typeof(QualitivityConfigurationRibbonGroup), 4, DisplayType.Large)]
    public class QualitivityConfiguration : AbstractViewControllerAction<QualitivityViewController>
    {
        protected override void Execute()
        {
            Controller.LoadSettings();
        }
    }


    [Action("QualitivityConfigurationPEMRates", typeof(QualitivityViewController), Name = "QualitivityConfigurationPEMRates_Name", Description = "QualitivityConfigurationPEMRates_Description", Icon = "QualitivityConfigurationPEMRates_Icon")]
    [ActionLayout(typeof(QualitivityConfigurationRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class QualitivityConfigurationPEMRates : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = true;            
        }
    
        protected override void Execute()
        {
            Controller.LoadSettings(1);
        }

    }


    [Action("QualitivityConfigurationClients", typeof(QualitivityViewController), Name = "QualitivityConfigurationClients_Name", Description = "QualitivityConfigurationClients_Description", Icon = "QualitivityConfigurationClients_Icon")]
    [ActionLayout(typeof(QualitivityConfigurationRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class QualitivityConfigurationClients : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = true;
            
        }
      
        protected override void Execute()
        {
            Controller.LoadSettings(2);
        }

    }



    [Action("QualitivityConfigurationMyInfo", typeof(QualitivityViewController), Name = "QualitivityConfigurationMyInfo_Name", Description = "QualitivityConfigurationMyInfo_Description", Icon = "QualitivityConfigurationMyInfo_Icon")]
    [ActionLayout(typeof(QualitivityConfigurationRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    public class QualitivityConfigurationMyInfo : AbstractViewControllerAction<QualitivityViewController>
    {
        public override void Initialize()
        {
            Enabled = true;
        }

        protected override void Execute()
        {
            Controller.LoadSettings(3);
        }

    }
}
