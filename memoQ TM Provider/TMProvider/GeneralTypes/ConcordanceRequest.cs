using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using AutoMapper.Mappers;

namespace TMProvider
{
    public enum Column
    {
        Prefix = 0,
        Middle = 1,
        Suffix = 2,
        Source = 3,
        Target = 4,
        Meta = 5,
        None = 6
    }
    public enum FirstLast
    {
        First = 0,
        Last = 1
    }
    public enum Meta
    {
        User = 0,
        ModifTime = 1
    }

    /// <summary>
    /// Settings for concordance lookup
    /// </summary>
    public class ConcordanceRequest
    {
        public int AbsScoreLimit { get; private set; }

        public bool Ascending { get; private set; }

        public bool CaseSensitive { get; private set; }

        public Column Column { get; private set; }

        /// <summary>
        /// Second criterion for sorting: prefix/first word or suffix/last word
        /// </summary>
        public FirstLast FirstLast { get; private set; }

        public bool IgnorePunctuation { get; private set; }

        public Meta Meta { get; private set; }

        public bool NumericEquivalence { get; private set; }

        public int RelScroreLimit { get; private set; }

        public bool ReverseLookup { get; private set; }

        /// <summary>
        /// Strings to filter the target (other) side.
        /// </summary>
        public List<string> TargetFilterStrings { get; private set; }

        public int TopScoreCount { get; private set; }

        public ConcordanceRequest(int absScoreLimit, int relScoreLimit, bool asc, bool caseSens, Column column, FirstLast firstLast, bool ignorePunct, Meta meta,
            bool numericEquivalence, bool reverseLookup, List<string> targetFilterStrings, int topScoreCount)
        {
            this.AbsScoreLimit = absScoreLimit;
            this.Ascending = asc;
            this.CaseSensitive = CaseSensitive;
            this.Column = column;
            this.FirstLast = firstLast;
            this.IgnorePunctuation = ignorePunct;
            this.Meta = meta;
            this.NumericEquivalence = numericEquivalence;
            this.RelScroreLimit = relScoreLimit;
            this.ReverseLookup = reverseLookup;
            this.TargetFilterStrings = targetFilterStrings;
            this.TopScoreCount = topScoreCount;
        }


        internal ConcordanceRequest(MemoQServerTypes.ConcordanceOptions memoQServerOptions)
        {
            Mapper.CreateMap<MemoQServerTypes.ConcordanceOptions, ConcordanceRequest>();
            Mapper.AssertConfigurationIsValid();
            Mapper.Map(memoQServerOptions, this);
        }
    }
}
