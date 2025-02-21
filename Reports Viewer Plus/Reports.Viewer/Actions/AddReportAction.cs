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
	[Action("ReportsViewer_AddReport_Action",
		Name = "ReportsViewer_AddReport_Name",
		Description = "ReportsViewer_AddReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Add"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 9, DisplayType.Large)]
	public class AddReportAction : BaseReportAction
	{
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private ReportsViewerController _reportsViewerController;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			Run(new Report());
		}

		public override void Initialize()
		{
			_canEnable = true;
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();

			SetEnabled();
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public void Run(Report report)
		{
			report.Language = _reportsViewerController.GetSelectedLanguage();
			report.Group = _reportsViewerController.GetSelectedGroup();
			AddNewReport(report);
		}

		private void AddNewReport(Report report)
		{
			var selectedProject = _reportsViewerController.GetSelectedProject();
			if (selectedProject == null)
			{
				return;
			}

			var reportTemplates = GetCustomReportTemplates();

			var reports = _reportsViewerController.GetReports();
			var groupNames = reports.OrderByDescending(b => b.Group).Select(a => a.Group).Distinct().ToList();
			var taskTemplateIdProvider = new TaskTemplateIdProvider();

			var viewModel = new AppendReportViewModel(report, _imageService, selectedProject, groupNames, reportTemplates, taskTemplateIdProvider);
			var view = new AppendReportWindow(viewModel, null);
			viewModel.Window = view;

			var result = view.ShowDialog();
			if (result != null && (bool)result)
			{
				_reportsViewerController.AddReports(new List<Report> { viewModel.Report });
			}
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

		private List<ReportTemplate> GetCustomReportTemplates()
		{
			if (File.Exists(_pathInfo.CustomReportTemplatesFilePath))
			{
				var json = File.ReadAllText(_pathInfo.CustomReportTemplatesFilePath);
				return JsonConvert.DeserializeObject<List<ReportTemplate>>(json);
			}

			return new List<ReportTemplate>();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
