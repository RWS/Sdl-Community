using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.Reports.Viewer.API.Example.Controls;
using Sdl.Community.Reports.Viewer.API.Example.View;
using Sdl.Community.Reports.Viewer.API.Example.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Models;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using Sdl.TranslationStudioAutomation.IntegrationApi;
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
		private ProjectsController _projectsController;
		private DataViewModel _dataViewModel;
		private DataView _dataView;

		protected override void Initialize(IViewContext context)
		{
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.CurrentProjectChanged += _projectsController_CurrentProjectChanged;
		}

		private void _projectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			RefreshView();
		}

		protected override IUIControl GetContentControl()
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
			var fileBasedProject = GetSelectedProject();
			if (fileBasedProject != null)
			{
				string reportData = File.ReadAllText(report.Path);
				var addedReport = new ProjectReportsOperations(fileBasedProject)
					.AddReport(report.TemplateId, report.Name, report.Description, report.Language, reportData);
				RefreshView();
				if (addedReport != null)
				{
					MessageBox.Show($"The report {addedReport.Name} was added", PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}	

		public void RemoveReports(List<Report> reports)
		{
			if (reports == null)
			{
				return;
			}
			var fileBasedProject = GetSelectedProject();
			if (fileBasedProject != null)
			{
				new ProjectReportsOperations(fileBasedProject).RemoveReports(reports.Select(r => r.Id).ToList());
				RefreshView();
			}
		}

		public void UpdateReport(Report report)
		{
			if (report == null)
			{
				return;
			}

			var fileBasedProject = GetSelectedProject();
			if (fileBasedProject != null)
			{
				new ProjectReportsOperations(fileBasedProject).UpdateReport(report.Id, report.Name, report.Description, File.ReadAllText(fileBasedProject.GetProjectInfo().LocalProjectFolder + "\\" + report.Path));
				RefreshView();
				MessageBox.Show($"The report {report.Name} was be updated!", PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		public string RenderReport(Report report)
		{
			string result = string.Empty;
			if (report == null)
			{
				return result;
			}
			var fileBasedProject = GetSelectedProject();
			if (fileBasedProject != null)
			{
				switch (report.Group)
				{
					case "Analysis":
						result = System.Text.Encoding.Default.GetString(new ProjectReportsOperations(fileBasedProject).GetReportRendering(report.Id, $"{Directory.GetCurrentDirectory()}\\Samples\\Analysis.xsl", "html"));
						break;

					case "WordCount":
						if (report.IsCustomReport)
						{
							result = System.Text.Encoding.Default.GetString(new ProjectReportsOperations(fileBasedProject).GetReportRendering(report.Id, $"{Directory.GetCurrentDirectory()}\\Samples\\BasicTemplate.xsl", "html"));
						}
						else
						{
							result = System.Text.Encoding.Default.GetString(new ProjectReportsOperations(fileBasedProject).GetReportRendering(report.Id, $"{Directory.GetCurrentDirectory()}\\Samples\\WordCount.xsl", "html"));
						}
						break;
					default:
						result = System.Text.Encoding.Default.GetString(new ProjectReportsOperations(fileBasedProject).GetReportRendering(report.Id, "html"));
						break;
				}				
				result = ReportCleanup(result);
			}
			return result;
		}

		public FileBasedProject GetSelectedProject()
		{
			if (_projectsController.CurrentProject is FileBasedProject fileBasedProject)
			{
				return fileBasedProject;
			}
			return null;
		}

		public List<Report> GetSelectedReports()
		{
			return _dataViewModel?.SelectedReports?.Cast<Report>().ToList();
		}

		public void RefreshView()
		{
			var fileBasedProject = GetSelectedProject();
			_reports = new ProjectReportsOperations(fileBasedProject).GetProjectReports();
			if (_dataViewModel != null)
			{
				_dataViewModel.Reports = new List<Report>(_reports);
			}
		}

		private void InitializeViews()
		{
			var fileBasedProject = GetSelectedProject();
			if (fileBasedProject == null)
			{
				return;
			}
			else

				_reports = new ProjectReportsOperations(fileBasedProject).GetProjectReports();

			_dataViewModel = new DataViewModel(_reports, this)
			{
				ProjectLocalFolder = fileBasedProject.GetProjectInfo().LocalProjectFolder
			};

			_dataView = new DataView
			{
				DataContext = _dataViewModel
			};


			_reportViewControl?.UpdateViewModel(_dataView);
		}

		private IEnumerable<Report> GetReports(IEnumerable<string> reportIds)
		{
			var reports = new List<Report>();
			foreach (var reportId in reportIds)
			{
				var report = _reports.FirstOrDefault(a => a.Id.ToString() == reportId);
				reports.Add(report);
			}

			return reports;
		}

		private string ReportCleanup(string report)
		{
			return Regex.Replace(report, @"[^\u0000-\u007F]+", string.Empty);
		}
	}
}
