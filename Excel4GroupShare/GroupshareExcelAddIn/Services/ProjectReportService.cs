using System;
using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using Sdl.Community.GroupShareKit.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Interfaces;
using NLog;

namespace GroupshareExcelAddIn.Services
{
    public class ProjectReportService
    {
        private readonly IExcelReporterService _excelReporter;
        private readonly IGroupshareConnection _groupShareConnection;
        private CancellationToken _cToken;
        private dynamic _projectWorksheet;
        private readonly Logger _logger = Log.GetLogger(nameof(ProjectReportService));
        private List<object[]> _report;

        public ProjectReportService(IGroupshareConnection groupShareConnection, IExcelReporterService excelReporterService)
        {
            _groupShareConnection = groupShareConnection;
            _excelReporter = excelReporterService;
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 3);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public async Task CreateFilteredProjectDataExcelSheet(ProjectFilter filter, CancellationToken dataRetrievalCancellationToken, CancellationToken dataWritingCancellationToken, dynamic projectWorkSheet)
        {
            _cToken = dataRetrievalCancellationToken;
            _projectWorksheet = projectWorkSheet;
            await CreateReport(filter, dataWritingCancellationToken);
        }

        private void AddHeaderToReport(ProjectFilter filter)
        {
            _report = new List<object[]>();
            var reportHeader = Constants.ProjectColumns.ToArray<object>();

            if (filter.IncludePhasesAndAssignees)
            {
                var reportHeaderAsList = reportHeader.ToList();
                reportHeaderAsList.AddRange(Constants.PhasesList);
                reportHeader = reportHeaderAsList.ToArray();
            }

            _report.Add(reportHeader);
        }

        private async Task CreateReport(ProjectFilter filter, CancellationToken dataWritingCancellationToken)
        {
            var projects = await _groupShareConnection.GetGsProjects(filter);

            await GenerateReportData(filter, projects);
            _excelReporter.Token = dataWritingCancellationToken;
            await _excelReporter.PopulateExcelSheet(_report, _projectWorksheet);
        }

        private async Task GenerateReportData(ProjectFilter filter, List<ProjectDetailsResponse> projects)
        {
            if (projects == null) return;

            AddHeaderToReport(filter);
            for (var projIterator = 0; projIterator < projects.Count && !_cToken.IsCancellationRequested; projIterator++)
            {
                ProgressChanged?.Invoke(new Progress(projIterator, projects.Count), 1);
                var files = await _groupShareConnection.GetProjectFiles(projects[projIterator]);

                ProgressChanged?.Invoke(new Progress(0, files.Count), 2);
                await GetReportEntriesForEachFile(filter, _cToken, files, projects[projIterator]);
            }
        }

        private object[] GetEntry(ProjectDetailsResponse project, File file, Dictionary<string, string> phasesWithAssignees)
        {
            if (file == null)
            {
                return null;
            }

            var assigneesNames = string.Empty;
            var assigneesRoles = string.Empty;
            var assigneesEmails = string.Empty;

            if (file.Assignees?.Count > 0)
            {
                assigneesNames = string.Join(", ", file.Assignees.Select(assignee => assignee.DisplayName));
                assigneesRoles = string.Join(", ",
                    file.Assignees.Select(assignee => $"{assignee.DisplayName}'s role(s): [{assignee.Role}]"));
                assigneesEmails = string.Join(", ", file.Assignees.Select(assignee => assignee.Email));
            }

            return new object[]
            {
                    project.OrganizationName,
                    project.Name,
                    project.CreatedAt?.Date,
                    project.DueDate?.Date,
                    project.CustomerName,
                    project.SourceLanguage + '-' + project.TargetLanguage,
                    Constants.ProjectStatusDict[project.Status],
                    project.ProjectDescription,
                    file.Status,
                    file.FileName,
                    file.FileType,
                    file.FileSize,
                    file.FileRole,
                    file.LanguageCode,
                    file.CheckInBy?.DisplayName,
                    file.CheckOutTo?.DisplayName,
                    file.LastCheckIn?.Date,
                    file.LastCheckOut?.Date,
                    file.LastModified?.Date,
                    file.Statistics?.ApprovedSignOffWords,
                    file.Statistics?.ApprovedTranslationWords,
                    file.Statistics?.DraftWords,
                    file.Statistics?.RejectedSignOffWords,
                    file.Statistics?.RejectedTranslationWords,
                    file.Statistics?.TotalWords,
                    file.Statistics?.TranslatedWords,
                    file.Statistics?.UnspecifiedWords,
                    file.Statistics?.TypePercentComplete,
                    assigneesNames,
                    assigneesRoles,
                    assigneesEmails,
                    file.Assignment?.ProjectPhase,
                    phasesWithAssignees?["Preparation"],
                    phasesWithAssignees?["Translation"],
                    phasesWithAssignees?["Review"],
                    phasesWithAssignees?["Finalisation"]
                };
        }

        private async Task<Dictionary<string, string>> GetPhasesWithAssigneesForFile(File file, ProjectDetailsResponse project)
        {
            var phases =
                await _groupShareConnection.GroupShareClient.Project.GetAllPhasesForProject(project.ProjectId);

            if (phases == null)
            {
                return null;
            }

            var phasesWithAssignees = new Dictionary<string, string>
            {
                ["Preparation"] = null,
                ["Translation"] = null,
                ["Review"] = null,
                ["Finalisation"] = null
            };

            foreach (var phase in phases)
            {
                IReadOnlyList<PhasesWithAssignees> phaseResponse = null;
                try
                {
                    phaseResponse = (await
                        _groupShareConnection?.GroupShareClient?.Project?.GetPhasesWithAssignees(project.ProjectId,
                            phase.ProjectPhaseId));
                }
                catch (Exception ex)
                {
                    _logger.Error($"Login {ex.Message}\n {ex.StackTrace}");
                }

                var phaseDetails = phaseResponse?[0]?.Phases?[0];
                var assignee = file?.Assignees?.FirstOrDefault(fa => phaseDetails?.Assignees?.FirstOrDefault(pd => pd == fa?.UserId) != null);
                if (assignee != null)
                {
                    phasesWithAssignees[phase.Name] = assignee.DisplayName;
                }
            }

            return phasesWithAssignees;
        }

        private async Task GetReportEntriesForEachFile(ProjectFilter filter, CancellationToken cToken, List<File> files,
            ProjectDetailsResponse project)
        {
            var localIterator = 1;
            foreach (var file in files)
            {
                Dictionary<string, string> phasesAndAssigneesForCurrentFile = null;
                if (filter.IncludePhasesAndAssignees)
                {
                    phasesAndAssigneesForCurrentFile = await GetPhasesWithAssigneesForFile(file, project);
                }
                _report.Add(GetEntry(project, file, phasesAndAssigneesForCurrentFile));
                ProgressChanged?.Invoke(new Progress(localIterator), 2);

                if (cToken.IsCancellationRequested)
                {
                    return;
                }
                localIterator++;
            }
        }
    }
}