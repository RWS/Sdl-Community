using System;
using System.Diagnostics;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.InSource
{
    [RibbonGroup("ConnectorViewRibbonGroup", "ConnectorViewRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]    
    class InSourceViewRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("CheckForProjectsAction", typeof(InSourceViewController), Name = "CheckForProjects_Name", Description = "CheckForProjects_Description", Icon = "CheckForProjects")]
    [ActionLayout(typeof(InSourceViewRibbonGroup), 2, DisplayType.Large)]
    public class CheckForProjectsAction : AbstractViewControllerAction<InSourceViewController>
    {
        protected override void Execute()
        {
            Controller.CheckForProjects();   
        }
    }

    [Action("CreateProjectsAction", typeof(InSourceViewController), Name = "CreateProjects_Name", Description = "CreateProjects_Description", Icon = "CreateProjects_Icon")]
    [ActionLayout(typeof(InSourceViewRibbonGroup), 1, DisplayType.Large)]
    public class CreateProjectsAction : AbstractViewControllerAction<InSourceViewController>
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

    [Action("ContributeToProjectAction", typeof(InSourceViewController), Name = "ContributeToProject_Name", Description = "ContributeToProject_Description", Icon = "opensourceimage")]
    [ActionLayout(typeof(InSourceViewRibbonGroup), 0, DisplayType.Large)]
    public class ContributeToProjectAction : AbstractViewControllerAction<InSourceViewController>
    {
        protected override void Execute()
        {
            Controller.Contribute();
        }
    }
	[Action("HelpAction", typeof(InSourceViewController), Name = "Help", Description = "An wiki page will be opened in browser uith user guide", Icon = "question")]
	[ActionLayout(typeof(InSourceViewRibbonGroup), 0, DisplayType.Large)]
	public class HelpAction : AbstractViewControllerAction<InSourceViewController>
	{
		protected override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3267.insource");
		}
	}

}
