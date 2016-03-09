using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace StudioIntegrationApiSample
{
    [RibbonGroup("ConnectorViewRibbonGroup", "ConnectorViewRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]    
    class ContentConnectorViewRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("CheckForProjectsAction", typeof(ContentConnectorViewController), Name = "CheckForProjects_Name", Description = "CheckForProjects_Description", Icon = "CheckForProjects_Icon")]
    [ActionLayout(typeof(ContentConnectorViewRibbonGroup), 2, DisplayType.Large)]
    public class CheckForProjectsAction : AbstractViewControllerAction<ContentConnectorViewController>
    {
        protected override void Execute()
        {
            Controller.CheckForProjects();   
        }
    }

    [Action("CreateProjectsAction", typeof(ContentConnectorViewController), Name = "CreateProjects_Name", Description = "CreateProjects_Description", Icon = "CreateProjects_Icon")]
    [ActionLayout(typeof(ContentConnectorViewRibbonGroup), 1, DisplayType.Large)]
    public class CreateProjectsAction : AbstractViewControllerAction<ContentConnectorViewController>
    {
        public override void Initialize()
        {
            Controller.ProjectRequestsChanged += OnProjectRequestsChanged;
        }

        void OnProjectRequestsChanged(object sender, EventArgs e)
        {
            Enabled = Controller.ProjectRequests.Count > 0;
        }

        protected override void Execute()
        {
            Controller.CreateProjects();
        }
    }

}
