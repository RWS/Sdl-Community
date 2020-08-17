using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Controls;
using Sdl.Community.Reports.Viewer.CustomEventArgs;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.Reports.Viewer.API;
using Sdl.Reports.Viewer.API.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;


namespace Sdl.Community.Reports.Viewer
{
	[View(
		Id = "SDLReportsViewer_View",
		Name = "SDLReportsViewer_Name",
		Description = "SDLReportsViewer_Description",
		Icon = "ReportsView",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class ReportsViewerController : AbstractViewController
	{
		private List<Report> _reports;
		private ReportViewModel _reportViewModel;
		private ReportsNavigationViewModel _reportsNavigationViewModel;
		private ReportViewControl _reportViewControl;
		private ReportsNavigationViewControl _reportsNavigationViewControl;
		private ReportView _reportView;
		private ReportsNavigationView _reportsNavigationView;
		private DataView _dataView;
		private BrowserView _browserView;
		private DataViewModel _dataViewModel;
		private BrowserViewModel _browserViewModel;
		private PathInfo _pathInfo;
		private ReportsController _controller;
		private string _clientId;

		protected override void Initialize(IViewContext context)
		{
			_clientId = Guid.NewGuid().ToString();

			_pathInfo = new PathInfo();
			_controller = ReportsController.Instance;
			_controller.ProjectChanged += Controller_ProjectChanged;
			_controller.ReportsAdded += Controller_ReportsAdded;
			_controller.ReportsRemoved += Controller_ReportsRemoved;
			_controller.ReportsUpdated += Controller_ReportsUpdated;

			_reports = _controller.GetReports();

			if (_reportsNavigationViewModel != null)
			{
				_reportsNavigationViewModel.Reports = _reports;
				_reportsNavigationViewModel.ProjectLocalFolder = _controller.GetProjectLocalFolder();
			}
		}
		
		protected override Control GetExplorerBarControl()
		{
			return _reportsNavigationViewControl ?? (_reportsNavigationViewControl = new ReportsNavigationViewControl());
		}

		protected override Control GetContentControl()
		{
			if (_reportViewControl == null)
			{
				_reportViewControl = new ReportViewControl();

				InitializeViews();
			}

			return _reportViewControl;
		}

		public EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		public List<Report> GetSelectedReports()
		{
			var selectedReport = _reportsNavigationViewModel?.SelectedReport;
			if (selectedReport != null)
			{
				return new List<Report> { selectedReport };
			}

			return _dataViewModel?.SelectedReports?.Cast<Report>().ToList();
		}

		public void AddReport(Report report)
		{
			report.IsSelected = true;

			var result = _controller.AddReports(_clientId, new List<Report> { report });
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			_reports.Add(result.Reports[0]);
			_reportsNavigationViewModel.Reports = _reports;
		}

		public void RemoveReports(List<string> reportIds)
		{
			var result = _controller.RemoveReports(_clientId, reportIds);
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			RemoveReportsInternal(result.Reports);
		}

		public void RefreshView()
		{
			_reportsNavigationViewModel.Refresh(GetSettings());
		}

		public IProject GetSelectedProject()
		{
			return _controller?.SelectedProject;
		}

		private void InitializeViews()
		{
			_browserViewModel = new BrowserViewModel();
			_browserView = new BrowserView
			{
				DataContext = _browserViewModel
			};

			_dataViewModel = new DataViewModel();
			_dataView = new DataView
			{
				DataContext = _dataViewModel
			};

			_reportViewModel = new ReportViewModel(_browserViewModel, _browserView, _dataViewModel, _dataView);
			_reportView = new ReportView
			{
				DataContext = _reportViewModel
			};

			_reports = _controller.GetReports();

			_reportsNavigationViewModel = new ReportsNavigationViewModel(_reports, GetSettings(), _pathInfo);
			_reportsNavigationViewModel.ReportSelectionChanged += OnReportSelectionChanged;
			_reportsNavigationViewModel.ReportViewModel = _reportViewModel;
			_reportsNavigationViewModel.ProjectLocalFolder = _controller.GetProjectLocalFolder();
			_reportsNavigationView = new ReportsNavigationView(_reportsNavigationViewModel);

			_reportViewControl.UpdateViewModel(_reportView);
			_reportsNavigationViewControl.UpdateViewModel(_reportsNavigationView);
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

		private IEnumerable<Report> GetReports(IEnumerable<string> reportIds)
		{
			var reports = new List<Report>();
			foreach (var reportId in reportIds)
			{
				var report = _reports.FirstOrDefault(a => a.Id == reportId);
				reports.Add(report);
			}

			return reports;
		}

		private void RemoveReportsInternal(IReadOnlyCollection<Report> reports)
		{
			foreach (var report in reports)
			{
				_reports.RemoveAll(a => a.Id == report.Id);
			}

			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var dataViewReports = _dataViewModel.Reports;
			foreach (var report in reports)
			{
				dataViewReports?.RemoveAll(a => a.Id == report.Id);
			}

			if (_reportsNavigationViewModel.IsReportSelected)
			{
				_browserViewModel.HtmlUri = null;
			}
			else
			{
				_dataViewModel.Reports = new List<Report>();
				_dataViewModel.Reports = dataViewReports;
			}

			_reportsNavigationViewModel.Reports = _reports;
		}

		private void Controller_ReportsRemoved(object sender, Sdl.Reports.Viewer.API.Events.ReportsRemovedEventArgs e)
		{
			if (e.ClientId != _clientId && e.Reports != null)
			{
				RemoveReportsInternal(e.Reports);
			}
		}

		private void Controller_ReportsAdded(object sender, Sdl.Reports.Viewer.API.Events.ReportsAddedEventArgs e)
		{
			if (e.ClientId != _clientId && e.Reports != null)
			{
				_reports.AddRange(e.Reports);
				if (_reportsNavigationViewModel != null)
				{
					_reportsNavigationViewModel.Reports = _reports;
				}
			}
		}

		private void Controller_ReportsUpdated(object sender, Sdl.Reports.Viewer.API.Events.ReportsUpdatedEventArgs e)
		{
			if (e.ClientId != _clientId)
			{
				MessageBox.Show("TODO: Controller_ReportsUpdated");
			}
		}

		private void Controller_ProjectChanged(object sender, Sdl.Reports.Viewer.API.Events.ProjectChangedEventArgs e)
		{
			_reports = e.Reports;

			if (_reportsNavigationViewModel != null)
			{
				_reportsNavigationViewModel.Reports = _reports;				
				_reportsNavigationViewModel.ProjectLocalFolder = _controller.GetProjectLocalFolder();
			}			
		}

		private void OnReportSelectionChanged(object sender, ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}
	}
}
