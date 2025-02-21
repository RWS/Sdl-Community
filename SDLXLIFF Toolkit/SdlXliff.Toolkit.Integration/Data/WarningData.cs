using System.Collections.Generic;

namespace SdlXliff.Toolkit.Integration.Data
{
    public class WarningData
    {
        public enum WarningType { TagFound, ContainComment, ContainLContent, IndexOverlap, Other };

        /// <summary>
        /// SegmentId the warning appeared in
        /// </summary>
        public int SID
        {
            get;
            set;
        }

        /// <summary>
        /// indexes of match the error appeared in
        /// </summary>
        public IndexData MatchIndex
        {
            get;
            set;
        }

        /// <summary>
        /// the index of tag overlap with the match text
        /// </summary>
        public int TagIndex
        {
            get;
            set;
        }

        /// <summary>
        /// type of warning
        /// </summary>
        public WarningType Type
        {
            get;
            set;
        }

        public WarningData(int sid, IndexData indexes, WarningType type)
        {
            SID = sid;
            MatchIndex = indexes;
            Type = type;
        }

        public WarningData(int sid, IndexData indexes, WarningType type, int tagIndex)
        {
            SID = sid;
            MatchIndex = indexes;
            Type = type;
            TagIndex = tagIndex;
        }

        /// <summary>
        /// get warning message by type
        /// </summary>
        /// <returns>warning text</returns>
        public string GetMessage()
        {
            switch (Type)
            {
                case WarningType.TagFound:
                    return "Inline tag is found within the match. The match was not replaced.";
                case WarningType.ContainComment:
                    return "A part of commented text is found within the match. The match was not replaced.";
                case WarningType.ContainLContent:
                    return "A part of locked content is found within the match. The match was not replaced.";
                case WarningType.IndexOverlap:
                    return "Match text indexes overlap. The match was not replaced.";
                default:
                    return "Unexpected exception.";
            }
        }
    }

    public class WarningDataSorter : IComparer<WarningData>
    {
        public int Compare(WarningData obj1, WarningData obj2)
        {
            int retval = obj1.SID.CompareTo(obj2.SID);
            if (retval == 0)
                retval = obj1.Type.CompareTo(obj2.Type);
            if (retval == 0)
                retval = obj2.MatchIndex.IndexStart.CompareTo(obj1.MatchIndex.IndexStart);

            return retval;
        }
    }
}
