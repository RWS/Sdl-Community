using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sdl.Core.Globalization;

namespace SdlXliff.Toolkit.Integration
{
    public class SearchSettings
    {
      

        /// <summary>
        /// search text string
        /// </summary>
        [XmlIgnore]
        public string SearchText
        { get; set; }
        /// <summary>
        /// replace text string
        /// </summary>
        [XmlIgnore]
        public string ReplaceText
        { get; set; }

        /// <summary>
        /// perform search in source text
        /// </summary>
        public bool SearchInSource
        { get; set; }
        /// <summary>
        /// perform search in target text
        /// </summary>
        public bool SearchInTarget
        { get; set; }
        /// <summary>
        /// search in inline and placeholder tags
        /// </summary>
        public bool SearchInTag
        { get; set; }

        public bool MatchCase
        { get; set; }
        public bool MatchWholeWord
        { get; set; }
        public bool UseRegex
        { get; set; }

        /// <summary>
        /// search in locked text, segment, TU
        /// </summary>
        public bool SearchInLocked
        { get; set; }
        /// <summary>
        /// statuses to not search/replace in
        /// </summary>
        public List<ConfirmationLevel> NotSearchStatus
        { get; set; }

        /// <summary>
        /// update segment status where replace was done
        /// </summary>
        public bool UpdateStatus
        { get; set; }
        /// <summary>
        /// new status of segment
        /// </summary>
        public ConfirmationLevel NewStatus
        { get; set; }
        /// <summary>
        /// lock segments where replace was done
        /// </summary>
        public bool LockSegment
        { get; set; }
        /// <summary>
        /// unlock segments or selection where replace was done
        /// </summary>
        public bool UnlockContent
        { get; set; }

        /// <summary>
        /// create backup file copy (when replacing)
        /// </summary>
        public bool MakeBackup
        { get; set; }
    }
}
