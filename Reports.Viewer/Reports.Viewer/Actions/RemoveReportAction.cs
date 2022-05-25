using System;
using System.Linq;
using System.Windows;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.Actions
{
	[Action("ReportsViewer_RemoveReport_Action",
		Name = "ReportsViewer_RemoveReport_Name",
		Description = "ReportsViewer_RemoveReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Delete"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 7, DisplayType.Normal)]
	public class RemoveReportAction : BaseReportAction
	{
		private ReportsViewerController _reportsViewerController;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			var reports = _reportsViewerController.GetSelectedReports();

			var message1 = "This action will remove the selected reports from the project";
			var message2 = "Do you want to proceed?";

			var response = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
				PluginResources.Plugin_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (response == MessageBoxResult.No)
			{
				return;
			}

			var removeIds = reports.Select(a => a.Id).ToList();
			_reportsViewerController.RemoveReports(removeIds);
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public void Run()
		{
			Execute();
		}

		public override void Initialize()
		{
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
			_reportsViewerController.ReportSelectionChanged += ReportsViewerController_ReportSelectionChanged;

			SetEnabled();
		}

		private void ReportsViewerController_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
		{
			_canEnable = e.SelectedReports?.Count > 0;
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
