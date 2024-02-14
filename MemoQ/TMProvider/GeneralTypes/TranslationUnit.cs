using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMProvider
{
    public class TranslationUnit
    {
        public string Client { get; set; }

        public string ContextID { get; set; }

        public DateTime Created { get; set; }

        public string Creator { get; set; }

        public string Document { get; set; }

        public string Domain { get; set; }

        public string FollowingSegment { get; set; }

        public int Key { get; set; }

        public DateTime Modified { get; set; }

        public string Modifier { get; set; }

        public string PrecedingSegment { get; set; }

        public string Project { get; set; }

        public string SourceSegment { get; set; }

        public string Subject { get; set; }

        public string TargetSegment { get; set; }
        /// <summary>
        /// Three-letter memoQ code.
        /// </summary>
        public string SourceLangCode { get; set; }
        /// <summary>
        /// Three letter memoQ code.
        /// </summary>
        public string TargetLangCode { get; set; }

        /// <summary>
        /// Don't forget to assign values to the properties! No default values.
        /// </summary>
        public TranslationUnit()
        { }


        internal TranslationUnit(MemoQServerTypes.TMEntryModel memoQServerTMEntry, string sourceLang = "", string targetLang = "")
        {
            this.Client = memoQServerTMEntry.Client;
            this.ContextID = memoQServerTMEntry.ContextID;
            this.Created = memoQServerTMEntry.Created;
            this.Creator = memoQServerTMEntry.Creator;
            this.Document = memoQServerTMEntry.Document;
            this.Domain = memoQServerTMEntry.Domain;
            this.FollowingSegment = memoQServerTMEntry.FollowingSegment;
            this.Key = memoQServerTMEntry.Key;
            this.Modified = memoQServerTMEntry.Modified;
            this.Modifier = memoQServerTMEntry.Modifier;
            this.PrecedingSegment = memoQServerTMEntry.PrecedingSegment;
            this.Project = memoQServerTMEntry.Project;
            this.SourceSegment = memoQServerTMEntry.SourceSegment;
            this.Subject = memoQServerTMEntry.Subject;
            this.TargetSegment = memoQServerTMEntry.TargetSegment;
            this.SourceLangCode = sourceLang;
            this.TargetLangCode = targetLang;
        }
    }
}
