using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;


namespace TMProvider
{
    public class ConcordanceResult
    {
        public ConcordanceItem[] ConcResult { get; private set; }

        public ConcTransItem[] ConcTransResult { get; private set; }

        public ConcordanceError[] Errors { get; private set; }

        public int TotalConcResult { get; private set; }

        public ConcordanceResult(ConcordanceItem[] concordanceItems, ConcTransItem[] concordanceTranslations, ConcordanceError[] errors, int totalConcResult)
        {
            this.ConcResult = concordanceItems;
            this.ConcTransResult = concordanceTranslations;
            this.TotalConcResult = totalConcResult;
            this.Errors = errors;
        }

 
        internal ConcordanceResult(MemoQServerTypes.ConcordanceResult memoQServerConcResult, string sourceLang = "", string targetLang = "")
        {
            if (memoQServerConcResult.ConcResult != null)
            {
                this.ConcResult = memoQServerConcResult.ConcResult.Select(range => new ConcordanceItem(range, sourceLang, targetLang)).ToArray();
            }
            else this.ConcResult = new ConcordanceItem[0];
            this.ConcTransResult = memoQServerConcResult.ConcTransResult;
            this.Errors = memoQServerConcResult.Errors;
            this.TotalConcResult = memoQServerConcResult.TotalConcResult;
        }
    }
}
