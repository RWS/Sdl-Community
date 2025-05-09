using GroupshareExcelAddIn.Models;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Interfaces;
using NLog;

namespace GroupshareExcelAddIn.Services
{
    public class ExcelReporterService : IExcelReporterService
    {
        private readonly Logger _logger = Log.GetLogger(nameof(ExcelReporterService));
        public dynamic _projectWorksheet;
        private int _entriesTotal;

        public delegate void ExcelWritingProgressChangedEventHandler(Progress progress);

        public event ExcelWritingProgressChangedEventHandler ProgressChanged;

        public CancellationToken Token { get; set; }

        public void AddEntryToReport(object[] entry, int row)
        {
            try
            {
                var range = (Range) _projectWorksheet.Range[
                    _projectWorksheet.Cells[row, 1],
                    _projectWorksheet.Cells[row, entry.Length]];

                range.Value = entry;
                AutoFit(row, _entriesTotal);
            }
            //we use this to suppress an error in Excel for when the user gets click happy
            catch (COMException ex)
            {
                _logger.Error($"Add entry to report failed {ex}");
            }
        }

        public async Task PopulateExcelSheet(List<object[]> table, dynamic projectWorksheet)
        {
            //TODO: make this method write everything at once using Range with a matrix form
            _entriesTotal = table.Count - 1;
            _projectWorksheet = projectWorksheet;
            PrepareWorkSheet();
            await Task.Run(() =>
            {
                for (var index = 0; index < table.Count; index++)
                {
                    AddEntryToReport(table[index], index + 1);
                    ProgressChanged?.Invoke(new Progress(index, table.Count - 1));

                    if (!Token.IsCancellationRequested) continue;

                    ProgressChanged?.Invoke(new Progress(table.Count - 1, table.Count - 1));
                    return;
                }
            });
        }

        private void AutoFit(int row, int total)
        {
            if (row % 10 == 0 || row == 1 || row == total)
            {
                _projectWorksheet?.Columns?.AutoFit();
            }
        }

        private void PrepareWorkSheet()
        {
            _projectWorksheet.Cells[1, 1].EntireRow.Font.Bold = true;
            _projectWorksheet.Application.ActiveWindow.SplitRow = 1;
            _projectWorksheet.Application.ActiveWindow.FreezePanes = true;
        }
    }
}