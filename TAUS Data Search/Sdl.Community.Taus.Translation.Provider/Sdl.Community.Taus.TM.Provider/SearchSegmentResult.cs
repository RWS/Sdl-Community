using System.Collections.Generic;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider
{
    public class SearchSegmentResult
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public AuthKey AuthKey { get; set; }
        public List<Segment.Segment> Segments { get; internal set; }

        public SearchSegmentResult()
        {
            AuthKey = new AuthKey();
            Status = string.Empty;
            Reason = string.Empty;
            Segments = new List<Segment.Segment>();
        }
    }
}
