using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>|
    /// Interaction logic for ReportViewFilterControl.xaml
    /// </summary>
    public partial class ReportViewFilter : UserControl, IUIControl
    {
        public static readonly DependencyProperty FuzzyBandsProperty = DependencyProperty.Register(nameof(FuzzyBands),
            typeof(List<string>), typeof(ReportViewFilter), new PropertyMetadata(default(List<string>)));

        public ReportViewFilter() => InitializeComponent();

        public event Action<SegmentFilter> FilterChanged;

        public List<string> FuzzyBands
        {
            get => (List<string>)GetValue(FuzzyBandsProperty);
            set => SetValue(FuzzyBandsProperty, value);
        }

        public void Dispose() => Root?.Dispose();

        private void ApplyFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilterButton.IsEnabled = false;
            OperatorComboBox.IsEnabled = false;
            StatusesExpander.IsEnabled = false;
            MatchTypesExpander.IsEnabled = false;

            var segmentFilter = GetSegmentFilter();
            FilterChanged?.Invoke(segmentFilter);
        }

        private SegmentFilter GetSegmentFilter()
        {
            var selectedStatuses = StatusesListBox.SelectedItems;
            var selectedMatchTypes = MatchTypesListBox.SelectedItems;
            var selectedFuzzyPercentages = FuzzyMatchPercentageListBox.SelectedItems.Cast<string>().ToList();

            var statuses = EnumToListConverter.ConvertStringsToFlagEnum<Statuses>(selectedStatuses);
            var matchTypes = EnumToListConverter.ConvertStringsToFlagEnum<MatchTypes>(selectedMatchTypes);

            return new SegmentFilter
            {
                Statuses = statuses,
                MatchTypes = matchTypes,
                FuzzyPercentage = selectedFuzzyPercentages,
                Operator = (Operator)OperatorComboBox.SelectedIndex
            };
        }

        private void MatchTypesListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchTypesListBox.SelectedItems.Cast<object>().Any(selectedItem => selectedItem.ToString().Contains("Fuzzy")))
            {
                FuzzyMatchPercentageListBox.Visibility = Visibility.Visible;
                return;
            }

            FuzzyMatchPercentageListBox.Visibility = Visibility.Collapsed;
        }

        private void ResetFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilterButton.IsEnabled = true;
            OperatorComboBox.IsEnabled = true;
            StatusesExpander.IsEnabled = true;
            MatchTypesExpander.IsEnabled = true;

            StatusesListBox.SelectedItems.Clear();
            MatchTypesListBox.SelectedItems.Clear();

            FilterChanged?.Invoke(SegmentFilter.Empty);
        }
    }
}