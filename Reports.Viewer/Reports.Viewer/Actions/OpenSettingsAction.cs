using System.Windows;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
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
			var view = new SettingsWindow();
			var viewModel = new SettingsViewModel(view);
			view.DataContext = viewModel;
			view.ShowDialog();
		}

		public override void Initialize()
		{
			Enabled = true;
		}
	}
}
