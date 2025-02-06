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
        public ReportViewFilter()
        {
            InitializeComponent();
        }

        public event Action<SegmentFilter> ApplyFilterEvent;

        public void Dispose() => Root?.Dispose();

        private void ApplyFilterButton_OnClick(object sender, RoutedEventArgs e) => ApplyFilterButton.IsEnabled = false;

        private void ResetFilterButton_OnClick(object sender, RoutedEventArgs e) => ApplyFilterButton.IsEnabled = true;
    }
}