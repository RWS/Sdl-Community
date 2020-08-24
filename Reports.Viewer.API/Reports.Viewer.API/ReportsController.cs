using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
		private readonly EditorController _editorController;
		private List<Report> _reports;
		private string _previousProjectId;
		private IProject _selectedProject;
		private bool _ignoreChanges;

		private ReportsController()
		{
			_reportService = new ReportService(new ProjectSettingsService());
			_reports = new List<Report>();

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;

			_editorController = SdlTradosStudio.Application.GetController<EditorController>();

			SelectedProject = _projectsController.CurrentProject
							  ?? _projectsController.SelectedProjects.FirstOrDefault();
		}

		public static ReportsController Instance => LazyController.Value;

		public event EventHandler<ProjectChangingEventArgs> ProjectChanging;
		public event EventHandler<ProjectChangedEventArgs> ProjectChanged;
		public event EventHandler<ReportsAddedEventArgs> ReportsAdded;
		public event EventHandler<ReportsUpdatedEventArgs> ReportsUpdated;
		public event EventHandler<ReportsRemovedEventArgs> ReportsRemoved;
		public event EventHandler<ProjectReportChangesEventArgs> ProjectReportChanges;

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

				List<Report> clonedReports = null;

				try
				{
					clonedReports = UpdateReportData(reports);

					UpdateProjectReports();

					ReportsUpdated?.Invoke(this, new ReportsUpdatedEventArgs
					{
						ClientId = clientId,
						ProjectId = GetProjectId(),
						Reports = clonedReports
					});

					return new ActionResult(true) { Reports = clonedReports };
				}
				catch (Exception e)
				{
					return new ActionResult(false)
					{
						Reports = clonedReports,
						Message = e.Message						
					};
				}
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
	
				List<Report> clonedReports = null;

				try
				{
					clonedReports = RemoveReportData(reportIds);

					UpdateProjectReports();

					ReportsRemoved?.Invoke(this, new ReportsRemovedEventArgs
					{
						ClientId = clientId,
						ProjectId = GetProjectId(),
						Reports = clonedReports
					});

					return new ActionResult(true) { Reports = clonedReports };
				}
				catch (Exception e)
				{
					return new ActionResult(false)
					{
						Reports = clonedReports,
						Message = e.Message
					};
				}
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
				if (_previousProjectId == projectId)
				{
					return;
				}

				var eventArgs = new ProjectChangingEventArgs
				{
					PreviousProjectId = _previousProjectId,
					ProjectId = projectId
				};

				_previousProjectId = projectId;
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

		public async Task<ReportChanges> GetStudioReportUpdates(string clientId)
		{
			var clonedAddedReports = new List<Report>();
			var clonedRemovedReports = new List<Report>();
			var studioReports = _reportService.GetStudioReports(SelectedProject, false).ToList();
			foreach (var studioReport in studioReports)
			{
				if (!_reports.Exists(a => a.IsStudioReport && a.Id == studioReport.Id))
				{
					_reports.Add(studioReport);
					clonedAddedReports.Add(studioReport.Clone() as Report);
				}
			}

			foreach (var report in _reports)
			{
				if (report.IsStudioReport && !studioReports.Exists(a => a.Id == report.Id))
				{
					clonedRemovedReports.Add(report.Clone() as Report);
				}
			}

			foreach (var removedReport in clonedRemovedReports)
			{
				_reports.RemoveAll(a => a.Id == removedReport.Id);
			}

			ProjectReportChanges?.Invoke(this, new ProjectReportChangesEventArgs
			{
				ClientId = clientId,
				ProjectId = GetProjectId(),
				AddedReports = clonedAddedReports,
				RemovedReports = clonedRemovedReports
			});

			var reportChanges = new ReportChanges
			{
				ProjectId = GetProjectId(),
				AddedReports = clonedAddedReports,
				RemovedReports = clonedRemovedReports
			};

			return await Task.FromResult(reportChanges);
		}

		private string GetProjectId()
		{
			return GetProjectId(SelectedProject);
		}

		private string GetProjectId(IProject project)
		{
			var projectInfo = project?.GetProjectInfo();
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
					var reportViewerSettings = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();
					var reports = SerializeProjectFiles(reportViewerSettings.ReportsJson.Value);
					var overwrite = CanOverwriteExistingReports(reportViewerSettings);

					reports.AddRange(_reportService.GetStudioReports(SelectedProject, overwrite));

					_reports = reports;
				}
				else
				{
					_previousProjectId = null;
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

		private bool CanOverwriteExistingReports(ReportsViewerSettings reportsViewerSettings)
		{
			var uiCultureName = reportsViewerSettings.UICultureName;

			if (Thread.CurrentThread.CurrentUICulture.Name != uiCultureName.Value)
			{
				uiCultureName.Value = Thread.CurrentThread.CurrentUICulture.Name;
				SelectedProject.UpdateSettings(reportsViewerSettings.SettingsBundle);
				if (SelectedProject is FileBasedProject fileBasedProject)
				{
					fileBasedProject.Save();
				}

				return true;
			}

			return false;
		}

		private void UpdateProjectReports()
		{
			var settingsBundle = SelectedProject.GetSettings();
			var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();

			var reports = GetClonedReports().Where(a => !a.IsStudioReport).ToList();

			reportViewerProject.ReportsJson.Value = JsonConvert.SerializeObject(reports);
			reportViewerProject.UICultureName.Value = Thread.CurrentThread.CurrentUICulture.Name;

			SelectedProject.UpdateSettings(reportViewerProject.SettingsBundle);
			if (SelectedProject is FileBasedProject fileBasedProject)
			{
				fileBasedProject.Save();
			}
		}

		private List<Report> AddRecordData(List<Report> reports)
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

		private List<Report> UpdateReportData(List<Report> reports)
		{
			var clonedReports = new List<Report>();
			if (reports == null)
			{
				return clonedReports;
			}

			var documents = _editorController.GetDocuments().ToList();
			var projectId = GetProjectId();
			var hasStudioReports = reports.Exists(a => a.IsStudioReport);
			var documentsOpen = documents.Exists(a => GetProjectId(a.Project) == projectId);
			if (hasStudioReports && documentsOpen)
			{
				throw new Exception("Please close project documents that are open in the editor before updating the selected reports");
			}


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

				clonedReports.Add(existingReport.Clone() as Report);
			}


			if (hasStudioReports)
			{
				try
				{
					_ignoreChanges = true;
					var studioReports = clonedReports.Where(a => a.IsStudioReport).ToList();

					if (SelectedProject is FileBasedProject fileBasedProject)
					{
						_projectsController.Close(fileBasedProject);
						_reportService.UpdateStudioReportInformation(fileBasedProject.FilePath, studioReports);
						_projectsController.Add(fileBasedProject.FilePath);
					}
				}
				finally
				{
					_ignoreChanges = false;
				}
			}

			return clonedReports;
		}

		private List<Report> RemoveReportData(List<string> reportIds)
		{
			var clonedReports = new List<Report>();
			if (reportIds == null)
			{
				return clonedReports;
			}

			var documents = _editorController.GetDocuments().ToList();
			var projectId = GetProjectId();
			var reports = GetReports(reportIds);
			var hasStudioReports = reports.Exists(a => a.IsStudioReport);
			var documentsOpen = documents.Exists(a => GetProjectId(a.Project) == projectId);
			if (hasStudioReports && documentsOpen)
			{
				throw new Exception("Please close project documents that are open in the editor before removing the selected reports");
			}

			foreach (var reportId in reportIds)
			{
				var existingReport = _reports?.FirstOrDefault(a => a.Id == reportId);
				if (existingReport == null)
				{
					continue;
				}

				var clonedReport = GetClonedReport(existingReport);
				clonedReports.Add(clonedReport);

				DeleteReportFile(clonedReport.Path);
				if (clonedReport.IsStudioReport)
				{
					var extension = Path.GetExtension(clonedReport.Path);
					if (extension != null)
					{
						var xmlPath = clonedReport.Path.Substring(0, clonedReport.Path.Length - extension.Length);
						DeleteReportFile(xmlPath);
					}
				}

				_reports.Remove(existingReport);
			}

			if (hasStudioReports)
			{
				try
				{
					_ignoreChanges = true;

					var studioReports = clonedReports.Where(a => a.IsStudioReport).ToList();
					if (SelectedProject is FileBasedProject fileBasedProject)
					{
						_projectsController.Close(fileBasedProject);
						_reportService.DeleteStudioReportInformation(fileBasedProject.FilePath, studioReports);
						_projectsController.Add(fileBasedProject.FilePath);
					}
				}
				finally
				{
					_ignoreChanges = false;
				}
			}

			return clonedReports;
		}

		private List<Report> GetReports(List<string> reportIds)
		{
			var reports = new List<Report>();
			foreach (var reportId in reportIds)
			{
				var report = _reports?.FirstOrDefault(a => a.Id == reportId);
				if (report == null)
				{
					continue;
				}

				reports.Add(report);
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

		private void DeleteReportFile(string path)
		{
			var projectLocalFolder = GetProjectLocalFolder();
			if (!Directory.Exists(projectLocalFolder))
			{
				return;
			}

			try
			{
				var reportFile = Path.Combine(projectLocalFolder, path);
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
				var reportFullPath = Path.Combine(reportsFolder, reportName);
				var index = 1;
				while (File.Exists(reportFullPath) && index < 1000)
				{
					var extension = Path.GetExtension(reportName);
					var fileName = reportName.Substring(0, reportName.Length - extension.Length);
					var newReportName = $"{fileName}({index++}){extension}";

					reportFullPath = Path.Combine(reportsFolder, newReportName);
				}

				File.Copy(report.Path, reportFullPath);
				report.Path = $"{Constants.ReportsViewerFolderName}\\{Path.GetFileName(reportFullPath)}";
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
			if (_ignoreChanges)
			{
				return;
			}

			SelectedProject = _projectsController.CurrentProject
							  ?? _projectsController.SelectedProjects.FirstOrDefault();
		}
	}
}
