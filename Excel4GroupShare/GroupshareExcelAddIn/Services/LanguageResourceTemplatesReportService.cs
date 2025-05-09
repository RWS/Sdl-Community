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
    public class LanguageResourceTemplatesReportService : IReportService
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private ExcelReporterService _excelReporter;
        private List<object[]> _report;

        public LanguageResourceTemplatesReportService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection;
            _excelReporter = new ExcelReporterService();
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 2);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public string DisplayName => "Language resource templates";

        public async Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken,
                    CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet)
        {
            var languageResourceTemplates = (await _groupshareConnection.GroupShareClient.TranslationMemories
                .GetAllLanguageResourceTemplates()).Items;

            languageResourceTemplates = GetFilteredLanguageResourceTemplates(filter, languageResourceTemplates);

            GenerateReportData(dataRetrievalCancellationToken, languageResourceTemplates);
            _excelReporter.Token = dataWritingCancellationToken;

            await _excelReporter.PopulateExcelSheet(_report, resourcesWorksheet);
        }

        private void AddHeaderToReport()
        {
            _report = new List<object[]>
            {
                Constants.LanguageResourceTemplateColumns.ToArray<object>()
            };
        }

        private void GenerateReportData(CancellationToken dataRetrievalCancellationToken, List<LanguageResourceTemplate> languageResourceTemplates)
        {
            AddHeaderToReport();
            for (var index = 0;
                index < languageResourceTemplates.Count && !dataRetrievalCancellationToken.IsCancellationRequested;
                index++)
            {
                var resTemplate = languageResourceTemplates[index];
                var langRes = string.Join(", ",
                    resTemplate.LanguageResources.Select(lr => $"[{lr.CultureName}, {lr.Type}]"));
                _report.Add(new object[]
                {
                    resTemplate.Name,
                    resTemplate.Location,
                    resTemplate.Description,
                    resTemplate.LanguageResourceTemplateId,
                    resTemplate.OwnerId,
                    resTemplate.IsTmSpecific,
                    langRes
                });
                ProgressChanged?.Invoke(new Progress(index + 1, languageResourceTemplates.Count), 1);
            }
        }

        private List<LanguageResourceTemplate> GetFilteredLanguageResourceTemplates(ResourceFilter filter, List<LanguageResourceTemplate> languageResourceTemplates)
        {
            if (filter?.Organization == null) return languageResourceTemplates;

            var listOfOrganizations = new List<Organization> { filter.Organization };
            if (filter.IncludeSubOrganizations)
            {
                _groupshareConnection.GetAllSubOrganizations(filter.Organization, listOfOrganizations);
            }

            return GetFilteredLanguageResourceTemplates(languageResourceTemplates, listOfOrganizations);
        }

        private List<LanguageResourceTemplate> GetFilteredLanguageResourceTemplates(List<LanguageResourceTemplate> languageResourceTemplates, List<Organization> listOfOrganizations)
        {
            var filteredLanguageResourceTemplates = new List<LanguageResourceTemplate>();
            foreach (var org in listOfOrganizations)
            {
                filteredLanguageResourceTemplates.AddRange(
                    languageResourceTemplates.Where(lrt => lrt.OwnerId == org.UniqueId.ToString()));
            }

            return filteredLanguageResourceTemplates;
        }
    }
}