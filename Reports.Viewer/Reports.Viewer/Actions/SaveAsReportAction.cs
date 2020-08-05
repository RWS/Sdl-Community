using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_SaveAsReport_Action",
		Name = "ReportsViewer_SaveAsReport_Name",
		Description = "ReportsViewer_SaveAsReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Save"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 6, DisplayType.Normal)]
	public class SaveAsReportAction : AbstractViewControllerAction<ReportsViewerController>
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
