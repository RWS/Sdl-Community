using System;
using System.Collections.Generic;

namespace Sdl.Community.Parser
{
    [Serializable]
    public class SegmentPair
    {
        public string Id { get; set; }
        public string ParagraphId { get; set; }
        public string ConfirmationLevel { get; set; }
   
        public bool IsLocked { get; set; }

        public string Source { get; set; }
        public string Target { get; set; }

        public List<SegmentSection> SourceSections { get; set; }
        public List<SegmentSection> TargetSections { get; set; }
        public List<SegmentSection> TargetModifiedSections { get; set; }
       
        public TranslationOrigin Origin { get; set; }
       
        public List<Comment> Comments { get; set; }


        public int SourceWords { get; set; }
        public int SourceChars { get; set; }
        public int SourceTags { get; set; }
        public int SourcePlaceables { get; set; }

        
        public List<string> RevisionMarkerUniqueIds { get; set; }

        public SegmentPair()
        {
            Id = string.Empty;
            ParagraphId = string.Empty;
            ConfirmationLevel = string.Empty;
          
            IsLocked = false;

            Source = string.Empty;
            Target = string.Empty;

            SourceSections = new List<SegmentSection>();
            TargetSections = new List<SegmentSection>();
            TargetModifiedSections = new List<SegmentSection>();
           
            Origin = new TranslationOrigin();
          
            Comments = new List<Comment>();


            SourceWords = 0;
            SourceChars = 0;
            SourceTags = 0;
            SourcePlaceables = 0;

            RevisionMarkerUniqueIds = new List<string>();
        }
    }
}
