using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;

public class ReportExplorerViewModel : ViewModelBase
{
    private ObservableCollection<ReportInfo> _reports;
    private ReportInfo _selectedReport;

    public ReportExplorerViewModel()
    {
        Initialize();
    }

    public ObservableCollection<ReportInfo> Reports
    {
        get => _reports;
        set => SetField(ref _reports, value);
    }

    public ReportInfo SelectedReport
    {
        get => _selectedReport;
        set => SetField(ref _selectedReport, value);
    }

    public void RefreshReportList()
    {
        var myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var reportList = Directory.GetFiles(Path.Combine(myDocPath, "PostEdit.Compare", "Reports"), "*.html", SearchOption.AllDirectories).ToList();

        Reports = [];

        foreach (var report in reportList)
        {
            var directoryName = new DirectoryInfo(Path.GetDirectoryName(report) ?? string.Empty).Name;
            Reports.Insert(0, new ReportInfo
            {
                ReportName = $@"{directoryName}\\{Path.GetFileName(report)}",
                ReportPath = report
            });
        }
    }

    private void Initialize()
    {
        RefreshReportList();
    }
}