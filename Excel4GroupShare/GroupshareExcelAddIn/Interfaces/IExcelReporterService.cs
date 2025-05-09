using GroupshareExcelAddIn.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GroupshareExcelAddIn.Interfaces
{
    public interface IExcelReporterService
    {
        event ExcelReporterService.ExcelWritingProgressChangedEventHandler ProgressChanged;

        CancellationToken Token { get; set; }

        void AddEntryToReport(object[] entry, int row);

        Task PopulateExcelSheet(List<object[]> table, dynamic projectWorksheet);
    }
}