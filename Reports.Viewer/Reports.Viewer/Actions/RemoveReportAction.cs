using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_RemoveReport_Action",
		Name = "ReportsViewer_RemoveReport_Name",
		Description = "ReportsViewer_RemoveReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Delete"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 7, DisplayType.Normal)]
	public class RemoveReportAction : AbstractViewControllerAction<ReportsViewerController>
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
