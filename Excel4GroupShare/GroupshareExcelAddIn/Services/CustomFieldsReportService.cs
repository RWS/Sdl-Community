using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using Sdl.Community.GroupShareKit.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Constants = GroupshareExcelAddIn.Helper.Constants;

namespace GroupshareExcelAddIn.Services
{
    public class CustomFieldsReportService : IReportService
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private ExcelReporterService _excelReporter;

        public CustomFieldsReportService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection;
            _excelReporter = new ExcelReporterService();
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 2);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public string DisplayName => "Custom fields";

        public async Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken,
                    CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet)
        {
            var customFields = await GetCustomFields(filter);
            var resourceTypes = (await _groupshareConnection.GetResourceTypes()).ToDictionary(rt=>rt.Id, rt=>rt.Name);
            var organizationsPathsById = (await _groupshareConnection.GetOrganizations(true)).ToDictionary(org=>org.UniqueId.ToString(), org => org.Path);

            var report = AddHeaderToReport();
            GenerateReportData(dataRetrievalCancellationToken, customFields, report, resourceTypes, organizationsPathsById);

            _excelReporter.Token = dataWritingCancellationToken;
            await _excelReporter.PopulateExcelSheet(report, resourcesWorksheet);
        }

        private List<object[]> AddHeaderToReport()
        {
            var report = new List<object[]> { Constants.CustomFieldColumns.ToArray<object>() };
            return report;
        }

        private void GenerateReportData(CancellationToken dataRetrievalCancellationToken, List<CustomField> customFields, List<object[]> report, Dictionary<string, string> resourceTypes, Dictionary<string, string> organizationPathsById)
        {
            for (var index = 0; index < customFields.Count && !dataRetrievalCancellationToken.IsCancellationRequested; index++)
            {
                var customField = customFields[index];
                var organization = string.Empty;
                if (organizationPathsById.ContainsKey(customField.TenantId))
                {
                    organization = organizationPathsById[customField.TenantId];
                }
                report.Add(new object[]
                {
                    customField.Name,
                    customField.Description,
                    resourceTypes[customField.ResourceTypeId],
                    customField.Default,
                    customField.IsReadOnly,
                    organization
                });
                ProgressChanged?.Invoke(new Progress(index + 1, customFields.Count), 1);
            }
        }

        private async Task<List<CustomField>> GetCustomFields(ResourceFilter filter)
        {
            Organization organization = null;
            if (filter?.IncludeSubOrganizations == true)
            {
                organization = filter.Organization;
                filter.Organization = null;
            }

            var customFields = await _groupshareConnection.GetCustomFields(filter);
            customFields = GetFilteredCustomFields(organization, customFields);
            return customFields;
        }

        private List<CustomField> GetFilteredCustomFields(Organization organization, List<CustomField> customFields)
        {
            if (organization == null) return customFields;

            var listOfOrganizations = new List<Organization> { organization };
            _groupshareConnection.GetAllSubOrganizations(organization, listOfOrganizations);

            return GetFilteredCustomFields(customFields, listOfOrganizations);
        }

        private List<CustomField> GetFilteredCustomFields(List<CustomField> customFields, List<Organization> listOfOrganizations)
        {
            var filteredFieldTemplates = new List<CustomField>();
            foreach (var org in listOfOrganizations)
            {
                filteredFieldTemplates.AddRange(
                    customFields.Where(cf => cf.TenantId == org.UniqueId.ToString()));
            }

            return filteredFieldTemplates;
        }
    }
}