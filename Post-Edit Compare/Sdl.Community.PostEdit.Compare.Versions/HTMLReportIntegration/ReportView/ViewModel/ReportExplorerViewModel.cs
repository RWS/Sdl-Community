using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;

public class ReportExplorerViewModel : ViewModelBase
{
    private ObservableCollection<ProjectInfo> _projects;
    private ObservableCollection<ReportInfo> _reports;
    private ObservableCollection<object> _selectedProjects = [];
    private ReportInfo _selectedReport;

    public ReportExplorerViewModel() => SelectedProjects.CollectionChanged += SelectedProjects_CollectionChanged;


    public ObservableCollection<ProjectInfo> Projects
    {
        get => _projects;
        set => SetField(ref _projects, value);
    }

    public ObservableCollection<ReportInfo> Reports
    {
        get => _reports;
        set => SetField(ref _reports, value);
    }

    public ObservableCollection<object> SelectedProjects
    {
        get => _selectedProjects;
        set => SetField(ref _selectedProjects, value);
    }

    public ReportInfo SelectedReport
    {
        get => _selectedReport;
        set => SetField(ref _selectedReport, value);
    }

    public void SetProjectsList(List<ProjectInfo> projects) =>
        Projects = new ObservableCollection<ProjectInfo>(projects);

    public void SetReportsList(List<ReportInfo> reports) => Reports = new ObservableCollection<ReportInfo>(reports);

    private void FilterProjects()
    {
        var reports = CollectionViewSource.GetDefaultView(Reports);
        reports.Filter = !SelectedProjects.Any() || SelectedProjects.Count == 0
            ? null
            : report => report is ReportInfo reportInfo && SelectedProjects.Cast<ProjectInfo>()
                .Select(sp => sp.Id.ToString())
                .ToList()
                .Contains(reportInfo.ProjectId);
    }

    private void SelectedProjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
        FilterProjects();
}