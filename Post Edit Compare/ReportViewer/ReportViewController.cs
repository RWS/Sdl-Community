using Sdl.Community.PostEdit.Versions.ReportViewer.Controls;
using Sdl.Community.PostEdit.Versions.ReportViewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

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
        private ReportExplorer Explorer { get; set; }
        private Report Report { get; set; }

        private ReportExplorerViewModel ReportExplorerViewModel { get; set; }

        protected override IUIControl GetContentControl()
        {
            Report ??= new Report();
            return Report;
        }

        protected override IUIControl GetExplorerBarControl()
        {
            Explorer ??= new ReportExplorer { DataContext = ReportExplorerViewModel };

            Explorer.SelectedReportChanged += () =>
            {
                Report.Navigate(ReportExplorerViewModel.SelectedReport?.ReportPath);
            };

            return Explorer;
        }

        protected override void Initialize(IViewContext context)
        {
            ReportExplorerViewModel = new ReportExplorerViewModel();
        }
    }
}