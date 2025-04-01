using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Versions.Comparer;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration;

public class Integration
{
    private static ReportViewController _reportViewController;

    public static bool IsSyncOn => SdlTradosStudio.Application.GetAction<SyncReportProjectOn>().Checked;
    public static ReportManager ReportManager { get; } = new();
    private static EditorEventListener EditorEventListener { get; } = new();

    private static ProjectsController ProjectsController =>
        SdlTradosStudio.Application.GetController<ProjectsController>();

    private static ReportFolderExplorer ReportFolderExplorer { get; set; }

    private static ReportViewController ReportViewController => _reportViewController ??=
        SdlTradosStudio.Application.GetController<ReportViewController>();

    private static StudioActionExecutor StudioActionExecutor { get; } = new();

    public static async Task ChangeStatusOfVisibleSegments(string newStatus)
    {
        var visibleSegments = await ReportViewController.GetAllSegmentsCurrentlyVisible();
        if (IsSyncOn)
            foreach (var visibleSegment in visibleSegments)
                StudioActionExecutor.ChangeStatusOfSegment(newStatus, visibleSegment.SegmentId, visibleSegment.FileId,
                    visibleSegment.ProjectId);
        else
            foreach (var visibleSegment in visibleSegments)
                await ReportViewController.UpdateStatus(newStatus, visibleSegment.SegmentId, visibleSegment.FileId);

        await SaveReport();
    }

    public static async Task EditReportFolderList()
    {
        ReportViewController.ShowLoadingScreen();
        ReportFolderExplorer = new ReportFolderExplorer { ReportFolders = new ObservableCollection<string>(ReportManager.ReportFolders) };

        var result = ReportFolderExplorer.ShowDialog();
        if (!result) return;

        ReportManager.ReportFolders = ReportFolderExplorer.ReportFolders.ToList();
        var reports = await ReportManager.GetReports();

        ReportViewController.RefreshReportsList(reports);
        ReportViewController.HideLoadingScreen();
    }

    public static async Task ExportReport()
    {
        try
        {
            var selectedReport = ReportViewController.GetSelectedReport();
            if (selectedReport == null) throw new ArgumentNullException();

            var projectId = selectedReport.ProjectId;
            var projectName = ProjectsController.GetAllProjects()
                .FirstOrDefault(p => p.GetProjectInfo().Id.ToString() == projectId)?.GetProjectInfo().Name;

            var excelReportProposedName = Path.GetFileNameWithoutExtension(selectedReport.ReportName);
            var report = await ReportViewController.GetNonInteractiveReport();
            ReportManager.ExportReport(report, projectName, excelReportProposedName);
        }
        catch (ArgumentException _)
        {
            ErrorHandler.ShowError("Please select a report");
        }
        catch (InvalidOperationException _)
        {
            ErrorHandler.ShowError("Please select a report with at least one change");
        }
        catch (Exception ex)
        {
            ErrorHandler.ShowError(ex);
        }
    }

    public static void FilterSegments(SegmentFilter segmentFilter) =>
        ReportViewController.ToggleFilter(segmentFilter);

    public static async Task HandleReportRequest(string jsonMessage)
    {
        try
        {
            var messageObject = JObject.Parse(jsonMessage);
            var action = messageObject["action"]?.ToString();

            if (!IsSyncOn && action != "navigate")
            {
                await ReportViewController.HandleReportRequestWithoutSync(messageObject);
            }
            else HandleReportRequest(messageObject);

            await SaveReport();
        }
        catch (Exception ex)
        {
            ErrorHandler.ShowError(ex);
        }
    }

    public static void HandleReportRequest(JObject messageObject)
    {
        var message = SyncMessage.Create(messageObject);
        switch (message)
        {
            case NavigateMessage navigateMessage:
                StudioActionExecutor.NavigateToSegment(navigateMessage.SegmentId, navigateMessage.FileId, navigateMessage.ProjectId);
                break;

            case UpdateStatusMessage updateStatusMessage:
                StudioActionExecutor.ChangeStatusOfSegment(updateStatusMessage.NewStatus, updateStatusMessage.SegmentId, updateStatusMessage.FileId, updateStatusMessage.ProjectId);
                break;

            case UpdateCommentsMessage updateCommentsMessage:
                StudioActionExecutor.AddComment(updateCommentsMessage.Comment, updateCommentsMessage.Severity, updateCommentsMessage.SegmentId, updateCommentsMessage.FileId, updateCommentsMessage.ProjectId);
                break;
        }
    }

    public static void Initialize()
    {
        SetUpReportExplorer();
    }

    public static void InitializeReportFilter(string projectId)
    {
        if (projectId == null) return;

        var reportFilter = SdlTradosStudio.Application.GetController<ReportViewFilterController>();
        reportFilter.Activate();

        var ranges = FuzzyRange.GetFuzzyRangesFromProjectId(projectId);
        ranges.Remove("All");
        reportFilter.InitializeReportFilter(ranges);
    }

    public static void OpenReportBackupFolder()
    {
        var selectedReport = ReportViewController.GetSelectedReport();
        ReportManager.OpenReportBackupFolder(selectedReport);
    }

