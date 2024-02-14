using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using AutoMapper.Mappers;


namespace TMProvider
{
    public class ConcordanceItem
    {
        public RangeInfo[] ConcordanceTextRanges { get; private set; }

        public TranslationRangeInfo[] ConcordanceTranslationRanges { get; private set; }

        public int Length { get; private set; }

        public int StartPos { get; private set; }

        public TranslationUnit TMEntry { get; private set; }

        public ConcordanceItem(RangeInfo[] concordanceTextRanges, TranslationRangeInfo[] concordanceTransRanges, int start, int length, TranslationUnit tmEntry)
        {
            this.ConcordanceTextRanges = concordanceTextRanges;
            this.ConcordanceTranslationRanges = concordanceTransRanges;
            this.Length = length;
            this.StartPos = start;
            this.TMEntry = TMEntry;
        }

 
        internal ConcordanceItem(MemoQServerTypes.ConcordanceItem memoQServerConcItem, string sourceLang = "", string targetLang = "")
        {
            this.ConcordanceTextRanges = memoQServerConcItem.ConcordanceTextRanges;
            this.ConcordanceTranslationRanges = memoQServerConcItem.ConcordanceTranslationRanges;
            this.Length = memoQServerConcItem.Length;
            this.StartPos = memoQServerConcItem.StartPos;
            this.TMEntry = new TranslationUnit(memoQServerConcItem.TMEntry, sourceLang, targetLang);
        }

    }
}
