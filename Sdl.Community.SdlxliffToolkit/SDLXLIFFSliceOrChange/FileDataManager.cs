using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.Utilities.BatchSearchReplace.Lib;
using Sdl.Utilities.BatchSearchReplace.SearchResultsControl;

namespace Sdl.Community.SDLXLIFFSliceOrChange
{
    internal class FileDataManager
    {
        // Fields
        private List<FileData> _data;
        private List<DetailCellData> _detailData;
        public List<DetailCellData> DetailFilteredData { get; set; }

        // Methods
        public FileDataManager(List<FileData> data)
        {
            _data = data;
        }

        public List<string> GetFiles()
        {
            var list = new List<string>();
            foreach (FileData data in _data)
                if (((data.SourceMatchesCount > 0) || (data.TargetMatchesCount > 0)) && !list.Contains(data.FilePath))
                {
                    list.Add(data.FilePath);
                }
            return list;
        }

        public bool IsSearchResultEmpty()
        {
            foreach (FileData data in _data)
            {
                if (data.SourceMatchesCount > 0 || data.TargetMatchesCount > 0)
                    return false;
            }
            return true;
        }

        public int SetDetailDataSearch()
        {
            SetSearchData();
            DetailFilteredData = new List<DetailCellData>();
            foreach (DetailCellData data in _detailData)
            {
                DetailFilteredData.Add(data.Clone());
            }
            DetailFilteredData.Sort(new DetailDataSorter());
            return DetailFilteredData.Count;
        }

        private void SetSearchData()
        {
            if (_detailData == null)
            {
                _detailData = new List<DetailCellData>();
                foreach (FileData data in _data)
                {
                    foreach (SegmentData data2 in data.SearchSourceResults.Values)
                    {
                        SegmentData data3 = data.SearchTargetResults[data2.SID];
                        _detailData.Add(new DetailCellData(data.FilePath, data2.SID, data2.SegmentID,
                                                                SegmentStatus.GetText(data2.SegmentStatus),
                                                                data2.SegmentText, data2.SegmentContent,
                                                                data3.SegmentText, data3.SegmentContent));
                        _detailData[_detailData.Count - 1].Source.MatchIndexes = data2.SearchResults;
                        _detailData[_detailData.Count - 1].Source.Tags = data2.Tags;
                        _detailData[_detailData.Count - 1].Target.MatchIndexes = data3.SearchResults;
                        _detailData[_detailData.Count - 1].Target.Tags = data3.Tags;
                    }
                }
            }
        }
    }

    public class SegmentStatus
    {
        // Methods
        public static string GetText(ConfirmationLevel status)
        {
            switch (status)
            {
                case ConfirmationLevel.Draft:
                    return "Draft";

                case ConfirmationLevel.Translated:
                    return "Translated";

                case ConfirmationLevel.RejectedTranslation:
                    return "RejectedTranslation";

                case ConfirmationLevel.ApprovedTranslation:
                    return "ApprovedTranslation";

                case ConfirmationLevel.RejectedSignOff:
                    return "RejectedSignOff";

                case ConfirmationLevel.ApprovedSignOff:
                    return "ApprovedSignOff";
            }
            return "Unspecified";
        }

        public static ConfirmationLevel GetValue(string status)
        {
            if (status == "Draft")
            {
                return ConfirmationLevel.Draft;
            }
            if (status == "Translated")
            {
                return ConfirmationLevel.Translated;
            }
            if (status == "RejectedTranslation")
            {
                return ConfirmationLevel.RejectedTranslation;
            }
            if (status == "RejectedSignOff")
            {
                return ConfirmationLevel.RejectedSignOff;
            }
            if (status == "ApprovedTranslation")
            {
                return ConfirmationLevel.ApprovedTranslation;
            }
            if (status == "ApprovedSignOff")
            {
                return ConfirmationLevel.ApprovedSignOff;
            }
            return ConfirmationLevel.Unspecified;
        }
    }
}