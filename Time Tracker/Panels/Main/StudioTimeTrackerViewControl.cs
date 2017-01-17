using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Main
{
    public partial class StudioTimeTrackerViewControl : UserControl
    {
        public StudioTimeTrackerViewControl()
        {
            InitializeComponent();


            olvColumn_client_name.ImageGetter = x => "client";


            olvColumn_activity_status.ImageGetter =
                x => ((Structures.TrackerProjectActivity) x).Status == "New" ? "question_blue" : "tick";


            olvColumn_billable.ImageGetter = x => ((Structures.TrackerProjectActivity) x).Billable ? "vyes" : "vno";

            olvColumn_invoiced.ImageGetter = x => ((Structures.TrackerProjectActivity) x).Invoiced ? "iyes" : "ino";

          


            olvColumn_project.ImageGetter =
                x =>
                    ((Structures.TrackerProjectActivity) x).TrackerProjectStatus == "In progress"
                        ? "flag_blue"
                        : "flag_green";


            olvColumn_activity_name.ImageGetter = x => "calendar";
        }

        private StudioTimeTrackerViewController _controller { get; set; }
        public StudioTimeTrackerViewController Controller
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

        private void CollapseAllGroups()
        {
            if (objectListView1.OLVGroups != null)
            {
                foreach (var t in objectListView1.OLVGroups)
                {
                    t.Collapsed = true;
                }
            }
           
        }

        private void ExpandAllGroups()
        {
            if (objectListView1.OLVGroups != null)
                foreach (var t in objectListView1.OLVGroups)
                {
                    t.Collapsed = false;
                }
        }

        private void linkLabel_expand_all_groups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ExpandAllGroups();
            objectListView1.Select();
        }

        private void linkLabel_collapse_all_groups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CollapseAllGroups();
            objectListView1.Select();
        }

        private void linkLabel_select_all_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (OLVListItem itmx in objectListView1.Items)
            {
                itmx.Selected = true;
            }
            objectListView1.Select();
        }

        private void linkLabel_unselect_all_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            foreach (OLVListItem itmx in objectListView1.Items)
            {
                itmx.Selected = false;
            }
            objectListView1.Select();
        }

   

      
    }
}
