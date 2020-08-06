using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Controls;
using Sdl.Community.Reports.Viewer.CustomEventArgs;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.Model.ProjectSettings;
using Sdl.Community.Reports.Viewer.TestData;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
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
		private ProjectsController _projectsController;
		private ReportView _reportView;
		private ReportsNavigationView _reportsNavigationView;
		private DataView _dataView;
		private BrowserView _browserView;
		private DataViewModel _dataViewModel;
		private BrowserViewModel _browserViewModel;
		private PathInfo _pathInfo;
		private bool _isLoaded;
		private string _loadedProjectId;

		protected override void Initialize(IViewContext context)
		{			
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;

			_pathInfo = new PathInfo();
			_reports = new List<Report>();

			SelectedProject = _projectsController.CurrentProject
							  ?? _projectsController.SelectedProjects.FirstOrDefault();

			_isLoaded = true;

			LoadProjectReports();
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

		public void AddReport(Report report)
		{
			report.IsSelected = true;
			_reports.Add(report);
			_reportsNavigationViewModel.Reports = _reports;

			// Update Project Settings
			var settingsBundle = SelectedProject.GetSettings();
			var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewer>();

			reportViewerProject.ReportsJson.Value = JsonConvert.SerializeObject(_reports);
			SelectedProject.UpdateSettings(reportViewerProject.SettingsBundle);
			SelectedProject.Save();
		}

		public List<Report> GetSelectedReports()
		{
			var selectedReport = _reportsNavigationViewModel?.SelectedReport;
			if (selectedReport != null)
			{
				return new List<Report> {selectedReport};
			}

			return _dataViewModel?.SelectedReports.Cast<Report>().ToList();
		}

		public void RemoveReports(List<Report> reports)
		{
			var dataViewReports = _dataViewModel.Reports;

			foreach (var report in reports)
			{
				dataViewReports?.Remove(report);
				_reports.Remove(report);
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

			var settingsBundle = SelectedProject.GetSettings();
			var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewer>();

			reportViewerProject.ReportsJson.Value = JsonConvert.SerializeObject(_reports);
			SelectedProject.UpdateSettings(reportViewerProject.SettingsBundle);
			SelectedProject.Save();			 
		}

		public void RefreshView()
		{
			_reportsNavigationViewModel.Refresh(GetSettings());
		}

		public FileBasedProject SelectedProject { get; private set; }

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

			_reportsNavigationViewModel = new ReportsNavigationViewModel(
				_reports, GetSettings(), _pathInfo, _projectsController);
			_reportsNavigationViewModel.ReportSelectionChanged += OnReportSelectionChanged;
			_reportsNavigationViewModel.ReportViewModel = _reportViewModel;
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

		private void OnReportSelectionChanged(object sender, ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			SelectedProject = _projectsController.CurrentProject
							  ?? _projectsController.SelectedProjects.FirstOrDefault();

			if (!_isLoaded)
			{
				return;
			}
			LoadProjectReports();
		}

		private void LoadProjectReports()
		{
			if (SelectedProject == null)
			{
				_reports = new List<Report>();
				if (_reportsNavigationViewModel != null)
				{
					_reportsNavigationViewModel.Reports = _reports;
				}

				return;
			}

			var projectInfo = SelectedProject.GetProjectInfo();
			var projectId = projectInfo.Id.ToString();

			if (projectId == _loadedProjectId)
			{
				return;
			}

			_loadedProjectId = projectId;

			var settingsBundle = SelectedProject.GetSettings();
			var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewer>();
			var reports = SerializeProjectFiles(reportViewerProject.ReportsJson.Value);
			_reports = reports;

			if (_reportsNavigationViewModel != null)
			{
				_reportsNavigationViewModel.Reports = _reports;
			}
		}

		private static List<Report> SerializeProjectFiles(string value)
		{
			try
			{
				var reports =
					JsonConvert.DeserializeObject<List<Report>>(value);
				return reports;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

	}
}
