using Microsoft.Web.WebView2.Core;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.Interface;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>
    /// Interaction logic for ReportViewerWindow.xaml
    /// </summary>
    public partial class ReportViewerWindow : IReportViewer
    {
        public ReportViewerWindow()
        {
            ReportViewer = new ReportViewer();
            InitializeComponent();
            Content = ReportViewer;
        }

        public event Action Hidden;

        public event Action<object, CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived
        {
            add => ReportViewer.WebMessageReceived += value;
            remove => ReportViewer.WebMessageReceived -= value;
        }

        private ReportViewer ReportViewer { get; }

        public void Dispose()
        {
        }

        public void Dock()
        {
            Hide();
            Hidden?.Invoke();
        }

        public async Task<List<SegmentComments>> GetAllComments()
        {
            return await ReportViewer.GetAllComments();
        }

        public async Task<List<ReportSegment>> GetAllSegments()
        {
            return await ReportViewer.GetAllSegments();
        }

        public async Task<string> GetLoadedReport()
        {
            return await ReportViewer.GetLoadedReport();
        }

        public async Task<string> GetNonInteractiveReport()
        {
            return await ReportViewer.GetNonInteractiveReport();
        }

        public async Task<string> GetProjectId()
        {
            return await ReportViewer.GetProjectId();
        }

        public async Task Navigate(string reportPath) => await ReportViewer.Navigate(reportPath);

        public async Task ShowAllSegments()
        {
            await ReportViewer.ShowAllSegments();
        }

        public async Task ShowSegments(List<(string SegmentId, string FileId)> segmentList)
        {
            await ReportViewer.ShowSegments(segmentList);
        }

        public async Task UpdateComments(List<CommentInfo> comments, string segmentId, string fileId, AddReplace addReplace = AddReplace.Replace)
        {
            await ReportViewer.UpdateComments(comments, segmentId, fileId, addReplace);
        }

        public async Task UpdateStatus(string newStatus, string segmentId, string fileId)
        {
            await ReportViewer.UpdateStatus(newStatus, segmentId, fileId);
        }

        private void ReportViewerWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Dock();
        }
    }
}