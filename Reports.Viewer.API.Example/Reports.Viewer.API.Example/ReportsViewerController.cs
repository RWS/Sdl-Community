using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Reports.Viewer.API.Example.Controls;
using Sdl.Community.Reports.Viewer.API.Example.View;
using Sdl.Community.Reports.Viewer.API.Example.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.Reports.Viewer.API;
using Sdl.Reports.Viewer.API.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Reports.Viewer.API.Example
{
	[View(
		Id = "ReportsViewerAPIExample_View",
		Name = "ReportsViewerAPIExample_Name",
		Description = "ReportsViewerAPIExample_Description",
		Icon = "ReportsView",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class ReportsViewerController : AbstractViewController
	{
		private ReportViewControl _reportViewControl;
		private List<Report> _reports;
		private ReportsController _controller;
		private DataViewModel _dataViewModel;
		private DataView _dataView;
		private string _clientId;
		private bool _isActive;

		protected override void Initialize(IViewContext context)
		{
			_clientId = Guid.NewGuid().ToString();

			_controller = ReportsController.Instance;
			_controller.ProjectChanged += Controller_ProjectChanged;
			_controller.ReportsAdded += Controller_ReportsAdded;
			_controller.ReportsRemoved += Controller_ReportsRemoved;
			_controller.ReportsUpdated += Controller_ReportsUpdated;

			ActivationChanged += ReportsViewerController_ActivationChanged;			
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

		public void AddReport(Report report)
		{
			if (_reports == null)
			{
				return;
			}

			report.IsSelected = true;

			var result = _controller.AddReports(_clientId, new List<Report> { report });
			if (!result.Success)
			{
				MessageBox.Show(result.Message, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_reports.Add(result.Reports[0]);
			_dataViewModel.Reports = new List<Report>(_reports);

		}

		public void RemoveReports(List<Report> reports)
		{
			if (reports == null)
			{
				return;
			}

			var result = _controller.RemoveReports(_clientId, reports.Select(a => a.Id).ToList());
			if (!result.Success)
			{
				MessageBox.Show(result.Message, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			RemoveReportsInternal(result.Reports);
		}

		public void UpdateReports(List<Report> reports)
		{
			if (reports == null)
			{
				return;
			}

			var result = _controller.UpdateReports(_clientId, reports);
			if (!result.Success)
			{
				MessageBox.Show(result.Message);
				return;
			}

			UpdateReportsInternal(result.Reports);
		}
	
		public IProject GetSelectedProject()
		{
			return _controller?.SelectedProject;
		}

		public List<Report> GetSelectedReports()
		{
			return _dataViewModel?.SelectedReports?.Cast<Report>().ToList();
		}

		public void RefreshView()
		{
			_reports = _controller.GetReports(false).Result;
			if (_dataViewModel != null)
			{
				_dataViewModel.Reports = new List<Report>(_reports);
			}						
		}

		private void InitializeViews()
		{
			_reports = _controller.GetReports(false).Result;

			_dataViewModel = new DataViewModel(_reports)
			{
				ProjectLocalFolder = _controller.GetProjectLocalFolder()
			};

			_dataView = new DataView
			{
				DataContext = _dataViewModel
			};

			
			_reportViewControl?.UpdateViewModel(_dataView);
		}

		private void Controller_ReportsUpdated(object sender, Sdl.Reports.Viewer.API.Events.ReportsUpdatedEventArgs e)
		{
			if (e.ClientId != _clientId && e.Reports != null)
			{
				UpdateReportsInternal(e.Reports);
			}
		}

		private void Controller_ReportsRemoved(object sender, Sdl.Reports.Viewer.API.Events.ReportsRemovedEventArgs e)
		{
			if (e.ClientId != _clientId && e.Reports != null)
			{
				if (_dataViewModel != null)
				{
					RemoveReportsInternal(e.Reports);
				}
			}
		}

		private void Controller_ReportsAdded(object sender, Sdl.Reports.Viewer.API.Events.ReportsAddedEventArgs e)
		{
			if (e.ClientId != _clientId && e.Reports != null)
			{
				_reports.AddRange(e.Reports);
				if (_dataViewModel != null)
				{
					_dataViewModel.Reports = new List<Report>(_reports);
				}
			}
		}

		private void Controller_ProjectChanged(object sender, Sdl.Reports.Viewer.API.Events.ProjectChangedEventArgs e)
		{
			_reports = e.Reports.ToList();

			if (_dataViewModel != null)
			{
				_dataViewModel.ProjectLocalFolder = _controller.GetProjectLocalFolder();
				_dataViewModel.Reports = _reports;
			}
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

		private void UpdateReportsInternal(IEnumerable<Report> reports)
		{
			if (reports == null)
			{
				return;
			}
			foreach (var updatedReport in reports)
			{
				var report = _reports.FirstOrDefault(a => a.Id == updatedReport.Id);
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

			if (_dataViewModel != null)
			{				
				_dataViewModel.Reports = new List<Report>(_reports);
			}
		}

		private void RemoveReportsInternal(IEnumerable<Report> reports)
		{
			if (reports == null)
			{
				return;
			}

			foreach (var report in reports)
			{
				_reports.RemoveAll(a => a.Id == report.Id);
			}

			if (_dataViewModel != null)
			{				
				_dataViewModel.Reports = new List<Report>(_reports);
			}
		}

		private void ReportsViewerController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			_isActive = e.Active;

			if (_dataViewModel == null)
			{
				return;
			}

			if (e.Active)
			{
				var task = System.Threading.Tasks.Task.Run(() => _controller.GetStudioReportChanges());

				task.ContinueWith(t =>
				{
					if (t.Result.AddedReports.Count > 0 || t.Result.RemovedReports.Count > 0)
					{
						DisplayRefreshViewMessage(t.Result.AddedReports, t.Result.RemovedReports);
					}
				});
			}
		}

		private void DisplayRefreshViewMessage(IReadOnlyCollection<Report> addedRecords, IReadOnlyCollection<Report> removedRecords)
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
