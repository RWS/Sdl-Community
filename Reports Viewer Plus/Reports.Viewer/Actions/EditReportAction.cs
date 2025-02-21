using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Api.Providers;
using Reports.Viewer.Plus.Model;
using Reports.Viewer.Plus.Service;
using Reports.Viewer.Plus.View;
using Reports.Viewer.Plus.ViewModel;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.Actions
{
	[Action("ReportsViewer_EditReport_Action",
		Name = "ReportsViewer_EditReport_Name",
		Description = "ReportsViewer_EditReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Edit"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 8, DisplayType.Normal)]
	public class EditReportAction : BaseReportAction
	{
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private ReportsViewerController _reportsViewerController;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			var report = _reportsViewerController.GetSelectedReports().FirstOrDefault();
			if (report == null)
			{
				return;
			}

			var reportTemplates = _reportsViewerController.GetCustomReportTemplates();
			var reports = _reportsViewerController.GetReports();
			var groupNames = reports.OrderByDescending(b => b.Group).Select(a => a.Group).Distinct().ToList();
			var taskTemplateIdProvider = new TaskTemplateIdProvider();

			var viewModel = new AppendReportViewModel(report.Clone() as Report, _imageService,
				_reportsViewerController.GetSelectedProject(), groupNames, reportTemplates, taskTemplateIdProvider, true);
			var view = new AppendReportWindow(viewModel, null);
			viewModel.Window = view;

			var result = view.ShowDialog();
			if (result != null && (bool)result)
			{
				_reportsViewerController.UpdateReports(new List<Report> { viewModel.Report });
			}
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public override void Initialize()
		{
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
			_reportsViewerController.ReportSelectionChanged += ReportsViewerController_ReportSelectionChanged;

			SetEnabled();
		}

		public void Run()
		{
			Execute();
		}

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private void ReportsViewerController_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
		{
			_canEnable = e.SelectedReports?.Count == 1;
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
