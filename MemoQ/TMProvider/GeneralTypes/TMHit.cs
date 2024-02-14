using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMProvider
{
    public class TMHit: IComparable
    {
        public int MatchRate { get; set; }
        public TranslationUnit TranslationUnit { get; set; }
        /// <summary>
        /// Can be set later at lookup.
        /// </summary>
        public string TMName { get; set; }
        /// <summary>
        /// The source TM this hit was coming from. Can be set later during lookup. Null if not set.
        /// </summary>
        public Guid TMGuid { get; set; }

        internal TMHit(MemoQServerTypes.TMHit memoQServerTMHit, string sourceLang = "", string targetLang = "")
        {
            this.TMName = "";
            this.MatchRate = memoQServerTMHit.MatchRate;
            this.TranslationUnit = new TranslationUnit(memoQServerTMHit.TransUnit, sourceLang, targetLang);
        }

        public TMHit(TranslationUnit tu, int matchRate, Guid tmGuid, string tmName = "")
        {
            this.TranslationUnit = tu;
            this.MatchRate = matchRate;
            this.TMGuid = tmGuid;
            this.TMName = tmName;
        }

        public int CompareTo(object hit)
        { 
            // compares only match rates
            if (hit == null) return 1;
            return this.MatchRate.CompareTo(((TMHit)hit).MatchRate);
        }
    }
}
