using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.ReportViewFilter;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView
{
    [ViewPart(
        Id = "ReportViewFilterController",
        Name = "Report Segment Filter",
        Description = "Report Segment Filter",
        Icon = "Filter"
    )]
    [ViewPartLayout(typeof(ReportViewController), Dock = DockType.Right)]
    public class ReportViewFilterController : AbstractViewPartController
    {
        public ReportViewFilter ReportViewFilter { get; set; }

        public void InitializeReportFilter(List<string> ranges) => ReportViewFilter.FuzzyBands = new ObservableCollection<string>(ranges);

        protected override IUIControl GetContentControl() => ReportViewFilter;

        protected override void Initialize()
        {
            ReportViewFilter = new ReportViewFilter();
            ReportViewFilter.FilterChanged += ReportViewFilter_FilterChanged;
        }

        private void ReportViewFilter_FilterChanged(SegmentFilter segmentFilter) =>
            Integration.FilterSegments(segmentFilter);

        public void SetFilteringResultCount(int matchingSegmentsCount, int segmentsCount)
        {
            ReportViewFilter.FilteredSegmentCount = matchingSegmentsCount;
            ReportViewFilter.SegmentCount = segmentsCount;
        }
    }
}