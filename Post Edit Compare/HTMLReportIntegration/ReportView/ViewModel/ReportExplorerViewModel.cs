using Sdl.Community.PostEdit.Versions.Commands;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;

public class ReportExplorerViewModel : ViewModelBase
{
    private List<ReportInfo> _reports;
    private ReportInfo _selectedReport;
    private ICommand _refreshCommand;


    public ReportExplorerViewModel()
    {
        Initialize();
    }

    public List<ReportInfo> Reports
    {
        get => _reports;
        set => SetField(ref _reports, value);
    }

    public ReportInfo SelectedReport
    {
        get => _selectedReport;
        set => SetField(ref _selectedReport, value);
    }

    public ICommand RefreshCommand => _refreshCommand ??= new RelayCommand(RefreshReportList);


    private void Initialize()
    {
        RefreshReportList();
    }

    private void RefreshReportList()
    {
        var myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var reportList = Directory.GetFiles(Path.Combine(myDocPath, "PostEdit.Compare", "Reports"), "*.html", SearchOption.AllDirectories).ToList();

        Reports = [];

        foreach (var report in reportList)
        {
            var directoryName = new DirectoryInfo(Path.GetDirectoryName(report) ?? string.Empty).Name;
            Reports.Add(new ReportInfo
            {
                ReportName = $@"{directoryName}\\{Path.GetFileName(report)}",
                ReportPath = report
            });
        }
    }
}