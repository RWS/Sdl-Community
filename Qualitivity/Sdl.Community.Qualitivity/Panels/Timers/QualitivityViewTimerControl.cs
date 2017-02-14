using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Panels.Timers
{
    public partial class QualitivityViewTimerControl : UserControl
    {
        private QualitivityViewController _controller { get; set; }
        public QualitivityViewController Controller
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


        public QualitivityViewTimerControl()
        {
           
            InitializeComponent();


            Timer4ActivityViewerArea = new Timer {Interval = 1000};
            Timer4ActivityViewerArea.Tick += Timer4ActivityViewerArea_Tick;
            Timer4ActivityViewerArea.Start();


            label_document.Text = @"none";
            label_type.Text = @"none";
            label_elapsed_time_document.Text = string.Format("{0:00}:{1:00}:{2:00}",
              0, 0, 0);

            label_elapsed_time.Text = string.Format("{0:00}:{1:00}:{2:00}",
              0, 0, 0);
            
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

            update_timer_controls();

            if (Tracked.TrackingIsDirtyC1)
            {
                update_button_controls();
            }    
        }


        private void update_button_controls()
        {
            try
            {
        
                    Tracked.TrackingIsDirtyC1 = false;

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

                            label_elapsed_idle_time.Visible = false;
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

                            label_elapsed_idle_time.Visible = false;
                            break;
                        case Tracked.TimerState.Stopped:
                            toolStripButton_start.Enabled = true;
                            toolStripButton_pause.Enabled = false;
                            toolStripButton_stop.Enabled = false;
                            toolStripButton_delete.Enabled = false;
                            toolStripButton_pause.Image = imageList2.Images["pause"];
                            toolStripButton_pause.Text = @"Pause";
                            toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";


                            label_elapsed_idle_time.Visible = false;
                            break;
                        case Tracked.TimerState.Deleted:

                            toolStripButton_start.Enabled = false;
                            toolStripButton_pause.Enabled = false;
                            toolStripButton_stop.Enabled = false;
                            toolStripButton_delete.Enabled = false;
                            toolStripButton_pause.Image = imageList2.Images["pause"];
                            toolStripButton_pause.Text = @"Pause";
                            toolStripButton_pause.ToolTipText = @"Pause Activity Tracking";



                            label_elapsed_idle_time.Visible = false;
                            break;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void update_timer_controls()
        {
            if (Tracked.ActiveDocument != null)
            {
                var projectFile = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (projectFile == null || !Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
                    return;
                var firstOrDefault = Tracked.ActiveDocument.Files.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var trackedDocuments = Tracked.DictCacheDocumentItems[firstOrDefault.Id.ToString()];

                    label_document.Text = trackedDocuments.ActiveDocument.Name;
                    label_type.Text = trackedDocuments.ActivityType;

                    label_elapsed_time.Text = string.Format("{0:00}:{1:00}:{2:00}",
                        Tracked.TrackingTimer.Elapsed.Hours, Tracked.TrackingTimer.Elapsed.Minutes,
                        Tracked.TrackingTimer.Elapsed.Seconds);


                    label_elapsed_time_document.Text = string.Format("{0:00}:{1:00}:{2:00}",
                        trackedDocuments.ActiveDocument.DocumentTimer.Elapsed.Hours,
                        trackedDocuments.ActiveDocument.DocumentTimer.Elapsed.Minutes,
                        trackedDocuments.ActiveDocument.DocumentTimer.Elapsed.Seconds);
                }


                label_elapsed_time_document.Visible = true;

                if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("idleTimeOut").Value))
                {
                    if ((Tracked.TrackingState != Tracked.TimerState.Started) || Tracked.ActiveDocument == null)
                        return;
                    if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("idleTimeOutShow").Value))
                    {

                        label_elapsed_idle_time.Visible = true;

                        var tsIdleElapsed = DateTime.Now.Subtract(Tracked.TrackerLastActivity);
                        label_elapsed_idle_time.Text = string.Format("Idle: {0:00}:{1:00}:{2:00}",
                            tsIdleElapsed.Hours, tsIdleElapsed.Minutes, tsIdleElapsed.Seconds);
                    }
                    else
                    {

                        label_elapsed_idle_time.Visible = false;
                    }
                }
                else
                {

                    label_elapsed_idle_time.Visible = false;
                }
            }
            else
            {
                label_document.Text = @"none";
                label_type.Text = @"none";
                label_elapsed_time_document.Text = string.Format("{0:00}:{1:00}:{2:00}",
                    0, 0, 0);

                label_elapsed_time.Text = string.Format("{0:00}:{1:00}:{2:00}",
                    0, 0, 0);

                label_elapsed_idle_time.Visible = false;
            }
        }

   

     
      

     



      

       

      
     

        
        
      

       

        



      
    }
}
