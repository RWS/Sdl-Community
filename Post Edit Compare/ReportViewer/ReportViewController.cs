using Sdl.Community.PostEdit.Versions.ReportViewer.Controls;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;

namespace Sdl.Community.PostEdit.Versions.ReportViewer
{
    [View(
        Id = "PostEdit.ReportViewer",
        Name = "Post-Edit Report Viewer",
        Description = "Post-Edit Report Viewer",
        Icon = "PostEditVersions_Icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation)
    )]
    public class ReportViewController : AbstractViewController
    {
        private readonly Lazy<Report> _report = new();
        private readonly Lazy<ReportExplorer> _reportExplorer = new();

        protected override IUIControl GetContentControl()
        {
            return _report.Value;
        }

        protected override IUIControl GetExplorerBarControl()
        {
            _reportExplorer.Value.SelectedReportChanged += () =>
            {
                _report.Value.Navigate(_reportExplorer.Value.SelectedReport.ReportPath);
            };

            return _reportExplorer.Value;
        }

        protected override void Initialize(IViewContext context)
        {
        }
    }
}