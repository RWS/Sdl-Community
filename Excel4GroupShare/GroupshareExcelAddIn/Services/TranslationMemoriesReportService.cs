using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupshareExcelAddIn.Services
{
    public class TranslationMemoriesReportService : IReportService
    {
        private readonly ExcelReporterService _excelReporter;
        private readonly IGroupshareConnection _groupshareConnection;
        private CancellationToken _dataRetrievalCancellationToken;
        private List<object[]> _report;
        private dynamic _resourcesWorksheet;

        public TranslationMemoriesReportService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection ?? throw new ArgumentNullException(nameof(groupshareConnection));
            _groupshareConnection.ConnectionChanged += (o, args) =>
            {
                ResourcesSourceChanged?.Invoke(o, EventArgs.Empty);
            };

            _excelReporter = new ExcelReporterService();
            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 2);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public event EventHandler ResourcesSourceChanged;

        public string DisplayName => "Translation memories";

        public async Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken, CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet)
        {
            _dataRetrievalCancellationToken = dataRetrievalCancellationToken;
            _resourcesWorksheet = resourcesWorksheet;
            await CreateReport(dataWritingCancellationToken, filter);
        }

        private void AddHeaderToReport()
        {
            _report = new List<object[]>();
            var reportHeader = Constants.TranslationMemoryColumns?.ToArray<object>();
            _report.Add(reportHeader);
        }

        private async Task CreateReport(CancellationToken dataWritingCancellationToken, ResourceFilter filter)
        {
            var tms = await GetTms(filter);
            GenerateReportData(tms);
            _excelReporter.Token = dataWritingCancellationToken;
            await _excelReporter.PopulateExcelSheet(_report, _resourcesWorksheet);
        }

        private void GenerateReportData(List<TranslationMemoryDetails> tms)
        {
            AddHeaderToReport();
            for (var index = 0; index < tms.Count && !_dataRetrievalCancellationToken.IsCancellationRequested; index++)
            {
                tms[index].LanguageDirections.ForEach(ld => _report.Add(GetEntry(tms[index], ld)));
                ProgressChanged?.Invoke(new Progress(index + 1, tms.Count), 1);
            }
        }

        private object[] GetEntry(TranslationMemoryDetails tm, LanguageDirection languageDirection)
        {
            if (tm?.CreatedOn != null)
                return new object[]
                {
                        tm.Name,
                        $"{languageDirection.Source} -> {languageDirection.Target}",
                        languageDirection.TuCount,
                        tm.Description,
                        tm.Location,
                        tm.CreatedOn.Value.Date,
                        tm.TokenizerFlags,
                        tm.WordCountFlags,
                        tm.LastRecomputedDate?.Date,
                        tm.LastRecomputedSize ?? 0,
                        tm.FuzzyIndexes,
                        tm.LastReIndexDate?.Date,
                        tm.LastReIndexSize,
                        tm.ShouldRecomputeStatistics
                };
            return null;
        }

        private async Task<List<TranslationMemoryDetails>> GetTms(ResourceFilter filter)
        {
            return (await _groupshareConnection.GetGsTms(filter))?.Items;
        }
    }
}