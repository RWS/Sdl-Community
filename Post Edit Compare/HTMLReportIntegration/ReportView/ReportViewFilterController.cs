using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView
{
    [ViewPart(
        Id = "ReportViewFilterController",
        Name = "Report Filter",
        Description = "Report Filter",
        Icon = ""
    )]
    [ViewPartLayout(typeof(ReportViewController), Dock = DockType.Right)]
    public class ReportViewFilterController : AbstractViewPartController
    {
        public ReportViewFilter ReportViewFilter { get; set; }

        public ReportViewFilterViewModel ReportViewFilterViewModel { get; set; }
        public static ReportViewFilterController Instance { get; set; }

        protected override IUIControl GetContentControl()
        {
            return ReportViewFilter;
        }

        protected override void Initialize()
        {
            ReportViewFilterViewModel = new ReportViewFilterViewModel();
            ReportViewFilter = new ReportViewFilter
            {
                DataContext = ReportViewFilterViewModel
            };

            Instance = this;
        }
    }
}