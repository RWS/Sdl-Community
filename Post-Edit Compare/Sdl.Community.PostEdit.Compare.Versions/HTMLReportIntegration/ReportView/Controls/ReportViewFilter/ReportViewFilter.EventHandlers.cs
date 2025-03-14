using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Extension;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.ReportViewFilter
{
    public partial class ReportViewFilter
    {
        private const string FuzzyBandsString = "FuzzyMatch";

        private void AddFiltersButton_Clicked(object sender, RoutedEventArgs e)
        {
            var highlightedItems = StatusesListBox.SelectedItems.Cast<string>().ToList();
            SelectedStatuses.AddRange(highlightedItems);
            Statuses.RemoveRange(highlightedItems);

            highlightedItems = MatchTypesListBox.SelectedItems.Cast<string>().ToList();
            SelectedMatchTypes.AddRange(highlightedItems);
            MatchTypes.RemoveRange(highlightedItems.Except([FuzzyBandsString]));

            if (highlightedItems.Contains(FuzzyBandsString))
            {
                var highlightedFuzzyBands = FuzzyBandsListBox.SelectedItems.Cast<string>().ToList();

                if (!highlightedFuzzyBands.Any())
                {
                    SelectedFuzzyBands.AddRange(FuzzyBands);
                    MatchTypes.Remove(FuzzyBandsString);
                    FuzzyBands.Clear();
                    return;
                }

                SelectedFuzzyBands.AddRange(highlightedFuzzyBands);
                FuzzyBands.RemoveRange(highlightedFuzzyBands);
                if (!FuzzyBands.Any()) MatchTypes.Remove(FuzzyBandsString);
            }
        }

        private void ApplyFilterButton_Clicked(object sender, RoutedEventArgs e)
        {
            List<string> expressions = [];

            if (SelectedStatuses.Any())
            {
                expressions.Add($"Status: ({string.Join("|", SelectedStatuses)})");
            }

            if (SelectedMatchTypes.Any())
            {
                expressions.Add($"Origin: ({string.Join("|", SelectedMatchTypes)})");
            }

            var separator = ((ComboBoxItem)OperatorCombobox.SelectedItem).Content.ToString();
            FilterExpression_TextBox.Text = string.Join($" {separator}{Environment.NewLine}", expressions);

            ApplyFilter();
        }

        private void ApplyFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            //ApplyFilterButton.IsEnabled = false;
            //OperatorComboBox.IsEnabled = false;
            StatusesExpander.IsEnabled = false;
            MatchTypesExpander.IsEnabled = false;

            var segmentFilter = GetSegmentFilter();
            FilterChanged?.Invoke(segmentFilter);
        }

        private void ClearFilterButton_Clicked(object sender, RoutedEventArgs e)
        {
            FilterExpression_TextBox.Text = "";
            ApplyFilter();
        }

        private void FuzzyBandsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FuzzyBandsListBox.SelectedItems.Cast<string>().Any())
                MatchTypesListBox.SelectedItems.Add(FuzzyBandsString);
        }

        private void MatchTypesListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FuzzyBandsListBox.Visibility = MatchTypesListBox.SelectedItems.Contains("FuzzyMatch")
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RemoveAllFiltersButton_Clicked(object sender, RoutedEventArgs e)
        {
            Statuses.AddRange(SelectedStatuses.ToList());
            MatchTypes.AddRange(SelectedMatchTypes.ToList());

            SelectedStatuses.Clear();
            SelectedMatchTypes.Clear();
        }

        private void RemoveFilterButton_Clicked(object sender, RoutedEventArgs e)
        {
            var highlightedItems = SelectedStatusesListBox.SelectedItems.Cast<string>().ToList();
            SelectedStatuses.RemoveRange(highlightedItems);
            Statuses.AddRange(highlightedItems);

            highlightedItems = SelectedMatchTypesListBox.SelectedItems.Cast<string>().ToList();
            SelectedMatchTypes.RemoveRange(highlightedItems.Except([FuzzyBandsString]));
            MatchTypes.AddRange(highlightedItems);

            if (highlightedItems.Contains(FuzzyBandsString))
            {
                var highlightedFuzzyBands = SelectedFuzzyBandsListBox.SelectedItems.Cast<string>().ToList();
                if (!highlightedFuzzyBands.Any())
                {
                    FuzzyBands.AddRange(SelectedFuzzyBands);
                    SelectedMatchTypes.Remove(FuzzyBandsString);
                    SelectedFuzzyBands.Clear();
                    return;
                }
                FuzzyBands.AddRange(highlightedFuzzyBands);
                SelectedFuzzyBands.RemoveRange(highlightedFuzzyBands);
                if (!SelectedFuzzyBands.Any()) SelectedMatchTypes.Remove(FuzzyBandsString);
            }
        }

        //TODO Fix fuzzy matches
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

        private void SelectedFuzzyBandsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedFuzzyBandsListBox.SelectedItems.Cast<string>().Any())
                SelectedMatchTypesListBox.SelectedItems.Add(FuzzyBandsString);
        }
    }
}