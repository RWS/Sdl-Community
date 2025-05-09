using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Models.Response;
using Termbase = GroupshareExcelAddIn.Models.Termbase;

namespace GroupshareExcelAddIn.Services
{
    public class TermbasesReportService : IReportService
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private ExcelReporterService _excelReporter;
        private CancellationToken _dataRetrievalCancellationToken;

        public TermbasesReportService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection;
            _groupshareConnection.ProgressChanged +=
                (progress, barIndex) => ProgressChanged?.Invoke(progress, barIndex);
            _excelReporter = new ExcelReporterService();
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 2);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public string DisplayName => "Termbases";

        public async Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken,
                    CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet)
        {
            _dataRetrievalCancellationToken = dataRetrievalCancellationToken;
            var termbases = await GetTermbases(filter);
            var organizationsById =
                (await _groupshareConnection.GetOrganizations(true)).ToDictionary(org => org.UniqueId.ToString(), org => org);

            var report = AddHeaderToReport();
            GenerateReportData(termbases, report, organizationsById);

            _excelReporter.Token = dataWritingCancellationToken;
            await _excelReporter.PopulateExcelSheet(report, resourcesWorksheet);
        }

        private List<object[]> AddHeaderToReport()
        {
            var report = new List<object[]>()
            {
                Constants.TermbaseColumns.ToArray<object>()
            };
            return report;
        }

        private void GenerateReportData(List<Termbase> termbases, List<object[]> report, Dictionary<string, Organization> organizationsById)
        {
            termbases.ForEach(tb =>
            {
                var index = 0;
                do
                {
                    if (tb == null) continue;
                    var linkedOrganization = tb.LinkedOrganizationIds?.Length > 0 ? tb.LinkedOrganizationIds[index] : null;
                    var pathOfLinkedOrganization = string.Empty;
                    if (linkedOrganization != null)
                    {
                        if (organizationsById.ContainsKey(linkedOrganization))
                        {
                            pathOfLinkedOrganization = organizationsById[linkedOrganization].Path;
                        }
                    }
                    report.Add(new object[]
                    {
                        tb.Name,
                        tb.Id,
                        tb.Description,
                        organizationsById[tb.ParentOrganizationId]?.Path,
                        pathOfLinkedOrganization,
                    });
                    index++;

                } while (index < tb.LinkedOrganizationIds.Length);

            });
        }

        private async Task<List<Termbase>> GetTermbases(ResourceFilter filter)
        {
            return await _groupshareConnection.GetTermbases(filter, _dataRetrievalCancellationToken);
        }
    }
}