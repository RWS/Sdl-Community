using System.Collections.Generic;
using SdlXliff.Toolkit.Integration.Data;

namespace SdlXliff.Toolkit.Integration.File
{
    public class FileData
    {
        /// <summary>
        /// file path
        /// </summary>
        public string FilePath
        { get; set; }
        /// <summary>
        /// results of search operation in segments source
        /// </summary>
        public Dictionary<int, SegmentData> SearchSourceResults
        { get; private set; }
        /// <summary>
        /// results of search operation in segments target
        /// </summary>
        public Dictionary<int, SegmentData> SearchTargetResults
        { get; private set; }
        /// <summary>
        /// results of successful replace operations
        /// </summary>
        public List<SegmentData> ReplaceResults
        { get; set; }
        /// <summary>
        /// results of not successful replace operations - data that was found and not replaced
        /// </summary>
        public List<WarningData> Warnings
        { get; set; }

        public FileData(string file, List<SegmentData> sourceResults, List<SegmentData> targetResults)
        {
            FilePath = file;

            SearchSourceResults = new Dictionary<int, SegmentData>();
            SearchTargetResults = new Dictionary<int, SegmentData>();
            ReplaceResults = new List<SegmentData>();
            Warnings = new List<WarningData>();

            // populate SearchSourceResults & SearchTargetResults
            for (int i = 0; i < sourceResults.Count; i++)
            {
                SearchSourceResults.Add(sourceResults[i].Sid, sourceResults[i]);
                SearchTargetResults.Add(targetResults[i].Sid, targetResults[i]);
            }
        }

        /// <summary>
        /// total number of matches inside source
        /// </summary>
        public int SourceMatchesCount
        {
            get
            {
                int count = 0;
                foreach (SegmentData data in SearchSourceResults.Values)
                    count += data.MatchesCount;

                return count;
            }
        }
        /// <summary>
        /// total number of matches inside target
        /// </summary>
        public int TargetMatchesCount
        {
            get
            {
                int count = 0;
                foreach (SegmentData data in SearchTargetResults.Values)
                    count += data.MatchesCount;

                return count;
            }
        }
        /// <summary>
        /// total number of replaces inside target
        /// </summary>
        public int ReplaceCount
        {
            get
            {
                int count = 0;
                foreach (SegmentData data in ReplaceResults)
                    count += data.MatchesCount;

                return count;
            }
        }
    }
}
