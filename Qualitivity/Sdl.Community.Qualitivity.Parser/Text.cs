using System;

namespace Sdl.Community.Parser
{

    [Serializable]
    public class Text : SegmentSection
    {
        
        public string Value { get; set; }
        public RevisionMarker Revision { get; set; }

        public Text()
        {
            Value = string.Empty;
            Revision = null;
        }
    }
}
