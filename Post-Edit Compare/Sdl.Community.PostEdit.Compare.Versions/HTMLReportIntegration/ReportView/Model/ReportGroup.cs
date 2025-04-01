using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;
using System.Collections.ObjectModel;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    public class ReportGroup : ViewModelBase
    {
        private bool _isExpanded;
        private ObservableCollection<ReportInfo> _reports = [];

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetField(ref _isExpanded, value);
        }

        public string ProjectName { get; set; }

        public ObservableCollection<ReportInfo> Reports
        {
            get => _reports;
            set => SetField(ref _reports, value);
        }
    }
}