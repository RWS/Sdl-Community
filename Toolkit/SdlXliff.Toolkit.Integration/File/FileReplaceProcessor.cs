using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SdlXliff.Toolkit.Integration.Data;
using SdlXliff.Toolkit.Integration.Helpers;

namespace SdlXliff.Toolkit.Integration.File
{
    class FileReplaceProcessor : AbstractBilingualContentProcessor
    {
        private string _filePath;
        private SearchSettings _searchSettings;
        private DataExtractor _dataExtractor;
        private DataSearcher _searcher;

        private List<SegmentData> _resultSrc;
        private List<SegmentData> _resultTrg;
        private List<SegmentData> _resultReplace;
        private List<WarningData> _resultWarnings;

        private int _lockedCharactersCount = 0;
        private int _charactersShift = 0;

        /// <summary>
        /// list of SegmentData objects - source segment data in one file
        /// </summary>
        public List<SegmentData> ResultSource
        {
            get { return _resultSrc; }
            set { _resultSrc = value; }
        }
        /// <summary>
        /// list of SegmentData objects - search matches data (target) in one file
        /// </summary>
        public List<SegmentData> ResultTarget
        {
            get { return _resultTrg; }
            set { _resultTrg = value; }
        }

        /// <summary>
        /// list of SegmentData objects - replacements data in one file
        /// </summary>
        public List<SegmentData> ResultOfReplace
        {
            get { return _resultReplace; }
            set { _resultReplace = value; }
        }
        /// <summary>
        /// list of WarningData objects - warnings data in one file
        /// </summary>
        public List<WarningData> Warnings
        {
            get { return _resultWarnings; }
            set { _resultWarnings = value; }
        }

        public FileReplaceProcessor(string filePath, SearchSettings settings)
        {
            _filePath = filePath;
            _searchSettings = settings;
            _dataExtractor = new DataExtractor();
            _searcher = new DataSearcher(_searchSettings);

            _resultSrc = new List<SegmentData>();
            _resultTrg = new List<SegmentData>();
            _resultReplace = new List<SegmentData>();
            _resultWarnings = new List<WarningData>();
        }

