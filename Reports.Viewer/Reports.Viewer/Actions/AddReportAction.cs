﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.Service;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Reports.Viewer.API.Model;
using Sdl.Reports.Viewer.API.Services;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.Actions
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

		private void AddNewReport(Report report)
		{
			var reportTemplates = _reportsViewerController.ReportsController.GetCustomReportTemplates();
			var reports = _reportsViewerController.GetReports();
			var groupNames = reports.OrderByDescending(b => b.Group).Select(a => a.Group).Distinct().ToList();
			var taskTemplateIdProvider = new TaskTemplateIdProvider();
			
			var viewModel = new AppendReportViewModel(report, _imageService,
				_reportsViewerController.GetSelectedProject(), groupNames, reportTemplates, taskTemplateIdProvider);
			var view = new AppendReportWindow(viewModel, null);
			viewModel.Window = view;
			
			var result = view.ShowDialog();
			if (result != null && (bool)result)
			{
				_reportsViewerController.AddReports(new List<Report> {viewModel.Report});
			}
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

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		public override void Initialize()
		{
			_canEnable = true;
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();

			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
