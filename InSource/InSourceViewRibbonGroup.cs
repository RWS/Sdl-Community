using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
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
    public class CreateProjectsAction : AbstractViewControllerAction<InSourceViewController>, IDisposable
    {
		private readonly SafeHandle _safeHandle;
		public CreateProjectsAction()
		{
			_safeHandle = new SafeFileHandle(IntPtr.Zero, true);
		}

		// The public implementation of Dispose pattern callable by consumers.
		public void Dispose() => Dispose(true);

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

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			// Dispose managed state (managed objects).
			_safeHandle?.Dispose();
			if (Controller != null)
			{
				Controller.ProjectRequestsChanged -= OnProjectRequestsChanged;
				Controller.Dispose();
			}
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
	[Action("InSourceHelpAction", typeof(InSourceViewController), Name = "Help", Description = "An wiki page will be opened in browser uith user guide", Icon = "question")]
	[ActionLayout(typeof(InSourceViewRibbonGroup), 0, DisplayType.Large)]
	public class HelpAction : AbstractViewControllerAction<InSourceViewController>
	{
		protected override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3267.trados-insource");
		}
	}
}
