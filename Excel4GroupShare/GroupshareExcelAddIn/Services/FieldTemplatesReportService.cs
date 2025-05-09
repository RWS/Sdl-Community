using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using Sdl.Community.GroupShareKit.Models.Response;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupshareExcelAddIn.Services
{
    public class FieldTemplatesReportService : IReportService
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private ExcelReporterService _excelReporter;

        public FieldTemplatesReportService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection;
            _excelReporter = new ExcelReporterService();
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 2);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public string DisplayName => "Field templates";

        public async Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken,
                    CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet)
        {
            var fieldTemplates = (await _groupshareConnection.GroupShareClient.TranslationMemories.GetFieldTemplates()).Items;
            fieldTemplates = GetFilteredFieldTemplates(filter, fieldTemplates);

            if (fieldTemplates.Count < 1) return;

            var report = GenerateReportData(dataRetrievalCancellationToken, fieldTemplates);

            _excelReporter.Token = dataWritingCancellationToken;
            _excelReporter.PopulateExcelSheet(report, resourcesWorksheet);
        }

        private List<object[]> AddHeaderToReport()
        {
            var report = new List<object[]>
            {
                Constants.FieldTemplateColumns.ToArray<object>()
            };
            return report;
        }

        private List<object[]> GenerateReportData(CancellationToken dataRetrievalCancellationToken, List<FieldTemplate> fieldTemplates)
        {
            var report = AddHeaderToReport();
            for (var index = 0;
                index < fieldTemplates.Count && !dataRetrievalCancellationToken.IsCancellationRequested;
                index++)
            {
                var fieldTemplate = fieldTemplates[index];
                var fields = SerializeFields(fieldTemplate);
                report.Add(new object[]
                {
                    fieldTemplate.Name,
                    fieldTemplate.Location,
                    fieldTemplate.Description,
                    fieldTemplate.FieldTemplateId,
                    fieldTemplate.OwnerId,
                    fieldTemplate.IsTmSpecific,
                    fields
                });
                ProgressChanged?.Invoke(new Progress(index + 1, fieldTemplates.Count), 1);
            }

            return report;
        }

        private List<FieldTemplate> GetFilteredFieldTemplates(ResourceFilter filter, List<FieldTemplate> fieldTemplates)
        {
            if (filter?.Organization == null) return fieldTemplates;

            var listOfOrganizations = new List<Organization> { filter.Organization };
            if (filter.IncludeSubOrganizations)
            {
                _groupshareConnection.GetAllSubOrganizations(filter.Organization, listOfOrganizations);
            }

            return GetFilteredFieldTemplates(fieldTemplates, listOfOrganizations);
        }

        private List<FieldTemplate> GetFilteredFieldTemplates(List<FieldTemplate> fieldTemplates, List<Organization> listOfOrganizations)
        {
            var filteredFieldTemplates = new List<FieldTemplate>();
            foreach (var org in listOfOrganizations)
            {
                filteredFieldTemplates.AddRange(
                    fieldTemplates.Where(lrt => lrt.OwnerId == org.UniqueId.ToString()));
            }

            return filteredFieldTemplates;
        }

        private string SerializeFields(FieldTemplate fieldTemplate)
        {
            var fieldValues = fieldTemplate.Fields.ToDictionary(field => $"({field.Type}){field.Name}",
                field => string.Join(", ", field.Values.Select(v => v.Name)));
            var fields = string.Join("; ", fieldValues.Select(fv => $"{fv.Key}: {fv.Value}"));
            return fields;
        }
    }
}