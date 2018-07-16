using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SdlXliff.Toolkit.Integration.Data;
using SdlXliff.Toolkit.Integration.Helpers;

namespace SdlXliff.Toolkit.Integration.File
{
    class FileReadProcessor : AbstractBilingualContentProcessor
    {
        private string _filePath;
        private DataExtractor _dataExtractor;
        private DataSearcher _searcher;
        private List<SegmentData> _resultSrc;
        private List<SegmentData> _resultTrg;

        protected SearchSettings _searchSettings;

        /// <summary>
        /// list of SegmentData objects - search matches data in source in one file
        /// </summary>
        public List<SegmentData> ResultInSource
        {
            get { return _resultSrc; }
        }
        /// <summary>
        /// list of SegmentData objects - search matches data in target in one file
        /// </summary>
        public List<SegmentData> ResultInTarget
        {
            get { return _resultTrg; }
        }

        public FileReadProcessor(string filePath, SearchSettings settings)
        {
            _filePath = filePath;
            _searchSettings = settings;
            _dataExtractor = new DataExtractor();
            _searcher = new DataSearcher(_searchSettings);

            _resultSrc = new List<SegmentData>();
            _resultTrg = new List<SegmentData>();
        }

        public override void SetFileProperties(IFileProperties fileInfo)
        {
            base.SetFileProperties(fileInfo);
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            ISegment sourceSegment;
            string sourceText;
            List<TagData> sourceTags;
            List<IndexData> sourceLContent;
            List<IndexData> sourceResult;

            ISegment targetSegment;
            string targetText;
            List<TagData> targetTags;
            List<IndexData> targetLContent;
            List<IndexData> targetResult;

            ConfirmationLevel itemStatus;

            if (!paragraphUnit.IsStructure)
                foreach (ISegmentPair item in paragraphUnit.SegmentPairs)
                {
                    itemStatus = item.Properties.ConfirmationLevel;
                    sourceTags = null;
                    sourceResult = null;
                    targetTags = null;
                    targetResult = null;

                    // extract text and tags from Segment
                    sourceSegment = item.Source;
                    _dataExtractor.Process(sourceSegment);
                    sourceTags = _dataExtractor.Tags;
                    sourceText = _dataExtractor.PlainText.ToString();                 
                    sourceLContent = _dataExtractor.LockedContent;

                    targetSegment = item.Target;
                    _dataExtractor.Process(targetSegment);
                    targetText = _dataExtractor.PlainText.ToString();
                    targetTags = _dataExtractor.Tags;
                 targetLContent = _dataExtractor.LockedContent;

                    // perform search
                    if (_searcher.checkSegment(item.Properties.IsLocked, item.Properties.ConfirmationLevel))
                    {
                        if (_searchSettings.SearchInSource && (sourceText.Length > 0 || sourceTags.Count > 0))
                        {
                            _searcher.SearchInSegment(sourceText, sourceTags, sourceLContent);
                            sourceResult = _searcher.resultsInText;
                            sourceTags = _searcher.resultsInTags;
                        }

                        if (_searchSettings.SearchInTarget && (targetText.Length > 0 || targetTags.Count > 0))
                        {
                            _searcher.SearchInSegment(targetText, targetTags, targetLContent);
                            targetResult = _searcher.resultsInText;
                            targetTags = _searcher.resultsInTags;
                        }

                        // collect results
                        if (SegmentHelper.ContainMatches(sourceResult, sourceTags) || SegmentHelper.ContainMatches(targetResult, targetTags))
                        {
                            CollectResults(item.Properties.Id.Id, sourceText, itemStatus, sourceSegment, sourceResult, sourceTags, true);
                            CollectResults(item.Properties.Id.Id, targetText, itemStatus, targetSegment, targetResult, targetTags, false);
                        }
                    }

                    // TODO - REMOVE
                    //// process source
                    //if (_searchSettings.SearchInSource)
                    //{
                    //    sSegment = item.Source;
                    //    if (_searcher.checkSegment(sSegment.Properties.IsLocked, sSegment.Properties.ConfirmationLevel))
                    //    {
                    //        _dataExtractor.Process(sSegment, _searchSettings.SearchInLocked);
                    //        sText = _dataExtractor.PlainText.ToString();
                    //        sTags = _dataExtractor.Tags;
                    //        if (sText.Length > 0 || sTags.Count > 0)
                    //        {
                    //            _searcher.SearchInSegment(sText, sTags);
                    //            sResult = _searcher.resultsInText;
                    //            sTags = _searcher.resultsInTags;
                    //            CollectResults(sSegment.Properties.Id.Id, sText, sSegment, sResult, sTags, true);
                    //        }
                    //    }
                    //}

                    //// process target
                    //if (_searchSettings.SearchInTarget)
                    //{
                    //    sSegment = item.Target;
                    //    if (_searcher.checkSegment(sSegment.Properties.IsLocked, sSegment.Properties.ConfirmationLevel))
                    //    {
                    //        _dataExtractor.Process(sSegment, _searchSettings.SearchInLocked);
                    //        sText = _dataExtractor.PlainText.ToString();
                    //        sTags = _dataExtractor.Tags;
                    //        if (sText.Length > 0 || sTags.Count > 0)
                    //        {
                    //            _searcher.SearchInSegment(sText, sTags);
                    //            sResult = _searcher.resultsInText;
                    //            sTags = _searcher.resultsInTags;
                    //            CollectResults(sSegment.Properties.Id.Id, sText, sSegment, sResult, sTags, false);
                    //        }
                    //    }
                    //}
                }
        }

        public override void Complete()
        {
            base.Complete();
        }

        #region private
        private void CollectResults(string segmentID, string segmentText, ConfirmationLevel segmentStatus,
            ISegment segmentContent, List<IndexData> matches, List<TagData> tags, bool isSource)
        {
            int sID;
            if (int.TryParse(segmentID, out sID))
                if (isSource)
                {
                    _resultSrc.Add(new SegmentData(_resultSrc.Count,
                        sID, segmentText, segmentStatus, segmentContent));
                    _resultSrc[_resultSrc.Count - 1].SearchResults = matches;
                    _resultSrc[_resultSrc.Count - 1].Tags = tags;
                }
                else
                {
                    _resultTrg.Add(new SegmentData(_resultTrg.Count,
                        sID, segmentText, segmentStatus, segmentContent));
                    _resultTrg[_resultTrg.Count - 1].SearchResults = matches;
                    _resultTrg[_resultTrg.Count - 1].Tags = tags;
                }
        }


        #endregion
    }
}
