using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    public class ReportInfo : ViewModelBase
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }

        public string ProjectId { get; set; }
        public string ReportName { get; set; }
        public string ReportPath { get; set; }
    }
}