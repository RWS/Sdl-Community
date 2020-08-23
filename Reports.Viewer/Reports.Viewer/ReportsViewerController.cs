using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Actions;
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
		private ReportViewModel _reportViewModel;
		private ReportsNavigationViewModel _reportsNavigationViewModel;
		private ReportViewControl _reportViewControl;
		private ReportsNavigationViewControl _reportsNavigationViewControl;
		private ReportView _reportView;
		private ReportsNavigationView _reportsNavigationView;
		private DataView _dataView;
		private BrowserView _browserView;
		private DataViewModel _dataViewModel;
		private PathInfo _pathInfo;
		private ReportsController _controller;
		private string _clientId;
		private bool _isActive;

		private BaseReportAction _removeReportAction;
		private BaseReportAction _addReportAction;
		private BaseReportAction _editReportAction;
		private BaseReportAction _openSettingsAction;
		private BaseReportAction _pageSetupAction;
		private BaseReportAction _printPreviewReportAction;
		private BaseReportAction _printReportAction;
		private BaseReportAction _refreshAction;
		private BaseReportAction _saveAsReportAction;

		protected override void Initialize(IViewContext context)
		{
			_clientId = Guid.NewGuid().ToString();

			_removeReportAction = SdlTradosStudio.Application.GetAction<RemoveReportAction>();
			_addReportAction = SdlTradosStudio.Application.GetAction<AddReportAction>();
			_editReportAction = SdlTradosStudio.Application.GetAction<EditReportAction>();
			_openSettingsAction = SdlTradosStudio.Application.GetAction<OpenSettingsAction>();
			_pageSetupAction = SdlTradosStudio.Application.GetAction<PageSetupAction>();
			_printPreviewReportAction = SdlTradosStudio.Application.GetAction<PrintPreviewReportAction>();
			_printReportAction = SdlTradosStudio.Application.GetAction<PrintReportAction>();
			_refreshAction = SdlTradosStudio.Application.GetAction<RefreshAction>();
			_saveAsReportAction = SdlTradosStudio.Application.GetAction<SaveAsReportAction>();

			_pathInfo = new PathInfo();
			_controller = ReportsController.Instance;
			_controller.ProjectChanging += Controller_ProjectChanging;
			_controller.ProjectChanged += Controller_ProjectChanged;
			_controller.ReportsAdded += Controller_ReportsAdded;
			_controller.ReportsRemoved += Controller_ReportsRemoved;
			_controller.ReportsUpdated += Controller_ReportsUpdated;
			_controller.ProjectReportChanges += Controller_ProjectReportChanges;

			ActivationChanged += ReportsViewerController_ActivationChanged;
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


		public event EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

		public List<Report> GetSelectedReports()
		{
			var selectedReport = _reportsNavigationViewModel?.SelectedReport;
			if (selectedReport != null)
			{
				return new List<Report> { selectedReport };
			}

			return _dataViewModel?.SelectedReports?.Cast<Report>().ToList();
		}

		public void AddReports(List<Report> reports)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var result = _controller.AddReports(_clientId, reports);
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			_reportsNavigationViewModel.AddReports(result.Reports);
		}

		public void UpdateReports(List<Report> reports)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var result = _controller.UpdateReports(_clientId, reports);
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			_reportsNavigationViewModel.UpdateReports(result.Reports);
		}

		public void RemoveReports(List<string> reportIds)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			var result = _controller.RemoveReports(_clientId, reportIds);
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			_reportsNavigationViewModel.DeleteReorts(GetReports(result.Reports.Select(a => a.Id)));
		}

		public void RefreshView()
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			_reportsNavigationViewModel.RefreshView(GetSettings(), _controller.GetReports());
		}

		public void UpdateSettings()
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			_reportsNavigationViewModel.UpdateSettings(GetSettings());
		}

		public IProject GetSelectedProject()
		{
			return _controller?.SelectedProject;
		}

		private void InitializeViews()
		{
			_browserView = new BrowserView();
			_dataViewModel = new DataViewModel();
			_dataViewModel.ReportSelectionChanged += OnReportSelectionChanged;
			_dataView = new DataView
			{
				DataContext = _dataViewModel
			};

			_reportViewModel = new ReportViewModel(_browserView, _dataViewModel, _dataView);
			_reportView = new ReportView
			{
				DataContext = _reportViewModel
			};

			_reportsNavigationViewModel = new ReportsNavigationViewModel(_controller.GetReports(), GetSettings(), _pathInfo);
			_reportsNavigationViewModel.ReportSelectionChanged += OnReportSelectionChanged;
			_reportsNavigationViewModel.ReportViewModel = _reportViewModel;
			_reportsNavigationViewModel.ProjectLocalFolder = _controller.GetProjectLocalFolder();
			_reportsNavigationView = new ReportsNavigationView(_reportsNavigationViewModel);
			_reportsNavigationViewModel.ReportsNavigationView = _reportsNavigationView;

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

		public void Print()
		{
			_reportViewModel?.Print();
		}

		public string GetSelectedLanguage()
		{
			if (_reportsNavigationViewModel != null)
			{
				return _reportsNavigationViewModel.GetSelectedLanguage();
			}

			return string.Empty;
		}

		public string GetSelectedGroup()
		{
			if (_reportsNavigationViewModel != null)
			{
				return _reportsNavigationViewModel.GetSelectedGroup();
			}

			return string.Empty;
		}

		public void ShowPageSetupDialog()
		{
			_reportViewModel?.ShowPageSetupDialog();
		}

		public void ShowPrintPreviewDialog()
		{
			_reportViewModel?.ShowPrintPreviewDialog();
		}

		public void SaveReport()
		{
			_reportViewModel?.SaveReport();
		}

		private List<Report> GetReports(IEnumerable<string> reportIds)
		{
			if (_reportsNavigationViewModel == null)
			{
				return null;
			}

			var reports = new List<Report>();
			foreach (var reportId in reportIds)
			{
				var report = _reportsNavigationViewModel.Reports.FirstOrDefault(a => a.Id == reportId);
				reports.Add(report);
			}

			return reports;
		}

		private void Controller_ReportsRemoved(object sender, Sdl.Reports.Viewer.API.Events.ReportsRemovedEventArgs e)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			if (e.ClientId != _clientId && e.Reports != null)
			{
				_reportsNavigationViewModel.DeleteReorts(GetReports(e.Reports.Select(a => a.Id)));
			}
		}

		private void Controller_ReportsAdded(object sender, Sdl.Reports.Viewer.API.Events.ReportsAddedEventArgs e)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			if (e.ClientId != _clientId && e.Reports != null && e.Reports.Count > 0)
			{
				_reportsNavigationViewModel.AddReports(e.Reports);
			}
		}

		private void Controller_ReportsUpdated(object sender, Sdl.Reports.Viewer.API.Events.ReportsUpdatedEventArgs e)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			if (e.ClientId != _clientId && e.Reports != null)
			{
				foreach (var updatedReport in e.Reports)
				{
					var report = _reportsNavigationViewModel.Reports.FirstOrDefault(a => a.Id == updatedReport.Id);
					if (report == null)
					{
						return;
					}

					report.IsSelected = true;
					report.Path = updatedReport.Path;
					report.Name = updatedReport.Name;
					report.Description = updatedReport.Description;
					report.Language = updatedReport.Language;
					report.Group = updatedReport.Group;
				}
			}
		}

		private void Controller_ProjectChanged(object sender, Sdl.Reports.Viewer.API.Events.ProjectChangedEventArgs e)
		{
			if (_reportsNavigationViewModel != null)
			{
				_reportsNavigationViewModel.ProjectLocalFolder = _controller.GetProjectLocalFolder();
				_reportsNavigationViewModel.RefreshView(GetSettings(), e.Reports);

			}

			EnableControls(false);
		}

		private void Controller_ProjectChanging(object sender, Sdl.Reports.Viewer.API.Events.ProjectChangingEventArgs e)
		{
			EnableControls(true);
		}

		private void Controller_ProjectReportChanges(object sender, Sdl.Reports.Viewer.API.Events.ProjectReportChangesEventArgs e)
		{
			if (e.ClientId == _clientId || _reportsNavigationViewControl == null)
			{
				return;
			}

			if (e.AddedReports.Count > 0 || e.RemovedReports.Count > 0)
			{
				if (_isActive)
				{
					DisplayRefreshViewMessage(e.AddedReports, e.RemovedReports);
				}
				else
				{
					RefreshView();
				}
			}
		}

		private void EnableControls(bool isLoading)
		{
			if (_reportsNavigationViewModel == null)
			{
				return;
			}

			if (_reportsNavigationViewControl.InvokeRequired)
			{
				_reportsNavigationViewControl.Invoke(new Action<bool>(EnableControls), isLoading);
			}
			else
			{
				if (_reportsNavigationViewControl != null)
				{
					if (isLoading)
					{
						_reportsNavigationViewModel.RefreshView(GetSettings(), new List<Report>());
					}

					_reportsNavigationViewModel.IsLoading = isLoading;
					_reportsNavigationViewControl.Enabled = !isLoading;
				}

				if (_reportViewControl != null)
				{
					_reportViewControl.Enabled = !isLoading;
				}

				_openSettingsAction.UpdateEnabled(isLoading);

				_addReportAction.UpdateEnabled(isLoading);
				_editReportAction.UpdateEnabled(isLoading);
				_removeReportAction.UpdateEnabled(isLoading);

				_printReportAction.UpdateEnabled(isLoading);
				_pageSetupAction.UpdateEnabled(isLoading);
				_printPreviewReportAction.UpdateEnabled(isLoading);
				_saveAsReportAction.UpdateEnabled(isLoading);

				_refreshAction.UpdateEnabled(isLoading);
			}
		}

		private void OnReportSelectionChanged(object sender, ReportSelectionChangedEventArgs e)
		{
			ReportSelectionChanged?.Invoke(this, e);
		}

		private void ReportsViewerController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			_isActive = e.Active;

			if (_reportsNavigationViewControl == null)
			{
				return;
			}

			if (e.Active)
			{
				var task = System.Threading.Tasks.Task.Run(() => _controller.GetStudioReportUpdates(_clientId));

				task.ContinueWith(t =>
				{
					if (t.Result.AddedReports.Count > 0 || t.Result.RemovedReports.Count > 0)
					{
						DisplayRefreshViewMessage(t.Result.AddedReports, t.Result.RemovedReports);
					}
				});
			}
		}

		private void DisplayRefreshViewMessage(List<Report> addedRecords, List<Report> removedRecords)
		{
			var message = "Studio has applied changes in the Reports view."
						  + Environment.NewLine + Environment.NewLine
						  + string.Format("Added Reports: {0}", addedRecords.Count) + Environment.NewLine
						  + string.Format("Removed Reports: {0}", removedRecords.Count)
						  + Environment.NewLine + Environment.NewLine
						  + "Click on 'Yes' to refresh the view";
			var dialogResult = MessageBox.Show(message, "Reports Viewer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (dialogResult == DialogResult.Yes)
			{
				RefreshView();
			}
		}
	}
}
