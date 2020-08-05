using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_OpenSettings_Action",
		Name = "ReportsViewer_OpenSettings_Name",
		Description = "ReportsViewer_OpenSettings_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Settings"
	)]
	[ActionLayout(typeof(ReportsViewerSettingsGroups), 10, DisplayType.Large)]
	public class OpenSettingsAction : AbstractViewControllerAction<ReportsViewerController>
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
