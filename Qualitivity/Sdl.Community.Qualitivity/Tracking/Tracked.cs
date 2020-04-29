using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Sdl.Community.Parser;
using Sdl.Community.Structures.Configuration;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Tracking
{
    public class Tracked
    {
        public enum TimerState
        {
            None,
            Started,
            Paused,
            Stopped,
            Deleted
        }

	    static Tracked()
	    {
		    DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();

		    WarningMessageDocumentsIgnoreActivityNotRunning = new Dictionary<string, bool>();

		    ActiveDocument = null;

		    DocumentSegmentPairs = new Dictionary<string, SegmentPair>();

		    TarckerCheckNewActivityId = -1;
		    TarckerCheckNewActivityAdded = false;

		    TarckerCheckNewProjectId = -1;
		    TarckerCheckNewProjectAdded = false;

		    HandlerPartent = 0;

		    Reset();
	    }

	    private static Form _studioWindow;

	    public static Form StudioWindow => _studioWindow ?? (_studioWindow = GetParentForm());

		public static bool EditorControllerIsActive { get; set; }

        public static FocusedDocumentContent EditorControllerFocusedDocumentContent { get; set; }

        public static Dictionary<string, bool> WarningMessageDocumentsIgnoreActivityNotRunning { get; set; }

        public static Settings Settings { get; set; }

        public static TrackingProjects TrackingProjects { get; set; }

        public static List<Currency> Currencies { get; set; }

        public static bool TrackingIsDirtyC0 { get; set; }

        public static bool TrackingIsDirtyC1 { get; set; }

        public static bool TrackingIsDirtyC2 { get; set; }

        public static int HandlerPartent { get; set; }

        public static Dictionary<string, TrackedDocuments> DictCacheDocumentItems { get; set; }

        public static Document ActiveDocument { get; set; }

        public static DateTime TrackerLastActivity { get; set; }

        public static Dictionary<string, SegmentPair> DocumentSegmentPairs { get; set; }

        public static int TarckerCheckNewActivityId { get; set; }

        public static bool TarckerCheckNewActivityAdded { get; set; }

        public static int TarckerCheckNewProjectId { get; set; }

        public static bool TarckerCheckNewProjectAdded { get; set; }

        #region  |  global stopwatch  |

        public static TimerState TrackingState { get; set; }
        public static Stopwatch TrackingTimer { get; set; }
        public static DateTime? TrackingStart { get; set; }
        public static DateTime? TrackingEnd { get; set; }
        public static Stopwatch TrackingPaused { get; set; }

        #endregion

        public static void Reset()
        {
            TrackerLastActivity = DateTime.Now;

            TrackingIsDirtyC0 = true;
            TrackingIsDirtyC1 = true;
            TrackingIsDirtyC2 = true;

            // global stopwatch  |
            TrackingState = TimerState.Stopped;
            TrackingTimer = new Stopwatch();
            TrackingStart = null;
            TrackingEnd = null;
            TrackingPaused = new Stopwatch();
        }

	    private static Form GetParentForm()
	    {
		    Form parentForm = null;
		    foreach (Control form in Application.OpenForms)
		    {
			    if (string.Compare(form.Name, "StudioWindowForm", StringComparison.InvariantCultureIgnoreCase) == 0)
			    {
				    parentForm = form as Form;
			    }
		    }

		    return parentForm;
	    }
	}
}
