using Microsoft.Web.WebView2.Core;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.Interface
{
    public interface IReportViewer : IUIControl
    {
        event Action<object, CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived;

        Visibility Visibility { get; set; }

        Task<List<ReportSegment>> GetAllSegments();

        Task<string> GetProjectId();

        Task Navigate(string reportPath);
        Task UpdateStatus(string newStatus, string segmentId, string fileId);
        Task UpdateComments(List<CommentInfo> comments, string segmentId, string fileId, AddReplace addReplace = AddReplace.Replace);
        Task ShowAllSegments();
        Task ShowSegments(List<(string SegmentId, string FileId)> toList);
        Task<string> GetNonInteractiveReport();
        Task<string> GetLoadedReport();
        Task<List<SegmentComments>> GetAllComments();
    }
}