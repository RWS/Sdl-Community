using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView
{
    [ViewPart(
        Id = "ReportViewFilterController",
        Name = "Report Filter",
        Description = "Report Filter",
        Icon = "Filter"
    )]
    [ViewPartLayout(typeof(ReportViewController), Dock = DockType.Right)]
    public class ReportViewFilterController : AbstractViewPartController
    {
        public ReportViewFilter ReportViewFilter { get; set; }

        protected override IUIControl GetContentControl() => ReportViewFilter;

        protected override void Initialize()
        {
            ReportViewFilter = new ReportViewFilter();
            ReportViewFilter.FilterChanged += ReportViewFilter_FilterChanged;
        }

        private void ReportViewFilter_FilterChanged(SegmentFilter segmentFilter)
        {
            Integration.FilterSegments(segmentFilter);
        }
    }
}