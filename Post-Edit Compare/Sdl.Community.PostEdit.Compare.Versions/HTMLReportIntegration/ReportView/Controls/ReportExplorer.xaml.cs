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
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(ReportExplorer), new PropertyMetadata(false));

        public ReportExplorer() => InitializeComponent();

        public event Action SelectedReportChanged;

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public ReportInfo SelectedReport { get; set; }

        public void Dispose()
        {
            Root?.Dispose();
        }

        public void HideLoadingScreen() => IsLoading = false;

        public void ShowLoadingScreen() => IsLoading = true;

        public void ToggleOnOff(bool enabled) => IsEnabled = enabled;

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedReport = e.NewValue as ReportInfo;
            SelectedReportChanged?.Invoke();
        }
    }
}