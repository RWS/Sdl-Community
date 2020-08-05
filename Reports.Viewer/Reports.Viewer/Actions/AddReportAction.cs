using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_AddReport_Action",
		Name = "ReportsViewer_AddReport_Name",
		Description = "ReportsViewer_AddReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Add"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 9, DisplayType.Large)]
	public class AddReportAction : AbstractViewControllerAction<ReportsViewerController>
	{		
		protected override void Execute()
		{		
			MessageBox.Show("TODO");
		}

		public override void Initialize()
		{
			Enabled = true;
		}
	}
}
