using System;

namespace Sdl.Community.Parser
{
    [Serializable]
    public class Tag : SegmentSection
    {

        public enum Type
        {
            End,
            LockedContent,
            Standalone,
            Start,
            TextPlaceholder,
            Undefined,
            UnmatchedEnd,
            UnmatchedStart
        }

       
        public string AlignmentAnchor { get; set; }     
        public string Anchor { get; set; }     
        public string TagId { get; set; }      
        public string TextEquivalent { get; set; }        
        public Type SectionType { get; set; }
        public RevisionMarker Revision { get; set; }

        public Tag()
        {
            AlignmentAnchor = string.Empty;
            Anchor = string.Empty;
            TagId = string.Empty;
            TextEquivalent = string.Empty;
            SectionType = Type.Undefined;
            Revision = null;
        }

    }
}
