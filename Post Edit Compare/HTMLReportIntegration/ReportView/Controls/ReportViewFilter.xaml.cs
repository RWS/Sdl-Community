using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>|
    /// Interaction logic for ReportViewFilterControl.xaml
    /// </summary>
    public partial class ReportViewFilter : UserControl, IUIControl
    {
        public ReportViewFilter() => InitializeComponent();

        public event Action<SegmentFilter> FilterChanged;

        public void Dispose() => Root?.Dispose();

        private void ApplyFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilterButton.IsEnabled = false;
            OperatorComboBox.IsEnabled = false;
            StatusesListBox.IsEnabled = false;
            MatchTypesListBox.IsEnabled = false;

            FilterChanged?.Invoke(GetSegmentFilter());
        }

        private SegmentFilter GetSegmentFilter()
        {
            var selectedStatuses = StatusesListBox.SelectedItems;
            var selectedMatchTypes = MatchTypesListBox.SelectedItems;

            var statuses = EnumToListConverter.ConvertStringsToFlagEnum<Statuses>(selectedStatuses);
            var matchTypes = EnumToListConverter.ConvertStringsToFlagEnum<MatchTypes>(selectedMatchTypes);

            return new SegmentFilter
            {
                Statuses = statuses,
                MatchTypes = matchTypes
            };
        }

        private void ResetFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilterButton.IsEnabled = true;
            OperatorComboBox.IsEnabled = true;
            StatusesListBox.IsEnabled = true;
            MatchTypesListBox.IsEnabled = true;

            StatusesListBox.SelectedItems.Clear();
            MatchTypesListBox.SelectedItems.Clear();

            FilterChanged?.Invoke(SegmentFilter.Empty);
        }
    }
}