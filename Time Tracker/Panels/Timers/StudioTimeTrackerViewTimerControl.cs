using System;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.Community.Studio.Time.Tracker.Tracking;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Timers
{
    public partial class StudioTimeTrackerViewTimerControl : UserControl
    {
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

       
        private static EditorController GetEditorController()
        {
            return SdlTradosStudio.Application.GetController<EditorController>();
        }

        private const int HandlerParent = 1;
        private Timer Timer4ActivityViewerArea { get; set; }

        public StudioTimeTrackerViewTimerControl()
        {
           
            InitializeComponent();


            Timer4ActivityViewerArea = new Timer {Interval = 1000};
            Timer4ActivityViewerArea.Tick += Timer4ActivityViewerArea_Tick;
            Timer4ActivityViewerArea.Start();
            
        }
      
       
        private void Timer4ActivityViewerArea_Tick(object sender, EventArgs e)
        {
            update_control_4ActivityViewerArea();
        }

        private void toolStripButton_start_Click(object sender, EventArgs e)
        {

            try
            {
                Tracked.HandlerPartent = HandlerParent;

                toolStripButton_start.Enabled = false;
                toolStripButton_pause.Enabled = true;
                toolStripButton_stop.Enabled = true;
                toolStripButton_delete.Enabled = true;

                toolStripButton_pause.Image = imageList2.Images["pause"];
                toolStripButton_pause.Text = @"Pause";
                toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
            }
            finally
            {

                TrackedActions.start_tracking(GetEditorController(), Timer4ActivityViewerArea, true);
            }
        }

        private void toolStripButton_pause_Click(object sender, EventArgs e)
        {

            Tracked.HandlerPartent = HandlerParent;

            if (Tracked.TrackingState == Tracked.TimerState.Paused)
            {
                toolStripButton_start.Enabled = false;
                toolStripButton_pause.Enabled = true;
                toolStripButton_stop.Enabled = true;
                toolStripButton_delete.Enabled = true;
                toolStripButton_pause.Image = imageList2.Images["pause"];
                toolStripButton_pause.Text = @"Pause";

                TrackedActions.pause_tracking(GetEditorController(), Timer4ActivityViewerArea);
            }
            else
            {
                toolStripButton_start.Enabled = false;
                toolStripButton_pause.Enabled = true;
                toolStripButton_stop.Enabled = true;
                toolStripButton_delete.Enabled = true;
                toolStripButton_pause.Image = imageList2.Images["unpause"];
                toolStripButton_pause.Text = @"Unpause";
                toolStripButton_pause.ToolTipText = @"Unpause Activity Tracking";

                TrackedActions.pause_tracking(GetEditorController(), Timer4ActivityViewerArea);
            }
 
        }

        private void toolStripButton_stop_Click(object sender, EventArgs e)
        {
            try
            {
                Tracked.HandlerPartent = HandlerParent;

                toolStripButton_start.Enabled = true;
                toolStripButton_pause.Enabled = false;
                toolStripButton_stop.Enabled = false;
                toolStripButton_delete.Enabled = false;
                toolStripButton_pause.Image = imageList2.Images["pause"];
                toolStripButton_pause.Text = @"Pause";
                toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
            }
            finally
            {

                TrackedActions.stop_tracking(GetEditorController(), Timer4ActivityViewerArea);
            }
        }

        private void toolStripButton_delete_Click(object sender, EventArgs e)
        {
            try
            {
                Tracked.HandlerPartent = HandlerParent;

                toolStripButton_start.Enabled = false;
                toolStripButton_pause.Enabled = false;
                toolStripButton_stop.Enabled = false;
                toolStripButton_delete.Enabled = false;
                toolStripButton_pause.Image = imageList2.Images["pause"];
                toolStripButton_pause.Text = @"Pause";
                toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
            }
            finally
            {
                TrackedActions.cancel_tracking(GetEditorController(), Timer4ActivityViewerArea);
            }
        }
      
        private void update_control_4ActivityViewerArea()
        {

            label_project.Text = Tracked.TrackerProjectName.Trim() != string.Empty ? Tracked.TrackerProjectName : "n/a";
            label_activity.Text = Tracked.TrackerActivityName.Trim() != string.Empty ? Tracked.TrackerActivityName : "n/a";
            label_type.Text = Tracked.TrackerActivityType.Trim() != string.Empty ? Tracked.TrackerActivityType : "n/a"; 
            label_elapsed_time.Text = string.Format("{0:00}:{1:00}:{2:00}",
                Tracked.TrackingTimer.Elapsed.Hours, Tracked.TrackingTimer.Elapsed.Minutes, Tracked.TrackingTimer.Elapsed.Seconds);

            if (!Tracked.TrackingIsDirtyC1) 
                return;

            try
            {
                Tracked.TrackingIsDirtyC1 = false;
                Timer4ActivityViewerArea.Stop();

                Cursor = Cursors.Default;

                switch (Tracked.TrackingState)
                {
                    case Tracked.TimerState.None:
                        toolStripButton_start.Enabled = true;
                        toolStripButton_pause.Enabled = false;
                        toolStripButton_stop.Enabled = false;
                        toolStripButton_delete.Enabled = false;

                        toolStripButton_pause.Image = imageList2.Images["pause"];
                        toolStripButton_pause.Text = @"Pause";
                        toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
                        break;
                    case Tracked.TimerState.Started:
                        toolStripButton_start.Enabled = false;
                        toolStripButton_pause.Enabled = true;
                        toolStripButton_stop.Enabled = true;
                        toolStripButton_delete.Enabled = true;
                        toolStripButton_pause.Image = imageList2.Images["pause"];
                        toolStripButton_pause.Text = @"Pause";
                        toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
                        break;
                    case Tracked.TimerState.Paused:
                        toolStripButton_start.Enabled = false;
                        toolStripButton_pause.Enabled = true;
                        toolStripButton_stop.Enabled = true;
                        toolStripButton_delete.Enabled = true;
                        toolStripButton_pause.Image = imageList2.Images["unpause"];
                        toolStripButton_pause.Text = @"Unpause";
                        toolStripButton_pause.ToolTipText = @"Unpause Activity Tracking";
                        break;
                    case Tracked.TimerState.Unpaused:
                        toolStripButton_start.Enabled = false;
                        toolStripButton_pause.Enabled = true;
                        toolStripButton_stop.Enabled = true;
                        toolStripButton_delete.Enabled = true;
                        toolStripButton_pause.Image = imageList2.Images["pause"];
                        toolStripButton_pause.Text = @"Pause";
                        break;
                    case Tracked.TimerState.Stopped:


                        toolStripButton_start.Enabled = true;
                        toolStripButton_pause.Enabled = false;
                        toolStripButton_stop.Enabled = false;
                        toolStripButton_delete.Enabled = false;
                        toolStripButton_pause.Image = imageList2.Images["pause"];
                        toolStripButton_pause.Text = @"Pause";
                        toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
                        break;
                    case Tracked.TimerState.Deleted:

                        toolStripButton_start.Enabled = false;
                        toolStripButton_pause.Enabled = false;
                        toolStripButton_stop.Enabled = false;
                        toolStripButton_delete.Enabled = false;
                        toolStripButton_pause.Image = imageList2.Images["pause"];
                        toolStripButton_pause.Text = @"Pause";
                        toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";
                        break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Timer4ActivityViewerArea.Start();
            }
        }

      
        
      

     



      

       

      
     

        
        
      

       

        



      
    }
}
