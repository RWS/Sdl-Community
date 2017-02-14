using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.TM.Database;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Qualitivity.Panels.Document_Reports
{
    

    [ViewPart(
        Id = "QualitivityViewTrackChangesController",
        Name = "Activity Documents",
        Description = "Activity Documents",
        Icon = "QualitivityTrackChangesController_Icon"
        )]
    [ViewPartLayout(Dock = DockType.Bottom, ZIndex = 1, LocationByType = typeof(QualitivityViewController))]
    public class QualitivityViewTrackChangesController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public static readonly Lazy<QualitivityViewTrackChangesControl> Control = new Lazy<QualitivityViewTrackChangesControl>(() => new QualitivityViewTrackChangesControl());

        public static TreeView NavigationTreeView { get; set; }
        public static ObjectListView ObjectListView { get; set; }


        public static void UpdateReportsArea(List<DocumentActivity> documentActivities, Activity activity)
        {
            var query = new Query();
            ActivityReports activityReports = null;

            var reportOverview = string.Empty;
            var reportMetrics = string.Empty;

            var existsReportOverview = false;
            var existsReportMetrics = false;

            if (activity != null && activity.Id > -1)
            {
                activityReports = query.GetActivityReports(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, activity.Id);
                reportOverview = activityReports.ReportOverview;
                reportMetrics = activityReports.ReportMetrics;

                existsReportOverview = reportOverview.Trim() != string.Empty ? true : false;
                existsReportMetrics = reportMetrics.Trim() != string.Empty ? true : false;
            }
           

           
            Control.Value.Activity = activity;           
            Control.Value.DocumentActivities = documentActivities;
         
            Control.Value.UpdateDocumentOverview(ref reportOverview);
            Control.Value.UpdateQualityMetricsReport(ref reportMetrics);           
            Control.Value.UpdateDocumentRecords();

            if (activity != null && activityReports != null && (!existsReportOverview || !existsReportMetrics))
            {
                activityReports.ProjectActivityId = activity.Id;
                activityReports.ReportOverview = reportOverview;
                activityReports.ReportMetrics = reportMetrics;
                //update server
                activityReports.Id = query.SaveActivityReports(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, activityReports);
            }
          

        }

        
    }
}