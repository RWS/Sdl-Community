using Sdl.Community.PostEdit.Versions.Commands;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;

public class ReportExplorerViewModel : ViewModelBase
{
    private ICommand _clearCommand;
    private string _filterString = "";
    private ObservableCollection<ProjectInfo> _projects;
    private ObservableCollection<ReportInfo> _reports;
    private ReportInfo _selectedReport;
    private ObservableCollection<ReportGroup> _reportGroups = [];
    private bool _isLoading;
    public ICommand ClearFilterCommand => _clearCommand ??= new RelayCommand(ClearFilter);

    public string FilterString
    {
        get => _filterString;
        set
        {
            SetField(ref _filterString, value);
            ApplyFilter();
        }
    }

    public ObservableCollection<ProjectInfo> Projects
    {
        get => _projects;
        set => SetField(ref _projects, value);
    }

    public ReportInfo SelectedReport
    {
        get => _selectedReport;
        set => SetField(ref _selectedReport, value);
    }

    public ObservableCollection<ReportGroup> ReportGroups
    {
        get => _reportGroups;
        set => SetField(ref _reportGroups, value);
    }

    public void SetProjectsList(List<ProjectInfo> projects) =>
        Projects = new ObservableCollection<ProjectInfo>(projects);

    public void SetReportsList(List<ReportInfo> reports)
    {
        ReportGroups = [];
        foreach (var project in Projects)
        {
            var reportsInfo = reports
                .Where(r => r.ProjectId == project.Id.ToString());
            if (!reportsInfo.Any()) continue;

            ReportGroups.Add(new ReportGroup
            {
                ProjectName = project.Name,
                Reports = new ObservableCollection<ReportInfo>(reportsInfo.ToList())
            });
        }
    }

    private void ApplyFilter()
    {
        var reports = CollectionViewSource.GetDefaultView(ReportGroups);
        if (FilterString == "") reports.Filter = null;
        else
        {
            var projectNames = Projects.Where(p => p.Name.ToLower().Contains(FilterString.ToLower())).Select(p => p.Name.ToLower());
            reports.Filter = r => r is ReportGroup report && projectNames.Contains(report.ProjectName.ToLower());
        }
    }

    private void ClearFilter()
    {
        FilterString = "";
    }
}