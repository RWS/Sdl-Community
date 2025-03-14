using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.ReportViewFilter
{
    public partial class ReportViewFilter
    {
        public static readonly DependencyProperty FilteredSegmentCountProperty = DependencyProperty.Register(nameof(FilteredSegmentCount), typeof(int), typeof(ReportViewFilter), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty SegmentCountProperty = DependencyProperty.Register(nameof(SegmentCount), typeof(int), typeof(ReportViewFilter), new PropertyMetadata(default(int)));

        public int FilteredSegmentCount
        {
            get => (int)GetValue(FilteredSegmentCountProperty);
            set => SetValue(FilteredSegmentCountProperty, value);
        }

        public int SegmentCount
        {
            get => (int)GetValue(SegmentCountProperty);
            set => SetValue(SegmentCountProperty, value);
        }
    }
}