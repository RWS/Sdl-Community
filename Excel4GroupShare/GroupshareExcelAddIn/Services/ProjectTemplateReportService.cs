using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using Sdl.Community.GroupShareKit.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupshareExcelAddIn.Services
{
    public class ProjectTemplateReportService : IReportService
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private ExcelReporterService _excelReporter;

        public ProjectTemplateReportService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection;
            _excelReporter = new ExcelReporterService();
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 2);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public string DisplayName => "Project templates";

        public async Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken,
                    CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet)
        {
            var projectTemplates = await GetProjectTemplates(filter);
            var organizationPathsById =
                (await _groupshareConnection.GetOrganizations(true)).ToDictionary(org => org.UniqueId.ToString(), org => org.Path);

            if (projectTemplates.Count < 1) return;

            var report = AddHeaderToReport();
            GenerateReportData(dataRetrievalCancellationToken, projectTemplates, report, organizationPathsById);

            _excelReporter.Token = dataWritingCancellationToken;
            await _excelReporter.PopulateExcelSheet(report, resourcesWorksheet);
        }

        private List<object[]> AddHeaderToReport()
        {
            var report = new List<object[]>
            {
                Constants.ProjectTemplateColumns.ToArray<object>()
            };
            return report;
        }

        private void GenerateReportData(CancellationToken dataRetrievalCancellationToken, List<ProjectTemplates> projectTemplates, List<object[]> report, Dictionary<string, string> organizationPathsById)
        {
            for (var index = 0;
                index < projectTemplates.Count && !dataRetrievalCancellationToken.IsCancellationRequested;
                index++)
            {
                var projectTemplate = projectTemplates[index];
                var organization = string.Empty;
                if (organizationPathsById.ContainsKey(projectTemplate.OrganizationId))
                {
                    organization = organizationPathsById[projectTemplate.OrganizationId];
                }

                report.Add(new object[]
                {
                    projectTemplate.Name,
                    projectTemplate.Id,
                    projectTemplate.Description,
                    organization
                });
                ProgressChanged?.Invoke(new Progress(index + 1, projectTemplates.Count), 1);
            }
        }

        private List<ProjectTemplates> GetFilteredProjectTemplates(List<ProjectTemplates> projectTemplates, List<Organization> organizations)
        {
            var filteredLanguageResourceTemplates = new List<ProjectTemplates>();
            foreach (var org in organizations)
            {
                filteredLanguageResourceTemplates.AddRange(
                    projectTemplates.Where(pt => pt.OrganizationId == org.UniqueId.ToString()));
            }

            return filteredLanguageResourceTemplates;
        }

        private async Task<List<ProjectTemplates>> GetProjectTemplates(ResourceFilter filter)
        {
            var projectTemplates = (await _groupshareConnection.GroupShareClient.Project.GetAllTemplates()).ToList();

            if (filter?.Organization == null) return projectTemplates;

            var listOfOrganizations = new List<Organization> { filter.Organization };
            if (filter.IncludeSubOrganizations)
            {
                _groupshareConnection.GetAllSubOrganizations(filter.Organization, listOfOrganizations);
            }

            return GetFilteredProjectTemplates(projectTemplates, listOfOrganizations);
        }
    }
}