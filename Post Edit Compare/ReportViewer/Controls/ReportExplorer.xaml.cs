using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.ReportViewer.Controls
{
    /// <summary>
    /// Interaction logic for ReportExplorer.xaml
    /// </summary>
    public partial class ReportExplorer : UserControl, IUIControl
    {
        public ReportExplorer()
        {
            InitializeComponent();
        }

        public event Action SelectedReportChanged;

        public void Dispose()
        {
            Root?.Dispose();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedReportChanged?.Invoke();
        }
    }
}