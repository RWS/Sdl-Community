using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace SdlXliff.Toolkit.Integration.Data
{
    public class SegmentData
    {
        /// <summary>
        /// unique identifier of segment
        /// </summary>
        public int Sid
        {
            get;
            private set;
        }

        /// <summary>
        /// segment id
        /// </summary>
        public int SegmentId
        {
            get;
            private set;
        }

        /// <summary>
        /// segment text (from IText)
        /// </summary>
        public string SegmentText
        {
            get;
            private set;
        }

        /// <summary>
        /// segment confirmation status
        /// </summary>
        public ConfirmationLevel SegmentStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// ISegment of segment
        /// </summary>
        public ISegment SegmentContent
        {
            get;
            private set;
        }

        /// <summary>
        /// list of search matches (IndexData objects) - starting index, length of match
        /// </summary>
        public List<IndexData> SearchResults
        {
            get;
            set;
        }

        /// <summary>
        /// list of tags with matches inside text
        /// </summary>
        public List<TagData> Tags
        {
            get;
            set;
        }

        public SegmentData(int sid, int SegmentId, string segmentText, ConfirmationLevel segmentStatus, ISegment segmentContent)
        {
            Sid = sid;
            SegmentText = segmentText;
            SegmentStatus = segmentStatus;
            SegmentContent = segmentContent;
        }


        /// <summary>
        /// number of matches inside tags
        /// </summary>
        private int TagMatchesCount
        {
            get
            {
                int count = 0;
                if (Tags != null)
                    foreach (TagData data in Tags)
                        count += data.SearchResults.Count;

                return count;
            }
        }
        /// <summary>
        /// number of matches inside text
        /// </summary>
        public int IndexMatchesCount
        {
            get
            {
                return SearchResults == null ? 0 : SearchResults.Count;
            }
        }
        /// <summary>
        /// number of matches inside segment
        /// </summary>
        public int MatchesCount
        {
            get
            {
                return TagMatchesCount + IndexMatchesCount;
            }
        }
    }
}
