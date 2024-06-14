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

    [RibbonGroup("StudioTimeTrackerToolsRibbonGroup", "StudioTimeTrackerToolsRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 1)]
    internal class StudioTimeTrackerToolsRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport_Name", 
        Description = "StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport_Description", 
        Icon = "StudioTimeTrackerToolsRibbonGroupCreateActivitiesReport_Icon")]
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

    [Action("StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel_Name",
        Description = "StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel_Description", 
        Icon = "StudioTimeTrackerToolsRibbonGroupExoprtActivitiesToExcel_Icon")]
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


    [Action("StudioTimeTrackerStartTimer", typeof(StudioTimeTrackerViewController),
        Name = "StudioTimeTrackerStartTimer_Name", 
        Description = "StudioTimeTrackerStartTimer_Description", 
        Icon = "StudioTimeTrackerStartTimer_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerTimeTrackingRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackerStartTimer : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackerStopTimer", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerStopTimer_Name", 
        Description = "StudioTimeTrackerStopTimer_Description",
        Icon = "StudioTimeTrackerStopTimer_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerTimeTrackingRibbonGroup), ZIndex = 0, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackerStopTimer : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackerCreateProjectTaskAction", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerCreateProjectTaskAction_Name", 
        Description = "StudioTimeTrackerCreateProjectTaskAction_Description", 
        Icon = "StudioTimeTrackerCreateProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 5, DisplayType = DisplayType.Large)]
    public class StudioTimeTrackerCreateProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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

    [Action("StudioTimeTrackerEditProjectTaskAction", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerEditProjectTaskAction_Name", 
        Description = "StudioTimeTrackerEditProjectTaskAction_Description", 
        Icon = "StudioTimeTrackerEditProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerEditProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackerRemoveProjectTaskAction", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerRemoveProjectTaskAction_Name", 
        Description = "StudioTimeTrackerRemoveProjectTaskAction_Description", 
        Icon = "StudioTimeTrackerRemoveProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerRemoveProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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

    [Action("StudioTimeTrackerMergeProjectTaskAction", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerMergeProjectTaskAction_Name", 
        Description = "StudioTimeTrackerMergeProjectTaskAction_Description", 
        Icon = "StudioTimeTrackerMergeProjectTaskAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectTasksRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerMergeProjectTaskAction : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackerCreateProjectAction", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerCreateProjectAction_Name", 
        Description = "StudioTimeTrackerCreateProjectAction_Description", 
        Icon = "StudioTimeTrackerCreateProjectAction_Icon")]
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

    [Action("StudioTimeTrackerEditProjectAction", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerEditProjectAction_Name",
        Description = "StudioTimeTrackerEditProjectAction_Description", 
        Icon = "StudioTimeTrackerEditProjectAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectsRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerEditProject : AbstractViewControllerAction<StudioTimeTrackerViewController>
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


    [Action("StudioTimeTrackerRemoveProjectAction", typeof(StudioTimeTrackerViewController),
        Name = "StudioTimeTrackerRemoveProjectAction_Name", 
        Description = "StudioTimeTrackerRemoveProjectAction_Description",
        Icon = "StudioTimeTrackerRemoveProjectAction_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerProjectsRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class StudioTimeTrackerRemoveProject : AbstractViewControllerAction<StudioTimeTrackerViewController>
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




    [Action("StudioTimeTrackerConfiguration", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerConfiguration_Name", 
        Description = "StudioTimeTrackerConfiguration_Description", 
        Icon = "StudioTimeTrackerSettings_Icon")]
    [ActionLayout(typeof(StudioTimeTrackerConfigurationRibbonGroup), 4, DisplayType.Large)]
    public class StudioTimeTrackerConfiguration : AbstractViewControllerAction<StudioTimeTrackerViewController>
    {
        protected override void Execute()
        {
            Controller.LoadSettings();
        }
    }


    [Action("StudioTimeTrackerConfigurationActivityTypes", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerConfigurationActivityTypes_Name", 
        Description = "StudioTimeTrackerConfigurationActivityTypes_Description", 
        Icon = "StudioTimeTrackerConfigurationActivityTypes_Icon")]
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


    [Action("StudioTimeTrackerConfigurationClientRates", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerConfigurationClientRates_Name", 
        Description = "StudioTimeTrackerConfigurationClientRates_Description", 
        Icon = "StudioTimeTrackerConfigurationClientRates_Icon")]
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


    [Action("StudioTimeTrackerConfigurationMyInfo", typeof(StudioTimeTrackerViewController), 
        Name = "StudioTimeTrackerConfigurationMyInfo_Name", 
        Description = "StudioTimeTrackerConfigurationMyInfo_Description", 
        Icon = "StudioTimeTrackerConfigurationMyInfo_Icon")]
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
