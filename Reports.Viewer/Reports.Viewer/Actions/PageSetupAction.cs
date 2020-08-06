using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_PageSetup_Action",
		Name = "ReportsViewer_PageSetup_Name",
		Description = "ReportsViewer_PageSetup_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "PageSetup"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 3, DisplayType.Normal)]
	public class PageSetupAction : AbstractViewControllerAction<ReportsViewerController>
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
