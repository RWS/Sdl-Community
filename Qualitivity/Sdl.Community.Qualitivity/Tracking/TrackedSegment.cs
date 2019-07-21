using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sdl.Community.Structures.Documents.Records;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Qualitivity.Tracking
{
    /// <summary>
    /// used to keep track of what is currently going on in the studio editor
    /// </summary>
    public class TrackedSegment
    {
	    public TrackedSegment()
	    {
		    CurrentDocumentId = string.Empty;
		    CurrentParagraphId = string.Empty;
		    CurrentSegmentId = string.Empty;
		    CurrentSegmentUniqueId = string.Empty;
		    CurrentISegmentPairProperties = null;
		    CurrentSegmentContentHasChanged = false;
		    CurrentSegmentSelected = null;

		    CurrentSegmentTimer = new Stopwatch();
		    CurrentSegmentOpened = DateTime.Now;
		    CurrentSegmentClosed = DateTime.Now;

		    CurrentTranslationKeyStrokeObjectCheck = false;
		    CurrentTranslationKeyStokeObjectId = string.Empty;

		    CurrentKeyStrokes = new List<KeyStroke>();
		    CurrentTargetSections = new List<ContentSection>();
		    CurrentTargetSelection = string.Empty;
	    }

		public string CurrentDocumentId { get; set; }

        public string CurrentParagraphId { get; set; }

        public string CurrentSegmentId { get; set; }

        public string CurrentSegmentUniqueId { get; set; }

        public ISegmentPairProperties CurrentISegmentPairProperties { get; set; }

        public bool CurrentSegmentContentHasChanged { get; set; }

        public ISegmentPair CurrentSegmentSelected { get; set; }

        public Stopwatch CurrentSegmentTimer { get; set; }

        public DateTime CurrentSegmentOpened { get; set; }

        public DateTime CurrentSegmentClosed { get; set; }

        public bool CurrentTranslationKeyStrokeObjectCheck { get; set; }

        public string CurrentTranslationKeyStokeObjectId { get; set; }

        public List<KeyStroke> CurrentKeyStrokes { get; set; }

        public List<ContentSection> CurrentTargetSections { get; set; }

        public string CurrentTargetSelection { get; set; }      
    }
}
