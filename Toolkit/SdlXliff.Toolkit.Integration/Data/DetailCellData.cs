using System;
using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SdlXliff.Toolkit.Integration.Controls;

namespace SdlXliff.Toolkit.Integration.Data
{
    public class DetailCellData : Object
    {
        public DetailCellData Self
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// file path
        /// </summary>
        public string FileName
        { get; private set; }
        /// <summary>
        /// segment unique identifier
        /// </summary>
        public int SID
        { get; private set; }
        /// <summary>
        /// segment id
        /// </summary>
        public int SegID
        { get; private set; }
        /// <summary>
        /// confirmation status
        /// </summary>
        public string SegStatus
        { get; private set; }

        /// <summary>
        /// source segment match data
        /// </summary>
        public SegmentCellData Source
        { get; set; }

        /// <summary>
        /// target segment match/replace data
        /// </summary>
        public SegmentCellData Target
        { get; set; }

        /// <summary>
        /// warnings in segment (when replacing)
        /// </summary>
        public List<WarningCellData> Warnings
        { get; set; }

        public DetailCellData(string fileName, int sid, int segID, string segStatus)
        {
            FileName = fileName;
            SID = sid;
            SegID = segID;
            SegStatus = segStatus;
        }

        public DetailCellData(string fileName, int sid, int segID, string segStatus, string sourceText, ISegment sourceContent, string targetText, ISegment targetContent)
        {
            FileName = fileName;
            SID = sid;
            SegID = segID;
            SegStatus = segStatus;

            Source = new SegmentCellData(sourceText, sourceContent);
            Target = new SegmentCellData(targetText, targetContent);
        }

        /// <summary>
        /// number of warnings
        /// </summary>
        public int WarningsCount
        {
            get
            {
                int count = 0;
                if (Warnings != null)
                    foreach (WarningCellData data in Warnings)
                        count += data.MatchIndexes.Count;

                return count;
            }
        }

        /// <summary>
        /// make deep copy
        /// </summary>
        /// <returns></returns>
        public DetailCellData Clone()
        {
            DetailCellData data = new DetailCellData(FileName, SID, SegID, SegStatus);
            data.Source = Source.Clone();
            data.Target = Target.Clone();
            if (Warnings != null)
            {
                List<WarningCellData> warns = new List<WarningCellData>();
                foreach (WarningCellData warn in Warnings)
                    warns.Add(warn.Clone());
                data.Warnings = warns;
            }

            return data;
        }
    }

    public class SegmentCellData
    {
        /// <summary>
        /// target segment text (from IText only)
        /// </summary>
        public string Text
        { get; set; }
        /// <summary>
        /// ISegment object of current segment target
        /// </summary>
        public ISegment Content
        { get; private set; }
        /// <summary>
        /// indexes of matches in target text
        /// </summary>
        public List<IndexData> MatchIndexes
        { get; set; }
        /// <summary>
        /// tags in segment target (will be null if replace)
        /// </summary>
        public List<TagData> Tags
        {
            get;
            set;
        }

        public SegmentCellData(string text, ISegment content)
        {
            Text = text;
            Content = content;
        }

        public SegmentCellData(string text, ISegment content, List<IndexData> matchIndexes, List<TagData> tags)
        {
            Text = text;
            Content = content;
            MatchIndexes = matchIndexes;
            Tags = tags;
        }

        /// <summary>
        /// number of matches inside tags in source/target
        /// </summary>
        public int TagMatchesCount
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
        /// make deep copy
        /// </summary>
        /// <returns></returns>
        public SegmentCellData Clone()
        {
            SegmentCellData data = new SegmentCellData(Text, Content);

            if (MatchIndexes != null)
            {
                List<IndexData> sourceIndexes = new List<IndexData>();
                foreach (IndexData ind in MatchIndexes)
                    sourceIndexes.Add(new IndexData(ind.IndexStart, ind.Length));
                data.MatchIndexes = sourceIndexes;
            }

            if (Tags != null)
                data.Tags = CloneTagData(Tags);

            return data;
        }

        #region private
        private List<TagData> CloneTagData(List<TagData> tagData)
        {
            List<TagData> tags = new List<TagData>();
            foreach (var tag in tagData)
                tags.Add(tag.Clone());
            return tags;
        }
        #endregion
    }

    public class DetailDataSorter : IComparer<DetailCellData>
    {
        public int Compare(DetailCellData obj1, DetailCellData obj2)
        {
            int retval = obj1.FileName.CompareTo(obj2.FileName);
            if (retval == 0)
                retval = obj1.SID.CompareTo(obj2.SID);

            return retval;
        }
    }
}
