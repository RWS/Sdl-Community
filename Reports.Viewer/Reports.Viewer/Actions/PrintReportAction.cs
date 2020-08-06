using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_PrintReport_Action",
		Name = "ReportsViewer_PrintReport_Name",
		Description = "ReportsViewer_PrintReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Print"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 5, DisplayType.Normal)]
	public class PrintReportAction : AbstractViewControllerAction<ReportsViewerController>
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
