using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>
    /// Interaction logic for ReportExplorer.xaml
    /// </summary>
    public partial class ReportExplorer : IUIControl
    {
        public ReportExplorer()
        {
            InitializeComponent();
        }

        public event Action SelectedReportChanged;

        public ReportInfo SelectedReport { get; set; }

        public void Dispose()
        {
            Root?.Dispose();
        }

        public void ToggleOnOff(bool enabled) => IsEnabled = enabled;

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedReport = e.NewValue as ReportInfo;
            SelectedReportChanged?.Invoke();
        }
    }
}