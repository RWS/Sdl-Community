using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider.MemoQServerTypes
{
    internal class ConcordanceParam
    {
        public ConcordanceParam(List<string> searchExpr, ConcordanceOptions options)
        {
            this.SearchExpression = searchExpr;
            this.Options = options;
        }

        public List<string> SearchExpression { get; set; }
        public ConcordanceOptions Options { get; set; }
    }

 


    internal class ConcordanceOptions
    {
        public int AbsScoreLimit { get; set; }
        public bool Ascending { get; set; }
        public bool CaseSensitive { get; set; }
        public Column Column { get; set; }
        public FirstLast FirstLast { get; set; }
        public bool IgnorePunctuation { get; set; }
        public Meta Meta { get; set; }
        public bool NumericEquivalence { get; set; }
        public int RelScroreLimit { get; set; }
        public bool ReverseLookup { get; set; }
        public List<string> TargetFilterStrings { get; set; }
        public int TopScoreCount { get; set; }

        
    }

  

    internal class ConcordanceItem
    {
        public RangeInfo[] ConcordanceTextRanges;
        public TranslationRangeInfo[] ConcordanceTranslationRanges;
        public int Length;
        public int StartPos;
        public TMEntryModel TMEntry;
    }

  

    internal class ConcordanceResult
    {
        public ConcordanceItem[] ConcResult { get; set; }
        public ConcTransItem[] ConcTransResult { get; set; }
        public ConcordanceError[] Errors { get; set; }
        public int TotalConcResult { get; set; }
    }

}
