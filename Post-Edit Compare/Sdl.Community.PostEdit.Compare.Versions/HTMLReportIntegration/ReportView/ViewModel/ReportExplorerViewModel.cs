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

    public void SetProjectsList(List<ProjectInfo> projects) =>
        Projects = new ObservableCollection<ProjectInfo>(projects);

    public void SetReportsList(List<ReportInfo> reports) => Reports = new ObservableCollection<ReportInfo>(reports);

    private void ApplyFilter()
    {
        var reports = CollectionViewSource.GetDefaultView(Reports);

        if (FilterString == "") reports.Filter = null;
        else
        {
            var projectIds = Projects.Where(p => p.Name.Contains(FilterString)).Select(p => p.Id.ToString());
            reports.Filter = r => r is ReportInfo report && projectIds.Contains(report.ProjectId);
        }
    }

    private void ClearFilter()
    {
        FilterString = "";
    }
}