using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Main
{


    [RibbonGroup("StudioTimeTrackerContactRibbonGroup", "StudioTimeTrackerContactRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 0)]
    internal class StudioTimeTrackerContactRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("StudioTimeTrackerContactRibbonGroupAbout", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerContactRibbonGroupAbout_Name", Description = "StudioTimeTrackerContactRibbonGroupAbout_Description", Icon = "StudioTimeTrackerAbout_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerContactRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackerContactAbout : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {
        protected override void Execute()
        {
            Controller.ViewAboutInfo();
        }
    }

    [Action("StudioTimeTrackerContactRibbonGroupHelp", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerContactRibbonGroupHelp_Name", Description = "StudioTimeTrackerContactRibbonGroupHelp_Description", Icon = "StudioTimeTrackerHelp_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerContactRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerContactHelp : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {
        protected override void Execute()
        {
            Controller.ViewOnlineHelp();
        }
    }








    [RibbonGroup("StudioTimeTrackerToolsRibbonGroup", "StudioTimeTrackerToolsRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 1)]
    internal class StudioTimeTrackerToolsRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport_Name", Description = "StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport_Description", Icon = "StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerToolsRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport : AbstractViewControllerAction<StudioTimeTrackerViewController>
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
            //Controller.CreateActivitiesReport();
        }
      
    }

    [Action("StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel_Name", Description = "StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel_Description", Icon = "StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerToolsRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel : AbstractViewControllerAction<StudioTimeTrackerViewController>
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





    [RibbonGroup("StudioTimeTrackerTimeTrackingRibbonGroup", "StudioTimeTrackerTimeTrackingRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 2)]
    internal class StudioTimeTrackerTimeTrackingRibbonGroup : AbstractRibbonGroup
    {

    }


    [Action("StudioTimeTrackeStartTimer", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeStartTimer_Name", Description = "StudioTimeTrackeStartTimer_Description", Icon = "StudioTimeTrackeStartTimer_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerTimeTrackingRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackeStartTimer : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityStartTrackerEnabled;
        }
        protected override void Execute()
        {
            Controller.StartTimeTracking();
        }

    }


    [Action("StudioTimeTrackeStopTimer", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeStopTimer_Name", Description = "StudioTimeTrackeStopTimer_Description", Icon = "StudioTimeTrackeStopTimer_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerTimeTrackingRibbonGroup), ZIndex = 0, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackeStopTimer : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {

        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectActivityStopTrackerEnabled;
        }
        protected override void Execute()
        {
            Controller.StopTimeTracking();
        }

    }






    [RibbonGroup("StudioTimeTrackerProjectTasksRibbonGroup", "StudioTimeTrackerProjectTasksRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 3)]
    internal class StudioTimeTrackerProjectTasksRibbonGroup : AbstractRibbonGroup
    {

    }


    [Action("StudioTimeTrackeCreateProjectTaskAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeCreateProjectTaskAction_Name", Description = "StudioTimeTrackeCreateProjectTaskAction_Description", Icon = "StudioTimeTrackeCreateProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 5, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackeCreateProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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

    [Action("StudioTimeTrackeEditProjectTaskAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeEditProjectTaskAction_Name", Description = "StudioTimeTrackeEditProjectTaskAction_Description", Icon = "StudioTimeTrackeEditProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackeEditProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackeRemoveProjectTaskAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeRemoveProjectTaskAction_Name", Description = "StudioTimeTrackeRemoveProjectTaskAction_Description", Icon = "StudioTimeTrackeRemoveProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackeRemoveProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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

    [Action("StudioTimeTrackeMergeProjectTaskAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeMergeProjectTaskAction_Name", Description = "StudioTimeTrackeMergeProjectTaskAction_Description", Icon = "StudioTimeTrackeMergeProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackeMergeProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


  




    [RibbonGroup("StudioTimeTrackerProjectsRibbonGroup", "StudioTimeTrackerProjectsRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 4)]
    internal class StudioTimeTrackerProjectsRibbonGroup : AbstractRibbonGroup
    {

    }


    [Action("StudioTimeTrackeCreateProjectAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeCreateProjectAction_Name", Description = "StudioTimeTrackeCreateProjectAction_Description", Icon = "StudioTimeTrackeCreateProjectAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectsRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Large)]
    public class CreateProject : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {

        public override void Initialize()
        {
            Enabled = true;
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

    [Action("StudioTimeTrackeEditProjectAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeEditProjectAction_Name", Description = "StudioTimeTrackeEditProjectAction_Description", Icon = "StudioTimeTrackeEditProjectAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectsRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class EditProject : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackeRemoveProjectAction", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackeRemoveProjectAction_Name", Description = "StudioTimeTrackeRemoveProjectAction_Description", Icon = "StudioTimeTrackeRemoveProjectAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectsRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class RemoveProject : AbstractViewControllerAction<StudioTimeTrackerViewController>
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








    [RibbonGroup("StudioTimeTrackerConfigurationRibbonGroup", "StudioTimeTrackerConfigurationRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 5)]
    internal class StudioTimeTrackerConfigurationRibbonGroup : AbstractRibbonGroup
    {
    }




    [Action("StudioTimeTrackerConfiguration", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerConfiguration_Name", Description = "StudioTimeTrackerConfiguration_Description", Icon = "StudioTimeTrackerSettings_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerConfigurationRibbonGroup), 4, DisplayType.Large)]
    public class StudioTimeTrackerConfiguration : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {
        protected override void Execute()
        {
            Controller.LoadSettings();
        }
    }


    [Action("StudioTimeTrackerConfigurationActivityTypes", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerConfigurationActivityTypes_Name", Description = "StudioTimeTrackerConfigurationActivityTypes_Description", Icon = "StudioTimeTrackerConfigurationActivityTypes_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerConfigurationRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerConfigurationActivityTypes : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackerConfigurationClientRates", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerConfigurationClientRates_Name", Description = "StudioTimeTrackerConfigurationClientRates_Description", Icon = "StudioTimeTrackerConfigurationClientRates_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerConfigurationRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerConfigurationClientRates : AbstractViewControllerAction<StudioTimeTrackerViewController>
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



    [Action("StudioTimeTrackerConfigurationMyInfo", typeof(StudioTimeTrackerViewController), Name = "StudioTimeTrackerConfigurationMyInfo_Name", Description = "StudioTimeTrackerConfigurationMyInfo_Description", Icon = "StudioTimeTrackerConfigurationMyInfo_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerConfigurationRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerConfigurationMyInfo : AbstractViewControllerAction<StudioTimeTrackerViewController>
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
