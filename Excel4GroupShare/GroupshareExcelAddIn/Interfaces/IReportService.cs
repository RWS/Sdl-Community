using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace GroupshareExcelAddIn.Interfaces
{
    public interface IReportService
    {
        event ProgressChangedEventHandler ProgressChanged;

        string DisplayName { get; }

        Task CreateResourceDataExcelSheet(CancellationToken dataRetrievalCancellationToken,
                            CancellationToken dataWritingCancellationToken, ResourceFilter filter, dynamic resourcesWorksheet);
    }
}