        public override void SetFileProperties(IFileProperties fileInfo)
        {
            base.SetFileProperties(fileInfo);
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            ISegment sourceSegment;
            string sourceText;

            ISegment targetSegment;
            string targetText;
            List<IndexData> targetLContent;
            SegmentData segmentSearch;

            ConfirmationLevel itemStatus;

            if (paragraphUnit.IsStructure)
            {
                base.ProcessParagraphUnit(paragraphUnit);
                return;
            }

            foreach (ISegmentPair item in paragraphUnit.SegmentPairs)
            {
                sourceSegment = item.Source;
                targetSegment = item.Target;
                itemStatus = item.Properties.ConfirmationLevel;
                if (_searcher.checkSegment(item.Properties.IsLocked, itemStatus))
                {
                    _dataExtractor.Process(targetSegment);
                    targetText = _dataExtractor.PlainText.ToString();
                    targetLContent = _dataExtractor.LockedContent;
                    if (targetText.Length > 0)
                    {
                        // do search & save found results
                        _searcher.SearchInSegment(targetText, targetLContent);

                        // if matches in target were found
                        if (SegmentHelper.ContainMatches(_searcher.resultsInText))
                        {
                            #region extract source text
                            _dataExtractor.Process(sourceSegment);
                            sourceText = _dataExtractor.PlainText.ToString();
                            #endregion
                            CollectResults(item.Properties.Id.Id, sourceText, itemStatus, sourceSegment, null, true);
                            CollectResults(item.Properties.Id.Id, targetText, itemStatus, targetSegment, _searcher.resultsInText, false);

                            segmentSearch = _resultTrg[_resultTrg.Count - 1];

                            ISegment originalSegment = (ISegment)targetSegment.Clone();
                            // unlock selections
                            if (_searchSettings.UnlockContent)
                                StatusUpdateHelper.UnlockContent(targetSegment, ItemFactory, PropertiesFactory);

                            #region REPLACE Logic
                            int textLoc = 0;
                            int cnt = 0;
                            int status = 0;
                            for (int i = segmentSearch.SearchResults.Count - 1; i >= 0; i--)
                            {
                                // add warning - cannot be replaced
                                if (segmentSearch.SearchResults[i].IsIndexOverlap)
                                {
                                    status = -4;
                                    _resultWarnings.Add(new WarningData(segmentSearch.Sid,
                                               segmentSearch.SearchResults[i],
                                               WarningData.WarningType.IndexOverlap));
                                }
                                else
                                {
                                    // DO REPLACE - success
                                    status = PerformReplace(targetSegment, segmentSearch.SearchResults[i], ref textLoc);
                                    #region report warning - fail
                                    if (status != 0)
                                    {
                                        if (status == -1)
                                            _resultWarnings.Add(new WarningData(segmentSearch.Sid,
                                                segmentSearch.SearchResults[i],
                                                WarningData.WarningType.TagFound,
                                                segmentSearch.SearchResults[i].Length + textLoc));
                                        else if (status == -2)
                                            _resultWarnings.Add(new WarningData(segmentSearch.Sid,
                                                segmentSearch.SearchResults[i],
                                                WarningData.WarningType.ContainLContent,
                                                segmentSearch.SearchResults[i].Length + textLoc));
                                        else if (status == -3)
                                            _resultWarnings.Add(new WarningData(segmentSearch.Sid,
                                                segmentSearch.SearchResults[i],
                                                WarningData.WarningType.ContainComment,
                                                segmentSearch.SearchResults[i].Length + textLoc));
                                        else
                                            _resultWarnings.Add(new WarningData(segmentSearch.Sid,
                                                segmentSearch.SearchResults[i],
                                                WarningData.WarningType.Other));
                                    }
                                    #endregion
                                }

                                if (i == segmentSearch.SearchResults.Count - 1)
                                {
                                    // collect results
                                    _resultReplace.Add(new SegmentData(_resultReplace.Count,
                                        segmentSearch.SegmentId,
                                        segmentSearch.SegmentText,
                                        segmentSearch.SegmentStatus,
                                        segmentSearch.SegmentContent));
                                    _resultReplace[_resultReplace.Count - 1].SearchResults = new List<IndexData>();
                                }

                                // if replace was successful
                                if (status == 0)
                                {
                                    _resultReplace[_resultReplace.Count - 1].SearchResults.Add(segmentSearch.SearchResults[i]);
                                    _resultReplace[_resultReplace.Count - 1].SearchResults[cnt++].RealStartIndex = textLoc;
                                }
                            }
                            #endregion

                            #region UPDATE STATUSES Logic
                            // if replace occured
                            if (_resultReplace[_resultReplace.Count - 1].IndexMatchesCount > 0)
                            {
                                // update segment properties
                                StatusUpdateHelper.UpdateSegmentProperties(item, _searchSettings);
                            }
                            else
                            {
                                item.Target.Clear();
                                originalSegment.MoveAllItemsTo(item.Target);
                            }
                            #endregion
                        }
                    }
                }
            }

            base.ProcessParagraphUnit(paragraphUnit);
        }

   
        #region private
        private int PerformReplace(ISegment textSegment, IndexData searchData, ref int textIndex)
        {
            int textIndexStart = searchData.IndexStart;
            int textLength = searchData.Length;
            textIndex = 0;
            _charactersShift = 0;

            // iterator to find IText object the search text is placed in
            Location textLocation = new Location(textSegment, true);

            return PerformReplaceIterator(textLocation, textIndexStart, textLength, ref textIndex);
        }
        private int PerformReplaceIterator(Location textLocation, int textIndexStart, int textLength, ref int textIndex)
        {
            bool isWarning = false;
            _lockedCharactersCount = 0;
            TextCharacterCountingIterator iterator = new TextCharacterCountingIterator(textLocation);
            do
            {
                // if (iterator.CharacterCount + iterator.CharactersToNextLocation > textIndexStart)
                if (iterator.CharacterCount + iterator.CharactersToNextLocation + _charactersShift > textIndexStart)
                {
                    // text with tags - do not replace, report warning
                    //if (iterator.CharacterCount + iterator.CharactersToNextLocation < textIndexStart + textLength)
                    if (iterator.CharacterCount + iterator.CharactersToNextLocation + _charactersShift < textIndexStart + textLength)
                    {
                        //textIndex = (iterator.CharacterCount + iterator.CharactersToNextLocation) - (textIndexStart + textLength);
                        textIndex = (iterator.CharacterCount + iterator.CharactersToNextLocation + _charactersShift) - (textIndexStart + textLength);
                        isWarning = true;
                    }

                    #region IText
                    IText text = iterator.CurrentLocation.ItemAtLocation as IText;
                    if (text != null && !isWarning)
                    {
                        // real index in IText
                        textIndex = textIndexStart - (iterator.CharacterCount + _charactersShift);

                        // replace the text of IText
                        string replacedText = TextReplace(text.Properties.Text, textIndex, textLength);
                        iterator.CurrentLocation.ItemAtLocation.Parent[iterator.CurrentLocation.ItemAtLocation.IndexInParent] =
                             ItemFactory.CreateText(PropertiesFactory.CreateTextProperties(replacedText));
                        return 0;
                    }
                    #endregion

                    #region Tags
                    if (isWarning)
                    {
                        IPlaceholderTag phTag = iterator.CurrentLocation.ItemAtLocation as IPlaceholderTag;
                        ITagPair tagPair = iterator.CurrentLocation.ItemAtLocation as ITagPair;
                        if (phTag != null || tagPair != null)
                            return -1;
                        tagPair = iterator.CurrentLocation.BottomLevel.Parent as ITagPair;
                        if (iterator.CurrentLocation.BottomLevel.IsAtEndOfParent && tagPair != null)
                            return -1;
                    }
                    #endregion

                    #region Comment
                    if (isWarning)
                    {
                        ICommentMarker comment = iterator.CurrentLocation.ItemAtLocation as ICommentMarker;
                        if (comment != null)
                            return -3;
                        comment = iterator.CurrentLocation.BottomLevel.Parent as ICommentMarker;
                        if (iterator.CurrentLocation.BottomLevel.IsAtEndOfParent && comment != null)
                            return -3;
                    }
                    #endregion

                }

                #region LockedContent
                ILockedContent content = iterator.CurrentLocation.ItemAtLocation as ILockedContent;
                if (content != null)
                {
                    if (isWarning)
                        return -2;
                    // real index in ILockedContent
                    textIndex = textIndexStart - (iterator.CharacterCount + _charactersShift);

                    // call the iterator for Locked Content
                    Location lockedLocation = new Location((IAbstractMarkupDataContainer)content.Content, true);
                    int sts = PerformReplaceIterator(lockedLocation, textIndex, textLength, ref textIndex);

                    // CODE TO FIX TextCharacterCountingIterator BUG !!! >>
                    // if TextCharacterCountingIterator failed to get CharactersToNextLocation,
                    // we calculate it manually and save as _charactersShift
                    if (iterator.CharactersToNextLocation == 0)
                        _charactersShift += _lockedCharactersCount;
                    if (sts != -4)
                        return sts;
                }
                content = iterator.CurrentLocation.BottomLevel.Parent as ILockedContent;
                if (iterator.CurrentLocation.BottomLevel.IsAtEndOfParent && content != null)
                    if (isWarning)
                        return -1;
                #endregion

            } while (iterator.MoveNext());

            _lockedCharactersCount = iterator.CharacterCount;

            return -4;
        }
        private string TextReplace(string text, int startIndex, int length)
        {
            return string.Format("{0}{1}{2}",
                text.Substring(0, startIndex),
                _searchSettings.ReplaceText,
                text.Substring(startIndex + length));
        }

        private void CollectResults(string SegmentId, string segmentText, ConfirmationLevel segmentStatus, ISegment segmentContent,
            List<IndexData> matches, bool isSource)
        {
            int sID;
            if (int.TryParse(SegmentId, out sID))
            {
                if (isSource)
                {
                    _resultSrc.Add(new SegmentData(_resultSrc.Count, sID, segmentText, segmentStatus, segmentContent));
                }
                else
                {
                    _resultTrg.Add(new SegmentData(_resultTrg.Count, sID, segmentText, segmentStatus, segmentContent));
                    _resultTrg[_resultTrg.Count - 1].SearchResults = matches;
                }
            }
        }

        #endregion
    }
}
