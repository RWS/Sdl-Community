using Newtonsoft.Json;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Components;
using Sdl.Community.PostEdit.Versions.ReportViewer.Controls;
using Sdl.Community.PostEdit.Versions.ReportViewer.Model;
using Sdl.Community.PostEdit.Versions.ReportViewer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Windows;

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
        private ReportInteractionHandler ReportInteractionHandler { get; } = new();
        private StudioInteractionListener StudioInteractionListener { get; } = new();

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
            ReportExplorer.SyncTriggered += ReportExplorer_SyncTriggered;
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

        private void ReportExplorer_SyncTriggered(bool syncEnabled)
        {
            if (syncEnabled)
            {
                StudioInteractionListener.StartListening();
                StudioInteractionListener.CommentsChanged += StudioInteractionListener_CommentsChanged;
            }
            else
            {
                StudioInteractionListener.StopListening();
                StudioInteractionListener.CommentsChanged -= StudioInteractionListener_CommentsChanged;
            }
        }

        private void StudioInteractionListener_CommentsChanged(List<CommentInfo> comments, string segmentId)
        {
            var commentsJson = JsonConvert.SerializeObject(comments, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            var script = $"replaceCommentsForSegment('{segmentId}', {commentsJson});";

            try
            {
                Report.WebView2Browser.Dispatcher.Invoke(async () =>
                {
                    Report.WebView2Browser.ExecuteScriptAsync(script);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void WebView2Browser_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            ReportInteractionHandler.HandleHtmlInteraction(e);
        }
    }
}