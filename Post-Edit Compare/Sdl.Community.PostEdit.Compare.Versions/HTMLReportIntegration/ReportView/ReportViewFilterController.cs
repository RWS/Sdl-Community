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
        private ReportViewFilter _reportViewFilter;

        /// <summary>
        /// Built on first access rather than only in <see cref="Initialize"/>: Studio does not create the
        /// view part until it is activated, and callers reach the filter while no report is selected -- which
        /// is not a path that activates it.
        /// </summary>
        public ReportViewFilter ReportViewFilter
        {
            get => _reportViewFilter ??= CreateReportViewFilter();
            set => _reportViewFilter = value;
        }

        public void InitializeReportFilter(List<string> ranges) => ReportViewFilter.FuzzyBands = new ObservableCollection<string>(ranges);

        protected override IUIControl GetContentControl() => ReportViewFilter;

        protected override void Initialize() => _ = ReportViewFilter;

        private ReportViewFilter CreateReportViewFilter()
        {
            var reportViewFilter = new ReportViewFilter();
            reportViewFilter.FilterChanged += ReportViewFilter_FilterChanged;
            reportViewFilter.ChangeStatusRequested += ReportViewFilter_ChangeStatusRequested;
            return reportViewFilter;
        }

        private void ReportViewFilter_ChangeStatusRequested(string newStatus) => Integration.ChangeStatusOfVisibleSegments(newStatus);

        private void ReportViewFilter_FilterChanged(SegmentFilter segmentFilter) =>
            Integration.FilterSegments(segmentFilter);

        public void SetFilteringResultCount(int matchingSegmentsCount, int segmentsCount)
        {
            ReportViewFilter.FilteredSegmentCount = matchingSegmentsCount;
            ReportViewFilter.SegmentCount = segmentsCount;
        }
    }
}