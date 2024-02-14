using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

namespace TMProvider
{
    public enum InlineTagStrictness
    {
        Strict = 0,
        Medium = 1, 
        Permissive = 2
    }

    /// <summary>
    /// Settings for TM lookup.
    /// </summary>
    public class LookupSegmentRequest
    {
        public bool AdjustFuzzyMatches { get; private set; }

        public InlineTagStrictness InlineTagStrictness { get; private set; }

        public int MatchThreshold { get; private set; }

        public bool OnlyBest { get; private set; }

        public bool OnlyUnambiguous { get; private set; }

        public bool ReverseLookup { get; private set; }

        public void SetReverseLookup(bool reverse)
        {
            this.ReverseLookup = reverse;
        }

        public LookupSegmentRequest(int matchThreshold, bool onlyBest, bool adjustFuzzyMatches, bool onlyUnambiguous, bool reverseLookup, InlineTagStrictness tagStrictness)
        {
            this.AdjustFuzzyMatches = adjustFuzzyMatches;
            this.InlineTagStrictness = InlineTagStrictness;
            this.MatchThreshold = matchThreshold;
            this.OnlyBest = onlyBest;
            this.OnlyUnambiguous = onlyUnambiguous;
            this.ReverseLookup = reverseLookup;
        }


        internal LookupSegmentRequest(MemoQServerTypes.LookupSegmentOptions memoQServerLookupOptions)
        {
            Mapper.CreateMap<MemoQServerTypes.LookupSegmentOptions, LookupSegmentRequest>();
            Mapper.AssertConfigurationIsValid();
            Mapper.Map(memoQServerLookupOptions, this);
        }
    }
}