    public static void OpenReportFolder()
    {
        var selectedReport = ReportViewController.GetSelectedReport();
        ReportManager.OpenReportFolder(selectedReport?.ReportPath);
    }

    public static async Task SaveReport()
    {
        var reportFromMemory = await ReportViewController.GetLoadedReport();
        var selectedReport = ReportViewController.GetSelectedReport();

        ReportManager.SaveReport(reportFromMemory, selectedReport.ReportPath);
    }

    public static async Task SetUpReportExplorer(bool displayLoadingScreen = true)
    {
        if (displayLoadingScreen) ReportViewController.ShowLoadingScreen();
        var reports = await ReportManager.GetReports();
        var projects = ProjectsController.GetAllProjects().Select(p => p.GetProjectInfo()).ToList();
        ReportViewController.RefreshLists(projects, reports);
        if (displayLoadingScreen) ReportViewController.HideLoadingScreen();
    }

    public static async Task ShowLatestReport()
    {
        var dialogResult = MessageBox.Show(
            "Would you like to navigate to the newly created report? This will stop the current project-report synchronization.",
            "Report Created",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        if (dialogResult == DialogResult.No) return;

        ReportViewController.Activate();
        ReportViewController.ShowLoadingScreen();

        //await Task.Delay(100);

        await ToggleReportProjectSync(false);
        await SetUpReportExplorer(false);

        var reports = await ReportManager.GetReports();

        await ReportViewController.SelectLatestReport(reports.FirstOrDefault());
        ReportViewController.HideLoadingScreen();
    }

    public static void ShowReportsView() => ReportViewController.Activate();

    public static async Task ToggleReportProjectSync(bool syncEnabled)
    {
        if (ReportViewController.GetSelectedReport() == null) return;

        if (syncEnabled)
        {
            await ReportManager.BackUpReport(ReportViewController.GetSelectedReport());
            await RetroSync();
            ConnectEditorListener();
        }
        else
            DisconnectEditorListener();

        ReportViewController.ToggleReportExplorer(!syncEnabled);
    }

    public static void ToggleSyncRibbon(bool state) =>
        SdlTradosStudio.Application.GetAction<SyncReportProjectOn>().Enabled = state;

    public static async Task UndockReportViewer() => await ReportViewController.UndockReportViewer();

    private static void ConnectEditorListener()
    {
        EditorEventListener.StartListening();
        EditorEventListener.CommentsChanged += EditorEventListener_CommentsChanged;
        EditorEventListener.StatusChanged += EditorEventListener_StatusChanged;
    }

    private static void DisconnectEditorListener()
    {
        EditorEventListener.StopListening();
        EditorEventListener.CommentsChanged -= EditorEventListener_CommentsChanged;
        EditorEventListener.StatusChanged -= EditorEventListener_StatusChanged;
    }

    private static async void EditorEventListener_CommentsChanged(List<CommentInfo> comments, string segmentId,
        string fileId)
    {
        try
        {
            await ReportViewController.UpdateComments(comments, segmentId, fileId);
            await SaveReport();
        }
        catch (Exception e)
        {
            ErrorHandler.ShowError(e);
        }
    }

    private static async void EditorEventListener_StatusChanged(string newStatus, string segmentId, string fileId)
    {
        try
        {
            await ReportViewController.UpdateStatus(newStatus, segmentId, fileId);
            await SaveReport();
        }
        catch (Exception e)
        {
            ErrorHandler.ShowError(e);
        }
    }

    private static async Task RetroSync()
    {
        await RetroSyncComments();
        await RetroSyncStatuses();
    }

    private static async Task RetroSyncComments()
    {
        var commentTextComparer = new CommentTextComparer();
        var htmlComments = await ReportViewController.GetAllComments();
        var htmlFileGroupedComments = htmlComments.GroupBy(c => c.FileId);
        foreach (var htmlFileComments in htmlFileGroupedComments)
        {
            foreach (var htmlSegmentComments in htmlFileComments)
            {
                var editorComments = StudioActionExecutor.GetEditorComments(htmlSegmentComments.SegmentId, htmlSegmentComments.FileId,
                    htmlSegmentComments.ProjectId);

                var notInEditorComments = htmlSegmentComments.Comments.Except(editorComments, commentTextComparer);

                StudioActionExecutor.AddCommentsToEditorSegment(notInEditorComments, htmlSegmentComments.SegmentId, htmlSegmentComments.FileId,
                    htmlSegmentComments.ProjectId);

                var notInHtmlReportComments =
                    editorComments.Except(htmlSegmentComments.Comments, commentTextComparer);

                htmlSegmentComments.Comments = htmlSegmentComments.Comments
                    .Union(notInHtmlReportComments, commentTextComparer).ToList();

                await ReportViewController.UpdateComments(htmlSegmentComments.Comments, htmlSegmentComments.SegmentId,
                    htmlSegmentComments.FileId);
            }
        }
    }

    private static async Task RetroSyncStatuses()
    {
        var statuses = await ReportViewController.GetAllStatuses();
        StudioActionExecutor.ChangeStatusOfEditorSegments(statuses);
    }
}