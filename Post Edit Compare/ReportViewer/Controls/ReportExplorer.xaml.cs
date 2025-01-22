using Sdl.Community.PostEdit.Versions.ReportViewer.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.ReportViewer.Controls
{
    /// <summary>
    /// Interaction logic for ReportExplorer.xaml
    /// </summary>
    public partial class ReportExplorer : UserControl, IUIControl
    {
        public static readonly DependencyProperty ReportsProperty = DependencyProperty.Register(nameof(Reports), typeof(List<ReportInfo>), typeof(ReportExplorer), new PropertyMetadata(default(List<ReportInfo>)));

        public ReportExplorer()
        {
            InitializeComponent();
            InitializeProperties();
        }

        public event Action SelectedReportChanged;

        public List<ReportInfo> Reports
        {
            get => (List<ReportInfo>)GetValue(ReportsProperty);
            set => SetValue(ReportsProperty, value);
        }

        public ReportInfo SelectedReport { get; set; }

        public void Dispose()
        {
            Root?.Dispose();
        }

        private void InitializeProperties()
        {
            var myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var reportList = Directory.GetFiles(Path.Combine(myDocPath, "PostEdit.Compare", "Reports"), "*.html", SearchOption.AllDirectories).ToList();

            Reports = [];

            foreach (var report in reportList)
            {
                var directoryName = new DirectoryInfo(Path.GetDirectoryName(report) ?? string.Empty).Name;
                Reports.Add(new ReportInfo
                {
                    ReportName = $@"{directoryName}\\{Path.GetFileName(report)}",
                    ReportPath = report
                });
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedReportChanged?.Invoke();
        }
    }
}