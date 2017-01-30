using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sdl.Community.Studio.Time.Tracker.Tracking
{
    public class Tracked
    {
        public static Structures.Settings Preferences { get; set; }
        public static List<Structures.Currency> Currencies { get; set; }


        public enum TimerState
        {
            None,
            Started,            
            Paused,
            Unpaused,
            Stopped,
            Deleted
        }


        public static string TarckerCheckNewActivityId { get; set; }
        public static bool TarckerCheckNewActivityAdded { get; set; }

        public static string TarckerCheckNewProjectId { get; set; }
        public static bool TarckerCheckNewProjectAdded { get; set; }

        public static bool TrackingIsDirtyC0 { get; set; }
        public static bool TrackingIsDirtyC1 { get; set; }
        public static bool TrackingIsDirtyC2 { get; set; }

        public static int HandlerPartent { get; set; }

        public static TimerState TrackingState { get; set; }
        public static Stopwatch TrackingTimer { get; set; }
        public static DateTime TrackingStart { get; set; }
        public static DateTime TrackingEnd { get; set; }
        public static Stopwatch TrackingPaused { get; set; }

        public static string TrackerDocumentId { get; set; }

        public static string TrackerProjectIdStudio { get; set; }
        public static string TrackerProjectNameStudio { get; set; }
        public static string TrackerProjectPathStudio { get; set; }
        public static string TrackerProjectId { get; set; }
        public static string TrackerProjectName { get; set; }
        public static string TrackerProjectPath { get; set; }

        public static string TrackerClientId { get; set; }
        public static string TrackerClientName { get; set; }

        public static string TrackerActivityName { get; set; }
        public static string TrackerActivityDescription { get; set; }
        public static string TrackerActivityType { get; set; }//Translation, Review, Sign-off

 
        static Tracked()
        {
            TarckerCheckNewActivityId = string.Empty;
            TarckerCheckNewActivityAdded = false;

            TarckerCheckNewProjectId = string.Empty;
            TarckerCheckNewProjectAdded = false;

            HandlerPartent = 0;


            Reset();

            TrackingIsDirtyC0 = true;
            TrackingIsDirtyC1 = true;
            TrackingIsDirtyC2 = true;

        }

        public static void Reset()
        {

                      
            TrackingState = TimerState.Stopped;
            TrackingTimer = new Stopwatch();
            TrackingStart = Structures.Common.DateNull;
            TrackingEnd = Structures.Common.DateNull;
            TrackingPaused = new Stopwatch();

           
            TrackerProjectIdStudio = string.Empty;
            TrackerProjectNameStudio = string.Empty;
            TrackerProjectId = string.Empty;
            TrackerProjectName = string.Empty;

            TrackerDocumentId = string.Empty;
            TrackerActivityName = string.Empty;
            TrackerActivityDescription = string.Empty;
            TrackerActivityType = string.Empty;

            TrackerClientId = string.Empty;
            TrackerClientName = string.Empty;   

        }
       
    }
}
