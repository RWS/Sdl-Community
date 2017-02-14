using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Document_Reports;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Report;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Qualitivity.Panels.Activity_Records
{
    public partial class QualitivityViewActivityRecordsControl : UserControl
    {
    
        QualitivityViewActivityRecordsController _controller { get; set; }
        public QualitivityViewActivityRecordsController Controller
        {
            get
            {
                return _controller;
            }
            set
            {

                _controller = value;
            }
        }

        public QualitivityViewActivityRecordsControl()
        {
            InitializeComponent();

            webBrowser1.Dock = DockStyle.Fill;
            webBrowser2.Dock = DockStyle.Fill;

            webBrowser2.BringToFront();
              
        }

        private Dictionary<int, List<DocumentActivity>> DocumentActivityDict { get; set; }
        internal List<Activity> Activities { get; set; }

        public void UpdateActivityReport()
        {
            if (QualitivityViewTrackChangesController.NavigationTreeView.SelectedNode != null
                && QualitivityViewTrackChangesController.ObjectListView.SelectedObjects.Count > 0)
            {

                var isSingleProject = true;
                var projectId = -1;

                DocumentActivityDict = new Dictionary<int, List<DocumentActivity>>();
                foreach (var activity in Activities)
                {
                    if (projectId == -1)
                        projectId = activity.ProjectId;
                    else if (projectId != activity.ProjectId)
                    {
                        isSingleProject = false;
                        break;
                    }
                    DocumentActivityDict.Add(activity.Id, Helper.GetDocumentActivityObjects(activity));
                }
                if (isSingleProject)
                {

                    var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Project.Activity.xml.html");

                    var reports = new Processor();
                    webBrowser1.BringToFront();


                    var project = Helper.GetProjectFromId(Activities[0].ProjectId);
                    var cpi = Helper.GetClientFromId(project.CompanyProfileId);


                    var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Project.Activity.xml");
                    reports.CreateActivityReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, project
                        , Activities, cpi, Tracked.Settings.UserProfile, DocumentActivityDict);



                    webBrowser1.Navigate(new Uri(Path.Combine("file://", htmlFileFullPath)));
                }
                else
                {
                    webBrowser2.BringToFront();
                }
            }
            else
            {
        
                webBrowser2.BringToFront();
            }

        }
       

    }
}
