using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMProvider
{
    public class QuerySegment
    {
        public string ContextID { get; set; }
        public string FollowingSegment { get; set; }
        public string PrecedingSegment { get; set; }
        public string Segment { get; set; }

        public QuerySegment(string contextID, string segment, string precedingSegment, string followingSegment)
        {
            this.ContextID = contextID;
            this.Segment = segment;
            this.FollowingSegment = followingSegment;
            this.PrecedingSegment = precedingSegment;
        }

 
        // the memoQ Server API uses the same class

    }
}
