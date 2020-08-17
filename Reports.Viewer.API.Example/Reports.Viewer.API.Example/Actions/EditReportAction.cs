using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.API.Example.Actions
{
	[Action("ReportsViewerAPIExample_EditReport_Action",
		Name = "ReportsViewerAPIExample_EditReport_Name",
		Description = "ReportsViewerAPIExample_EditReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Edit"
	)]
	[ActionLayout(typeof(ReportsViewerAPIExampleActionsGroup), 8, DisplayType.Large)]
	public class EditReportAction : AbstractViewControllerAction<ReportsViewerController>
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
