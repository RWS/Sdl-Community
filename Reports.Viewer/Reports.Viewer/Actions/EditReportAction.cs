using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_EditReport_Action",
		Name = "ReportsViewer_EditReport_Name",
		Description = "ReportsViewer_EditReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Edit"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 8, DisplayType.Normal)]
	public class EditReportAction : AbstractViewControllerAction<ReportsViewerController>
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
