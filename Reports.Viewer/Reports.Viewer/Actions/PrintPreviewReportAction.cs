using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_PrintPreviewReport_Action",
		Name = "ReportsViewer_PrintPreviewReport_Name",
		Description = "ReportsViewer_PrintPreviewReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "PrintPreview"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 4, DisplayType.Normal)]
	public class PrintPreviewReportAction : AbstractViewControllerAction<ReportsViewerController>
	{
		protected override void Execute()
		{
			MessageBox.Show("TODO");
		}

		public override void Initialize()
		{
			Enabled = false;
		}
	}
}
