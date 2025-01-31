using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using Newtonsoft.Json;
using Reports.Viewer.Api;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Api.Providers;
using Reports.Viewer.Plus.Actions;
using Reports.Viewer.Plus.CustomEventArgs;
using Reports.Viewer.Plus.Model;
using Reports.Viewer.Plus.View;
using Reports.Viewer.Plus.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Reports.Viewer.Plus
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
		private ProjectsController _projectsController;
		private ReportViewModel _reportViewModel;
		private ReportsNavigationViewModel _reportsNavigationViewModel;

		private ReportView _reportView;
		private ReportsNavigationView _reportsNavigationView;
		private DataView _dataView;
		private BrowserView _browserView;
		private BrowserViewModel _browserViewModel;
		private DataViewModel _dataViewModel;
		private PathInfo _pathInfo;
		private ReportsController _reportsController;
		private TaskTemplateIdProvider _taskTemplateIdProvider;

		private BaseReportAction _removeReportAction;
		private BaseReportAction _addReportAction;
		private BaseReportAction _editReportAction;
		private BaseReportAction _openSettingsAction;
		private BaseReportAction _printReportAction;
		private BaseReportAction _refreshAction;
		private BaseReportAction _saveAsReportAction;

		protected override void Initialize(IViewContext context)
		{
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			_pathInfo = new PathInfo();
			_taskTemplateIdProvider = new TaskTemplateIdProvider();
			_reportsController = new ReportsController(
				_projectsController.CurrentProject ?? _projectsController.SelectedProjects.FirstOrDefault(),
				_pathInfo, _taskTemplateIdProvider);

			_projectsController.CurrentProjectChanged += ProjectsControllerOnCurrentProjectChanged;

			_removeReportAction = SdlTradosStudio.Application.GetAction<RemoveReportAction>();
			_addReportAction = SdlTradosStudio.Application.GetAction<AddReportAction>();
			_editReportAction = SdlTradosStudio.Application.GetAction<EditReportAction>();
			_openSettingsAction = SdlTradosStudio.Application.GetAction<OpenSettingsAction>();
			_printReportAction = SdlTradosStudio.Application.GetAction<PrintReportAction>();
			_refreshAction = SdlTradosStudio.Application.GetAction<RefreshAction>();
			_saveAsReportAction = SdlTradosStudio.Application.GetAction<SaveAsReportAction>();

			ActivationChanged += ReportsViewerController_ActivationChanged;
		}

		protected override IUIControl GetExplorerBarControl()
		{
			return _reportsNavigationView ?? (_reportsNavigationView = new ReportsNavigationView());
		}

		protected override IUIControl GetContentControl()
		{
			if (_reportView == null)
			{
				InitializeViews();
			}

			return _reportView;
		}

		public event EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		internal List<Report> GetSelectedReports()
		{
			var selectedReport = _reportsNavigationViewModel?.SelectedReport;
			if (selectedReport != null)
			{
				return new List<Report> { selectedReport };
			}

			return _dataViewModel?.SelectedReports?.Cast<Report>().ToList();
		}

		internal List<Report> GetReports()
		{
			return _reportsNavigationViewModel?.Reports;
		}

		internal void AddReports(List<Report> reports)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var result = _reportsController.AddReports(reports);
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			_reportsNavigationViewModel.AddReports(result.Reports);
		}

		internal void UpdateReports(List<Report> reports)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var result = _reportsController.UpdateReports(reports);
			if (!result.Success)
			{
				MessageBox.Show(result.Message, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_reportsNavigationViewModel.UpdateReports(result.Reports);
		}

		internal void RemoveReports(List<Guid> reportIds)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var result = _reportsController.RemoveReports(reportIds);
			if (!result.Success)
			{
				MessageBox.Show(result.Message, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_reportsNavigationViewModel.DeleteReports(GetReports(result.Reports.Select(a => a.Id).ToList()));
		}

		internal void RefreshView(bool force = true)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			if (force)
			{
				EnableControls(true);
				var task = System.Threading.Tasks.Task.Run(() => _reportsController.GetReports(true));
				task.ContinueWith(t =>
				{
					_reportsNavigationViewModel.RefreshView(GetSettings(), t.Result);
					EnableControls(false);
				});
			}
			else
			{
				var reports = _reportsController.GetReports(false).Result;
				_reportsNavigationViewModel.RefreshView(GetSettings(), reports);
			}
		}

		internal List<ReportTemplate> GetCustomReportTemplates()
		{
			if (File.Exists(_pathInfo.CustomReportTemplatesFilePath))
			{
				var json = File.ReadAllText(_pathInfo.CustomReportTemplatesFilePath);
				return JsonConvert.DeserializeObject<List<ReportTemplate>>(json);
			}

			return new List<ReportTemplate>();
		}

		internal bool UpdateCustomReportTemplates(List<ReportTemplate> reportTemplates)
		{
			try
			{
				File.WriteAllText(_pathInfo.CustomReportTemplatesFilePath, JsonConvert.SerializeObject(reportTemplates));
				return true;
			}
			catch
			{
				// catch all; ignore
				return false;
			}
		}

		internal void UpdateSettings(bool updatedTemplates)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			RefreshView();
		}

		internal FileBasedProject GetSelectedProject()
		{
			return _reportsController?.Project;
		}

		internal void Print()
		{
			_reportViewModel?.Print();
		}

		internal string GetSelectedLanguage()
		{
			if (_reportsNavigationViewModel != null)
			{
				return _reportsNavigationViewModel.GetSelectedLanguage();
			}

			return string.Empty;
		}

		internal string GetSelectedGroup()
		{
			if (_reportsNavigationViewModel != null)
			{
				return _reportsNavigationViewModel.GetSelectedGroup();
			}

			return string.Empty;
		}

		internal void SaveReport()
		{
			_reportViewModel?.SaveReport();
		}

		private void InitializeViews()
		{
			_browserViewModel = new BrowserViewModel();
			_browserView = new BrowserView(_browserViewModel);
			_dataViewModel = new DataViewModel();
			_dataViewModel.ReportSelectionChanged += OnReportSelectionChanged;
			_dataView = new DataView
			{
				DataContext = _dataViewModel
			};

			_reportViewModel = new ReportViewModel(_browserViewModel, _browserView, _dataViewModel, _dataView, GetSelectedProject());
			_reportView = new ReportView
			{
				DataContext = _reportViewModel
			};

			var reports = _reportsController.GetReports(true).Result;

			_reportsNavigationViewModel = new ReportsNavigationViewModel(reports, GetSettings(), _pathInfo, GetSelectedProject());
			_reportsNavigationViewModel.ReportSelectionChanged += OnReportSelectionChanged;
			_reportsNavigationViewModel.ReportViewModel = _reportViewModel;
			_reportsNavigationViewModel.SelectedProject = _reportsController?.Project;
			_reportsNavigationViewModel.ProjectLocalFolder = _reportsController?.ProjectLocalFolder;
			_reportsNavigationView.DataContext = _reportsNavigationViewModel;
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

		private List<Report> GetReports(List<Guid> reportIds)
		{
			if (_reportsNavigationViewModel == null)
			{
				return null;
			}

			var reports = new List<Report>();
			foreach (var reportId in reportIds)
			{
				var report = _reportsNavigationViewModel.Reports.FirstOrDefault(a => a.Id == reportId);
				if (report == null)
				{
					continue;
				}
				reports.Add(report);
			}

			return reports;
		}

		private void EnableControls(bool isLoading)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var dispatcher = _reportsNavigationView.Dispatcher;

			if (_reportsNavigationView != null)
			{
				dispatcher.Invoke(delegate
				{
					if (isLoading)
					{
						_reportsNavigationViewModel.ClearReports();
					}

					_reportsNavigationViewModel.IsLoading = isLoading;
					_reportsNavigationView.IsEnabled = !isLoading;

					if (_reportView != null)
					{
						_reportView.IsEnabled = !isLoading;
					}

					_openSettingsAction.UpdateEnabled(isLoading);

					_addReportAction.UpdateEnabled(isLoading);
					_editReportAction.UpdateEnabled(isLoading);
					_removeReportAction.UpdateEnabled(isLoading);

					_printReportAction.UpdateEnabled(isLoading);
					_saveAsReportAction.UpdateEnabled(isLoading);

					_refreshAction.UpdateEnabled(isLoading);
				}, DispatcherPriority.Normal);
			}
		}

		private void ProjectsControllerOnCurrentProjectChanged(object sender, EventArgs e)
		{
			_reportsController = new ReportsController(_projectsController.CurrentProject, _pathInfo, _taskTemplateIdProvider);
			if (_reportsNavigationViewModel != null)
			{
				_reportsNavigationViewModel.SelectedProject = _reportsController.Project;
				_reportsNavigationViewModel.ProjectLocalFolder = _reportsController.ProjectLocalFolder;
				RefreshView();
			}
		}

		private void OnReportSelectionChanged(object sender, ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}

		private void ReportsViewerController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			if (e.Active && _reportsController != null && _reportsController.IsDirty)
			{
				RefreshView();
			}
		}
	}
}
