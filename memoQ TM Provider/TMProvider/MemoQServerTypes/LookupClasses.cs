using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider.MemoQServerTypes
{
    internal class LookupSegmentsParam
    {
        public LookupSegmentsParam(QuerySegment[] segments, LookupSegmentRequest options)
        {
            this.Segments = segments;
            this.Options = options;
        }
        public QuerySegment[] Segments;
        public LookupSegmentRequest Options;
    }


    internal class LookupSegmentOptions
    {
        public LookupSegmentOptions(bool adjustFuzzy, InlineTagStrictness tagStrictness, int matchThreshold, bool onlyBest, bool onlyUnambiguous, bool reverseLookup)
        {
            this.AdjustFuzzyMatches = adjustFuzzy;
            this.InlineTagStrictness = tagStrictness;
            this.MatchThreshold = matchThreshold;
            this.OnlyBest = onlyBest;
            this.OnlyUnambiguous = onlyUnambiguous;
            this.ReverseLookup = reverseLookup;
        }

        public bool AdjustFuzzyMatches { get; set; }
        public InlineTagStrictness InlineTagStrictness { get; set; }
        public int MatchThreshold { get; set; }
        public bool OnlyBest { get; set; }
        public bool OnlyUnambiguous { get; set; }
        public bool ReverseLookup { get; set; }
    }


    internal class LookupSegmentsResult
    {
        public LookupSegmentsResult(TMHitsPerSegment[] result)
        {
            this.Result = result;
        }

        public TMHitsPerSegment[] Result;
    }
}
