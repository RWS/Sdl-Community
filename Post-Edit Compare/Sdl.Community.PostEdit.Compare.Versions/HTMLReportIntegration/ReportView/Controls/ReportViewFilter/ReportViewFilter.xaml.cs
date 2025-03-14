using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Converters;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.ReportViewFilter
{
    /// <summary>|
    /// Interaction logic for ReportViewFilterControl.xaml
    /// </summary>
    public partial class ReportViewFilter : IUIControl
    {
        private ObservableCollection<string> _fuzzyBands;

        public ReportViewFilter()
        {
            InitializeComponent();

            MatchTypesListBox.ItemsSource = MatchTypes;
            StatusesListBox.ItemsSource = Statuses;
            FuzzyBandsListBox.ItemsSource = FuzzyBands;

            SelectedStatusesListBox.ItemsSource = SelectedStatuses;
            SelectedMatchTypesListBox.ItemsSource = SelectedMatchTypes;
            SelectedFuzzyBandsListBox.ItemsSource = SelectedFuzzyBands;

            //TODO For testing purposes
            FilteredSegmentCount = 3;
            SegmentCount = 11;
        }

        public event Action<SegmentFilter> FilterChanged;

        public ObservableCollection<string> FuzzyBands
        {
            get => _fuzzyBands;
            set
            {
                _fuzzyBands = value;
                FuzzyBandsListBox.ItemsSource = _fuzzyBands;
            }
        }

        public ObservableCollection<string> SelectedMatchTypes { get; } = [];
        private ObservableCollection<string> MatchTypes { get; } = ["CM", "PM", "AT", "ExactMatch", "FuzzyMatch", "NoMatch"];
        private ObservableCollection<string> SelectedFuzzyBands { get; } = [];
        private ObservableCollection<string> SelectedStatuses { get; } = [];
        private ObservableCollection<string> Statuses { get; } = ["NotTranslated", "Draft", "Translated", "TranslationRejected", "TranslationApproved", "SignOffRejected", "SignedOff"];

        public void Dispose() => Root?.Dispose();

        private void ApplyFilter()
        {
            //TODO Add functionality
        }

        private SegmentFilter GetSegmentFilter()
        {
            var selectedStatuses = SelectedStatusesListBox.SelectedItems;
            var selectedMatchTypes = SelectedMatchTypesListBox.SelectedItems;
            var selectedFuzzyPercentages = FuzzyBandsListBox.SelectedItems.Cast<string>().ToList();

            var statuses = EnumToListConverter.ConvertStringsToFlagEnum<Statuses>(selectedStatuses);
            var matchTypes = EnumToListConverter.ConvertStringsToFlagEnum<MatchTypes>(selectedMatchTypes);

            return new SegmentFilter
            {
                Statuses = statuses,
                MatchTypes = matchTypes,
                FuzzyPercentage = selectedFuzzyPercentages,
                Operator = (Operator)OperatorCombobox.SelectedIndex
            };
        }
    }
}