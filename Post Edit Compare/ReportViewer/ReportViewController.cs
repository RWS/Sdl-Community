using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Components;
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
        private Report Report { get; set; }
        private ReportExplorer ReportExplorer { get; set; }
        private ReportExplorerViewModel ReportExplorerViewModel { get; set; }
        

        protected override IUIControl GetContentControl()
        {
            return Report;
        }

        protected override IUIControl GetExplorerBarControl()
        {
            return ReportExplorer;
        }

        protected override void Initialize(IViewContext context)
        {
            InitializeControls();
            AttachEvents();
        }

        private void AttachEvents()
        {
            ReportExplorer.SelectedReportChanged += ExplorerOnSelectedReportChanged;
            Report.WebMessageReceived += WebView2Browser_WebMessageReceived;
        }

        private void ExplorerOnSelectedReportChanged()
        {
            Report.Navigate(ReportExplorerViewModel.SelectedReport?.ReportPath);
        }

        private void InitializeControls()
        {
            Report = new Report();

            ReportExplorerViewModel = new ReportExplorerViewModel();
            ReportExplorer = new ReportExplorer
            {
                DataContext = ReportExplorerViewModel
            };
        }

        private void WebView2Browser_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            ReportInteractionListener.HandleRequest(e);
        }
    }
}