using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static readonly DependencyProperty SegmentCountProperty = DependencyProperty.Register(nameof(SegmentCount), typeof(int), typeof(ReportViewFilter), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty FilteredSegmentCountProperty = DependencyProperty.Register(nameof(FilteredSegmentCount), typeof(int), typeof(ReportViewFilter), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty AppliedFiltersProperty = DependencyProperty.Register(nameof(AppliedFilters), typeof(List<string>), typeof(ReportViewFilter), new PropertyMetadata(default(List<string>)));
        public static readonly DependencyProperty StatusesProperty = DependencyProperty.Register(nameof(Statuses), typeof(ObservableCollection<string>), typeof(ReportViewFilter), new PropertyMetadata(default(ObservableCollection<string>)));
        public static readonly DependencyProperty MatchTypesProperty = DependencyProperty.Register(nameof(MatchTypes), typeof(ObservableCollection<string>), typeof(ReportViewFilter), new PropertyMetadata(default(ObservableCollection<string>)));
        public static readonly DependencyProperty SelectedFiltersProperty = DependencyProperty.Register(nameof(SelectedFilters), typeof(ObservableCollection<string>), typeof(ReportViewFilter), new PropertyMetadata(default(ObservableCollection<string>)));

        public static readonly DependencyProperty FuzzyBandsProperty = DependencyProperty.Register(nameof(FuzzyBands),
            typeof(List<string>), typeof(ReportViewFilter), new PropertyMetadata(default(List<string>)));

        public ReportViewFilter()
        {
            InitializeComponent();
            SelectedFilters = new ObservableCollection<string>();

            MatchTypes = ["CM", "PM", "AT", "ExactMatch", "FuzzyMatch", "NoMatch"];
            Statuses = ["NotTranslated", "Draft", "Translated", "TranslationRejected", "TranslationApproved", "SignOffRejected", "SignedOff"];

            FilteredSegmentCount = 3;
            SegmentCount = 11;
        }

        public event Action<SegmentFilter> FilterChanged;

        public List<string> FuzzyBands
        {
            get => (List<string>)GetValue(FuzzyBandsProperty);
            set => SetValue(FuzzyBandsProperty, value);
        }

        public ObservableCollection<string> SelectedFilters
        {
            get => (ObservableCollection<string>)GetValue(SelectedFiltersProperty);
            set => SetValue(SelectedFiltersProperty, value);
        }

        public ObservableCollection<string> MatchTypes
        {
            get => (ObservableCollection<string>)GetValue(MatchTypesProperty);
            set => SetValue(MatchTypesProperty, value);
        }

        public ObservableCollection<string> Statuses
        {
            get => (ObservableCollection<string>)GetValue(StatusesProperty);
            set => SetValue(StatusesProperty, value);
        }

        public List<string> AppliedFilters
        {
            get => (List<string>)GetValue(AppliedFiltersProperty);
            set => SetValue(AppliedFiltersProperty, value);
        }

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

        public void Dispose() => Root?.Dispose();

        private void ApplyFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            //ApplyFilterButton.IsEnabled = false;
            //OperatorComboBox.IsEnabled = false;
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
                //Operator = (Operator)OperatorComboBox.SelectedIndex
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
            //ApplyFilterButton.IsEnabled = true;
            //OperatorComboBox.IsEnabled = true;
            StatusesExpander.IsEnabled = true;
            MatchTypesExpander.IsEnabled = true;

            StatusesListBox.SelectedItems.Clear();
            MatchTypesListBox.SelectedItems.Clear();

            FilterChanged?.Invoke(SegmentFilter.Empty);
        }

        private void AddFiltersButton_Clicked(object sender, RoutedEventArgs e)
        {
            foreach (var selectedItem in MatchTypesListBox.SelectedItems)
            {
                SelectedFilters.Add(selectedItem.ToString());
            }
            foreach (var selectedItem in StatusesListBox.SelectedItems)
            {
                SelectedFilters.Add(selectedItem.ToString());
            }

            StatusesListBox.SelectedItems.Clear();
            MatchTypesListBox.SelectedItems.Clear();
        }

        private void RemoveAllFiltersButton_Clicked(object sender, RoutedEventArgs e)
        {
            SelectedFilters.Clear();
        }

        private void RemoveFilterButton_Clicked(object sender, RoutedEventArgs e)
        {
            foreach (var selectedItem in SelectedFilters_ListBox.SelectedItems)
            {
                SelectedFilters.Remove(selectedItem.ToString());
            }
        }
    }
}