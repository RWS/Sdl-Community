using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.Reports.Viewer.API.Events;
using Sdl.Reports.Viewer.API.Model;
using Sdl.Reports.Viewer.API.Services;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Reports.Viewer.API
{
	public sealed class ReportsController
	{
		private static readonly Lazy<ReportsController> LazyController = new Lazy<ReportsController>(() => new ReportsController());
		private static readonly object LockObject = new object();
		private readonly ReportService _reportService;
		private readonly ProjectsController _projectsController;
		private List<Report> _reports;
		private string _currentProjectId;
		private IProject _selectedProject;

		private ReportsController()
		{
			_reportService = new ReportService(new ProjectSettingsService());
			_reports = new List<Report>();

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;

			SelectedProject = _projectsController.CurrentProject
							  ?? _projectsController.SelectedProjects.FirstOrDefault();
		}

		public static ReportsController Instance => LazyController.Value;

		public event EventHandler<ProjectChangingEventArgs> ProjectChanging;
		public event EventHandler<ProjectChangedEventArgs> ProjectChanged;
		public event EventHandler<ReportsAddedEventArgs> ReportsAdded;
		public event EventHandler<ReportsUpdatedEventArgs> ReportsUpdated;
		public event EventHandler<ReportsRemovedEventArgs> ReportsRemoved;

		public ActionResult AddReports(string clientId, List<Report> reports)
		{
			lock (LockObject)
			{
				if (reports == null || reports.Count == 0)
				{
					return new ActionResult(false)
					{
						Message = StringResources.Message_TheParameterCannotBeNull
					};
				}

				if (reports.Exists(a => string.IsNullOrEmpty(a.Id)))
				{
					return new ActionResult(false)
					{
						Message = StringResources.Message_TheReportIdCannotBeNull
					};
				}

				var clonedReports = AddRecordData(reports);
				UpdateProjectReports();

				ReportsAdded?.Invoke(this, new ReportsAddedEventArgs
				{
					ClientId = clientId,
					ProjectId = GetProjectId(),
					Reports = clonedReports
				});

				return new ActionResult(true) { Reports = clonedReports };
			}
		}

		public ActionResult UpdateReports(string clientId, List<Report> reports)
		{
			lock (LockObject)
			{
				if (reports == null || reports.Count == 0)
				{
					return new ActionResult(false)
					{
						Message = StringResources.Message_TheParameterCannotBeNull
					};
				}

				if (reports.Exists(a => string.IsNullOrEmpty(a.Id)))
				{
					return new ActionResult(false)
					{
						Message = StringResources.Message_TheReportIdCannotBeNull
					};
				}

				foreach (var report in reports)
				{
					var existingReport = _reports?.FirstOrDefault(a => a.Id == report.Id);
					if (existingReport == null)
					{
						return new ActionResult(false)
						{
							Message = string.Format(StringResources.Message_UnableToLocateReport, report.Id)
						};
					}
				}

				var clonedReports = UpdateReportData(reports);

				UpdateProjectReports();

				ReportsUpdated?.Invoke(this, new ReportsUpdatedEventArgs
				{
					ClientId = clientId,
					ProjectId = GetProjectId(),
					Reports = clonedReports
				});

				return new ActionResult(true) { Reports = clonedReports };
			}
		}

		public ActionResult RemoveReports(string clientId, List<string> reportIds)
		{
			lock (LockObject)
			{
				if (reportIds == null || reportIds.Count == 0)
				{
					return new ActionResult(false)
					{
						Message = StringResources.Message_TheParameterCannotBeNull
					};
				}

				if (reportIds.Exists(string.IsNullOrEmpty))
				{
					return new ActionResult(false)
					{
						Message = StringResources.Message_TheReportIdCannotBeNull
					};
				}

				foreach (var reportId in reportIds)
				{
					var existingReport = _reports?.FirstOrDefault(a => a.Id == reportId);
					if (existingReport == null)
					{
						return new ActionResult(false)
						{
							Message = string.Format(StringResources.Message_UnableToLocateReport, reportId)
						};
					}
				}

				var clonedReports = RemoveReportData(reportIds);

				UpdateProjectReports();

				ReportsRemoved?.Invoke(this, new ReportsRemovedEventArgs
				{
					ClientId = clientId,
					ProjectId = GetProjectId(),
					Reports = clonedReports
				});

				return new ActionResult(true) { Reports = clonedReports };
			}
		}

		public List<Report> GetReports()
		{
			lock (LockObject)
			{
				return GetClonedReports();
			}
		}

		public IProject SelectedProject
		{
			get => _selectedProject;
			internal set
			{
				var projectId = value?.GetProjectInfo().Id.ToString();
				if (_currentProjectId == projectId)
				{
					return;
				}

				var eventArgs = new ProjectChangingEventArgs
				{
					PreviousProjectId = _currentProjectId,
					ProjectId = projectId
				};

				_currentProjectId = projectId;
				_selectedProject = value;

				ProjectChanging?.Invoke(this, eventArgs);

				Task.Run(ReadProjectReports);
			}
		}

		public string GetReportsViewerFolder()
		{
			var localProjectFolder = _selectedProject.GetProjectInfo().LocalProjectFolder.Trim('\\');
			var reportsFolder = Path.Combine(localProjectFolder, Constants.ReportsViewerFolderName);
			if (!Directory.Exists(reportsFolder))
			{
				Directory.CreateDirectory(reportsFolder);
			}

			return reportsFolder;
		}

		public string GetProjectLocalFolder()
		{
			var localProjectFolder = _selectedProject.GetProjectInfo().LocalProjectFolder.Trim('\\');
			return localProjectFolder;
		}

		private string GetProjectId()
		{
			var projectInfo = SelectedProject?.GetProjectInfo();
			return projectInfo?.Id.ToString();
		}

		private async Task ReadProjectReports()
		{
			lock (LockObject)
			{
				string projectId = null;

				if (SelectedProject != null)
				{
					projectId = GetProjectId();

					var settingsBundle = SelectedProject.GetSettings();
					var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewer>();
					var reports = SerializeProjectFiles(reportViewerProject.ReportsJson.Value);

					var overwrite = true;
					// TODO: identify if we need to overwrite the existing studio reports 'always' when
					// switching the selected project?

					//if (Thread.CurrentThread.CurrentUICulture.Name != _currentUICulture.Name)
					//{
					//	_currentUICulture = Thread.CurrentThread.CurrentUICulture;
					//	overwrite = true;					
					//}

					reports.AddRange(_reportService.GetStudioReports(SelectedProject, overwrite));

					_reports = reports;
				}
				else
				{
					_currentProjectId = null;
					_reports = new List<Report>();
				}

				ProjectChanged?.Invoke(this, new ProjectChangedEventArgs
				{
					ProjectId = projectId,
					Reports = GetClonedReports()
				});
			}

			await Task.CompletedTask;
		}

		private void UpdateProjectReports()
		{
			var settingsBundle = SelectedProject.GetSettings();
			var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewer>();

			var reports = GetClonedReports().Where(a => !a.IsStudioReport).ToList();

			reportViewerProject.ReportsJson.Value = JsonConvert.SerializeObject(reports);

			SelectedProject.UpdateSettings(reportViewerProject.SettingsBundle);
			if (SelectedProject is FileBasedProject fileBasedProject)
			{
				fileBasedProject.Save();
			}
		}

		private List<Report> AddRecordData(IEnumerable<Report> reports)
		{
			var clonedReports = new List<Report>();
			foreach (var report in reports)
			{
				var existingReport = _reports?.FirstOrDefault(a => a.Id == report.Id);
				if (existingReport == null)
				{
					var clonedReport = GetClonedReport(report);
					if (_reports == null)
					{
						_reports = new List<Report>();
					}

					_reports.Add(clonedReport);
					clonedReports.Add(clonedReport);
				}
			}

			return clonedReports;
		}

		private List<Report> UpdateReportData(IEnumerable<Report> reports)
		{
			var clonedReports = new List<Report>();
			foreach (var report in reports)
			{
				var existingReport = _reports?.FirstOrDefault(a => a.Id == report.Id);
				if (existingReport == null)
				{
					continue;
				}

				var clonedReport = GetClonedReport(report);

				existingReport.Group = clonedReport.Group;
				existingReport.Language = clonedReport.Language;
				existingReport.Name = clonedReport.Name;
				existingReport.Description = clonedReport.Description;
				existingReport.Path = clonedReport.Path;

				clonedReports.Add(clonedReport);
			}

			return clonedReports;
		}

		private List<Report> RemoveReportData(IEnumerable<string> reportIds)
		{
			var reports = new List<Report>();
			foreach (var reportId in reportIds)
			{
				var existingReport = _reports?.FirstOrDefault(a => a.Id == reportId);
				if (existingReport == null)
				{
					continue;
				}

				var clonedReport = GetClonedReport(existingReport);
				reports.Add(clonedReport);

				DeleteReportFiles(clonedReport);

				_reports.Remove(existingReport);
			}

			return reports;
		}

		private List<Report> GetClonedReports()
		{
			var reports = new List<Report>();
			if (_reports != null)
			{
				foreach (var report in _reports)
				{
					reports.Add(GetClonedReport(report));
				}
			}

			return reports;
		}

		private Report GetClonedReport(Report report)
		{
			var clonedReport = report.Clone() as Report;

			MoveReportFilesToRelativePath(clonedReport);

			return clonedReport;
		}

		private void DeleteReportFiles(Report report)
		{
			var reportsFolder = GetReportsViewerFolder();
			if (!Directory.Exists(reportsFolder))
			{
				return;
			}

			try
			{
				var localProjectFolder = GetProjectLocalFolder();
				var reportFile = Path.Combine(localProjectFolder, report.Path);
				if (File.Exists(reportFile))
				{
					File.Delete(reportFile);
				}
			}
			catch
			{
				// catch all; ignore
			}
		}

		private void MoveReportFilesToRelativePath(Report report)
		{
			if (string.IsNullOrEmpty(report.Path) || !File.Exists(report.Path))
			{
				return;
			}

			if (report.IsStudioReport)
			{
				var localFolderPath = GetProjectLocalFolder();
				report.Path = report.Path.Substring(localFolderPath.Length + 1);
				return;
			}

			var reportsFolder = GetReportsViewerFolder();
			var reportsFolderName = Path.GetDirectoryName(report.Path);

			// Move report file to relative project folder
			if (string.Compare(reportsFolderName, Constants.ReportsViewerFolderName, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				var reportName = Path.GetFileName(report.Path);
				var reportPath = Path.Combine(reportsFolder, reportName);
				var index = 1;
				while (File.Exists(reportPath) && index < 1000)
				{
					var extension = Path.GetExtension(reportName);
					var fileName = reportPath.Substring(0, reportPath.Length - extension.Length);
					reportName = $"{fileName}({index++}){extension}";

					reportPath = Path.Combine(reportsFolder, reportName);
				}

				File.Copy(report.Path, reportPath);
				report.Path = $"{Constants.ReportsViewerFolderName}\\{Path.GetFileName(reportPath)}";
			}
		}

		private static List<Report> SerializeProjectFiles(string value)
		{
			try
			{
				var reports = JsonConvert.DeserializeObject<List<Report>>(value);
				return reports?.ToList() ?? new List<Report>();
			}
			catch
			{
				// catch all; ignore
			}

			return new List<Report>();
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			SelectedProject = _projectsController.CurrentProject
							  ?? _projectsController.SelectedProjects.FirstOrDefault();
		}
	}
}
