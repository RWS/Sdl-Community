namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    public class FileParser
    {
        public delegate void OnProgressDelegate(double progress);
        public event OnProgressDelegate OnProgress;

        #region Fields
        private string _fPath;
        private string _outPath;
        private bool _isSeparateFile;

        private SplitOptions _options;
        private int _wordsMax;
        
        private int? _outFCount;
        private StreamWriter _writer;
        private SplitInfoWriter _infoWriter;
        private string _fileHash;

        private string _xliffDeclar;
        private string _xliffTag = "";
        private string _fileTag = "";
        private StringBuilder _docinfo = new StringBuilder();
        private StringBuilder _header = new StringBuilder();
        private Dictionary<string, string> _headerTags = new Dictionary<string, string>();
        private Dictionary<string, string> _headerTagSubs = new Dictionary<string, string>();
        private Dictionary<string, bool> _prevFileTags;
        private Dictionary<string, bool> _splitFileTUs;

        private bool _isTagToAdd;

        private List<Warning> _warnings;
        private List<string> _segmentIDsFound;
        #endregion

        #region Props
        /// <summary>
        /// number of split files created,
        /// if -1 - corrupt file error
        /// </summary>
        public int? FilesCount
        {
            get { return _outFCount; }
        }
        /// <summary>
        /// warnings when splitting file (by segment numbers)
        /// </summary>
        public List<Warning> Warnings
        {
            get { return _warnings; }
        }
        /// <summary>
        /// input file path
        /// </summary>
        public string FilePath
        {
            get { return _fPath; }
        }
        /// <summary>
        /// split files directory (target folder, contains folders with files)
        /// </summary>
        public string OutPath
        {
            get
            { return _outPath; }
        }
        /// <summary>
        /// split files directory (a folder in target folder, contains sdlxliff files)
        /// </summary>
        private string currFileOut
        {
            get {
                return string.Format(@"{0}\{1}_{2}.sdlxliff", _outPath,
                    (_outFCount.HasValue ? _outFCount.Value.ToString("000") : "000"), 
                    _fileHash);
            }
        }
        /// <summary>
        /// temporary split file path
        /// </summary>
        private string tempUpdFilePath
        {
            get
            {
                return string.Format(@"{0}\{1}_{2}_tag.sdlxliff", _outPath,
                    (_outFCount.HasValue ? _outFCount.Value.ToString("000") : "000"),
                    _fileHash);
            }
        }
        #endregion

        #region Constructor
        public FileParser(string filePath, string outPath)
        {
            _fPath = filePath;
            _isSeparateFile = true;
            _outPath = NormalizeOutPath(outPath);
        }
        public FileParser(string filePath, bool isSeparateFile, string outPath)
        {
            _fPath = filePath;
            _isSeparateFile = isSeparateFile;
            _outPath = NormalizeOutPath(outPath);
        }
        #endregion

        #region Public
        /// <summary>
        /// splits the file using default options (word-count, 1000)
        /// </summary>
        public void Split()
        {
            _options = new SplitOptions();
            _wordsMax = _options.WordsCount;

            ValidateFields();

            DoSplit();
        }
        /// <summary>
        /// splits the file using custom options
        /// </summary>
        /// <param name="sOptions"></param>
        public void Split(SplitOptions sOptions)
        {
            _options = sOptions;
            _wordsMax = _options.WordsCount;

            ValidateFields();

            // if EgualParts - get words count for a file
            if (_options.Criterion == SplitOptions.SplitType.EqualParts)
            {
                SetWordsCount(_options.PartsCount);
                if (_wordsMax < 1)
                    throw new ArgumentOutOfRangeException(string.Format(Properties.StringResource.errWordCalcOutOfRange, 
                        _fPath, 
                        _options.PartsCount));
            }
            else if (_options.Criterion == SplitOptions.SplitType.SegmentNumbers)
            {
                _warnings = new List<Warning>();
                _segmentIDsFound = new List<string>();
            }

            DoSplit();

            if (_options.Criterion == SplitOptions.SplitType.SegmentNumbers)
                CheckFoundSegments();
        }
        #endregion

        #region Private 
        private void DoSplit()
        {
            int countWords = 0;
            string mrkID = "";
            bool isStart = true;

            string segID; 
            // int segIDNum;
            string segToSplit = "";

            // Words Count dictionary for every segment
            Dictionary<string, int> segmentCountWords = new Dictionary<string, int>();
            List<string> xTagTUs = new List<string>();
            _splitFileTUs = new Dictionary<string, bool>();
             
            // generate file hash string
            _fileHash = FileHelper.SHA1HashFile(_fPath);

            // Init writer
            _outFCount = 1;
            FileHelper.CheckPath(currFileOut, true);
            FileHelper.CheckPath(_outPath, true);
            _writer = new StreamWriter(currFileOut);
            _infoWriter = new SplitInfoWriter(_fPath, _fileHash, _outPath);
            _infoWriter.WriteFileTag(Path.GetFileName(currFileOut));

            StreamFeeder feeder = new StreamFeeder(_fPath);
            TagParser tagParser = new TagParser(feeder);
            bool isFileCorrupt = false;
            bool isFileEmpty = true;
            bool isMrkFound = false;

            while (tagParser.Next() && !isFileCorrupt)
            {
                #region BODY LOGIC
                if (tagParser.IsInBody)
                {
                        // if TEXT found -- count segment words and save in dictionary
                        if (!tagParser.IsTag)
                        {
                            if (tagParser.IsInMrkText && tagParser.IsInSegSource && tagParser.isUnitTranslatable)
                            {
                                mrkID = tagParser.MrkMIDAttr;
                                if (mrkID.Length > 0)
                                    // AddCountToDict(mrkID, TextHelper.GetWordsCountEng(tagParser.ParsedText), ref segmentCountWords);
                                    AddCountToDict(mrkID, TextHelper.GetWordsCount(tagParser.ParsedText, tagParser.SourceLangAttr), ref segmentCountWords);
                                
                                isMrkFound = true;
                            }
                        }

                        // if TAG found
                        else
                        {
                            // get info about segments, count words in group
                            // that satisfy the conditions
                            if (tagParser.TagID == Tags.SegStart)
                            {
                                segID = tagParser.ParsedText.AttributeValue(TagDirectory.AttrID) ?? "";
                                if (segID.Length > 0)
                                {
                                    if (_options.Criterion == SplitOptions.SplitType.SegmentNumbers)
                                        SetSegToSplit(segID, ref segToSplit);
                                    if (isGroupWordsCountable(tagParser.ParsedText.AttributeValue(TagDirectory.AttrSegConf),
                                        tagParser.ParsedText.AttributeValue(TagDirectory.AttrSegPerc),
                                        tagParser.ParsedText.AttributeValue(TagDirectory.AttrSegLocked)))
                                        if (segmentCountWords.ContainsKey(segID))
                                            countWords += segmentCountWords[segID];
                                }
                            }

                            // clear the Words Count dictionary for every segment
                            else if (tagParser.TagID == Tags.SegDefsEnd)
                                segmentCountWords = new Dictionary<string, int>();

                            // we are at groups edge - check for words count >= max words count
                            // to start new file
                            else if ((tagParser.TagID == Tags.GroupStart || tagParser.TagID == Tags.TransUnitStart) && tagParser.isBodyParent)
                            {
                                if (xTagTUs.Count == 0 || xTagTUs.Intersect(_splitFileTUs.Keys).Count() == xTagTUs.Count)
                                {
                                    if ((_options.Criterion == SplitOptions.SplitType.SegmentNumbers && segToSplit.Length > 0 && !_segmentIDsFound.Contains(segToSplit))
                                        || (_options.Criterion != SplitOptions.SplitType.SegmentNumbers && countWords >= _wordsMax))
                                    {
                                        // write to split info file
                                        _infoWriter.WriteTransUnitsCountTag(_splitFileTUs.Count);
                                        _infoWriter.WriteWordsCountTag(countWords);
                                        AddSplitSegmentIDs(segToSplit);

                                        WriteToNewFile();
                                        countWords = 0;
                                        segToSplit = "";

                                        // clear TUs log lists
                                        _splitFileTUs = new Dictionary<string, bool>();
                                        xTagTUs = new List<string>();
                                    }
                                }
                            }

                            // get trans-unit ids in <x> tag
                            else if (tagParser.TagID == Tags.XStart && !string.IsNullOrEmpty(tagParser.XTagXIDAttr))
                            {
                                string[] xTUs = tagParser.XTagXIDAttr.Split(' ');
                                foreach (string xTU in xTUs)
                                    if (!xTagTUs.Contains(xTU))
                                        xTagTUs.Add(xTU);
                            }

                            // get any trans-unit id and write it to splitinfo file & list of trans-units of one split file
                            if (tagParser.TagID == Tags.TransUnitStart)
                                if (!_splitFileTUs.ContainsKey(tagParser.TransUnitIDAttr))
                                    _splitFileTUs.Add(tagParser.TransUnitIDAttr, false);
                        }

                    isFileEmpty = false;
                }
                #endregion

                #region HEADER LOGIC (<tag id=""...> optimization only)
                else if (tagParser.IsInHeader)
                {
                    // we are handling tags separately
                    if (tagParser.IsInTag)
                    {
                        AddTagToDict(tagParser.TagIDAttr, tagParser.ParsedText, (tagParser.TagID == Tags.TagStart));
                        if (tagParser.TagID == Tags.TagSubStart && !string.IsNullOrEmpty(tagParser.TagSubXIDAttr))
                            AddTagSubToDict(tagParser.TagIDAttr, tagParser.TagSubXIDAttr);
                    }
                    // just write header to write it to all the split files
                    else
                        if (!tagParser.IsInReference)
                            _header.Append(tagParser.ParsedText);
                }
                #endregion

                #region DOCINFO LOGIC
                else if (tagParser.IsInDocInfo)
                    _docinfo.Append(tagParser.ParsedText);
                #endregion

                #region OTHER
                else
                {
                    if (tagParser.TagID == Tags.FileStart)
                    {
                        if (!string.IsNullOrEmpty(_fileTag))
                            isFileCorrupt = true;
                        _fileTag = tagParser.ParsedText;
                    }
                    else if (tagParser.TagID == Tags.XliffStart)
                        _xliffTag = tagParser.ParsedText;

                    // XML Declaration
                    else if (isStart)
                    {
                        _xliffDeclar = tagParser.ParsedText;
                        isStart = false;
                    }
                }
                #endregion

                // if not <tag> tag - always write to file
                // if not reference - optimized split files
                if (!tagParser.IsInTag && !tagParser.IsInReference)
                    WriteToFile(tagParser.ParsedText);

                // report current operation progress
                ProgressSplit(tagParser.Progress(), 2);
            }

            tagParser.Dispose();
            _writer.Dispose();

            // write to split info file
            _infoWriter.WriteTransUnitsCountTag(_splitFileTUs.Count);
            _infoWriter.WriteWordsCountTag(countWords);
            _infoWriter.CloseWrite(_outFCount.HasValue ? _outFCount.Value : 0);
            AddSplitSegmentIDs(segToSplit);

            // validate file (throws exception)
            this.ValidateFileAfterParsing(isFileCorrupt, isMrkFound, isFileEmpty);

            // last file is empty
            if (countWords == 0)
                UpdateFile(true);
            // read & write split file again to add tags into <tag_defs>
            else if (_headerTags.Count > 0)
                UpdateFile(false);
        }

        private void ValidateFields()
        {
            // input file path/name
            if (!Directory.Exists(Path.GetDirectoryName(_fPath)) || !File.Exists(_fPath))
                throw new FileNotFoundException(string.Format(Properties.StringResource.errFileNotExist, _fPath));

            // percent (for any criterion)
            if (_options.IsPercent)
                if (_options.PercMax < 1 || _options.PercMax > 100)
                    throw new ArgumentOutOfRangeException(Properties.StringResource.errPercentOutOfRange);

            // words number (for word-count & equal parts)
            if (_options.Criterion == SplitOptions.SplitType.WordsCount && _wordsMax < 1)
                throw new ArgumentOutOfRangeException(Properties.StringResource.errWordCountOutOfRange);
            
            // parts number (for equal parts)
            if (_options.Criterion == SplitOptions.SplitType.EqualParts && _options.PartsCount < 2)
                throw new ArgumentOutOfRangeException(Properties.StringResource.errPartsCountOutOfRange);
        }

        private void ValidateFileAfterParsing(bool isFileCorrupt, bool isMrkFound, bool isFileEmpty)
        {
            // file is corrupt - rollback all changes
            if (isFileCorrupt || _xliffTag == string.Empty || _fileTag == string.Empty)
            {
                RollbackFiles();
                throw new InvalidDataException(Properties.StringResource.errFileCorrupt);
            }

            // file is empty (has no content)
            if (isFileEmpty)
            {
                RollbackFiles();
                throw new InvalidDataException(Properties.StringResource.errFileEmpty);
            }

            // file is not pre-processed
            if (!isMrkFound)
            {
                RollbackFiles();
                throw new InvalidDataException(Properties.StringResource.errFileUnexpectedStructure);
            }
        }

        private void SetWordsCount(int partsNumber)
        {
            int wordsInFile = 0;
            //int segIDNum = 0;
            string mrkID = "";
            string segID = "";

            // Words Count dictionary for every segment
            Dictionary<string, int> segmentCountWords = new Dictionary<string, int>();

            StreamFeeder feeder = new StreamFeeder(_fPath);
            TagParser tagParser = new TagParser(feeder);
            bool isMrkFound = false;

            while (tagParser.Next())
            {
                // look for text in body only
                if (tagParser.IsInBody)
                {
                    // if TEXT found -- count segment words and save in dictionary
                    if (!tagParser.IsTag)
                    {
                        if (tagParser.IsInMrkText && tagParser.IsInSegSource && tagParser.isUnitTranslatable)
                        {
                            mrkID = tagParser.MrkMIDAttr;
                            if (mrkID.Length > 0)
                                AddCountToDict(mrkID, TextHelper.GetWordsCount(tagParser.ParsedText, tagParser.SourceLangAttr), ref segmentCountWords);

                            isMrkFound = true;
                        }
                    }
                    else
                    {
                        // get info about segments, count words in group
                        // that satisfy the conditions
                        if (tagParser.TagID == Tags.SegStart)
                        {
                            segID = tagParser.ParsedText.AttributeValue(TagDirectory.AttrID) ?? "";
                            if (segID.Length > 0)
                            {
                                if (isGroupWordsCountable(tagParser.ParsedText.AttributeValue(TagDirectory.AttrSegConf),
                                    tagParser.ParsedText.AttributeValue(TagDirectory.AttrSegPerc),
                                    tagParser.ParsedText.AttributeValue(TagDirectory.AttrSegLocked)))
                                    if (segmentCountWords.ContainsKey(segID))
                                        wordsInFile += segmentCountWords[segID];
                            }
                        }

                        // clear the Words Count dictionary for every segment
                        else if (tagParser.TagID == Tags.SegDefsEnd)
                            segmentCountWords = new Dictionary<string, int>();
                    }
                }

                // report current operation progress
                ProgressSplit(tagParser.Progress(), 1);
            }
            tagParser.Dispose();

            // file is not pre-processed
            if (!isMrkFound)
            {
                throw new InvalidDataException(Properties.StringResource.errFileUnexpectedStructure);
            }

            // calculate number of words for a file
            _wordsMax = wordsInFile / partsNumber;
            if (wordsInFile % partsNumber > 0)
                _wordsMax += 1;
        }
        private void SetSegToSplit(string currSegID, ref string currSegToSplit)
        {
            if (currSegToSplit.Length > 0)
            {
                if (_options.SegmentIDs.Contains(currSegToSplit))
                    _warnings.Add(new Warning(currSegToSplit, Warning.WarningType.WrongSplitLocation));
                currSegToSplit = currSegID;
            }
            else if (_options.SegmentIDs.Contains(currSegID))
            {
                currSegToSplit = currSegID;
            }
        }
        private void CheckFoundSegments()
        {
            foreach (string segID in _options.SegmentIDs.Except(_segmentIDsFound))
            {
                if (_warnings.FindIndex(w => w.ElementID == segID && w.Type == Warning.WarningType.WrongSplitLocation) < 0)
                    _warnings.Add(new Warning(segID, Warning.WarningType.NotFound));
            }
        }

        private string NormalizeOutPath(string path)
        {
            string outPath = "";
            if (_isSeparateFile)
                outPath = string.Format(Properties.StringResource.dirSplitFiles, path, Path.GetFileNameWithoutExtension(_fPath)).Replace("/", @"\");
            else
            {
                string[] origFilePath = _fPath.Replace(@"\", "/").Split('/');

                outPath = string.Format(Properties.StringResource.dirSplitFilesFromProject,
                    path,
                    (origFilePath.Length > 1 ? origFilePath[origFilePath.Length - 2] : ""),
                    Path.GetFileNameWithoutExtension(_fPath)).Replace("/", @"\");
            }

            string outPathTemp = outPath; 
            int i = 0;
            while (Directory.Exists(outPathTemp))
                outPathTemp = string.Format("{0}_{1}", outPath, ++i);

            return outPathTemp;
        }

        #region add to dictionary/list methods
        private void AddCountToDict(string key, int value, ref Dictionary<string, int> dictionary)
        {
            if (value > 0)
            {
                if (dictionary.ContainsKey(key)) dictionary[key] += value;
                else dictionary.Add(key, value);
            }
        }
        private void AddTagToDict(string key, string value, bool isTagStart)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (isTagStart)
                    _isTagToAdd = (_headerTags.ContainsKey(key) ? false : true);

                if (_isTagToAdd)
                    if (_headerTags.ContainsKey(key)) _headerTags[key] += value;
                    else _headerTags.Add(key, value);
            }
        }
        private void AddTagSubToDict(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (_headerTagSubs.ContainsKey(key))
                    _headerTagSubs[key] += string.Format("|{0}", value);
                else _headerTagSubs.Add(key, value);
            }
        }
        private void AddSplitSegmentIDs(string segmentID)
        {
            if (segmentID.Length > 0)
                if (!_segmentIDsFound.Contains(segmentID))
                    _segmentIDsFound.Add(segmentID);
        }
        #endregion

        private bool isGroupWordsCountable(string segStatus, string segPerc, string segLocked)
        {
            int segPercNum = -1;
            int.TryParse(segPerc, out segPercNum);

            if (string.IsNullOrEmpty(segLocked))
                if (isStatusCountable(TagSegStatus.getTagSegStatus(segStatus)))
                {
                    if (!_options.IsPercent || (_options.IsPercent && segPercNum < _options.PercMax))
                        return true;
                }

            return false;
        }
        private bool isStatusCountable(SegStatus segStatus)
        {
            if (_options.SplitNonCountStatus.Contains(segStatus))
                return false;
            return true;
        }

        #region writer methods
        private void WriteToFile(string content)
        {
            //try
            //{
                _writer.Write(content);
            //}
            //catch (Exception ex)
            //{
            //    _writer.Dispose();
            //    RollbackFiles();
            //    throw new IOException(string.Format("Error when writing to file: {0}", ex.Message));
            //}
        }
        private void WriteToNewFile()
        { 
            // finish writing to old file
            WriteToFile(string.Concat(TagDirectory.BodyEnd, 
                TagDirectory.FileEnd, 
                TagDirectory.XliffEnd));
            _writer.Close();

            // read & write split file again to add tags into <tag_defs>
            if (_headerTags.Count > 0)
                UpdateFile(false);

            // start writing to new file
            _outFCount++;

            _writer = new StreamWriter(currFileOut);
            WriteToFile(string.Concat(new string[] { _xliffDeclar,
                _xliffTag,
                _docinfo.ToString(),
                _fileTag,
                _header.ToString(),
                TagDirectory.BodyStart + '>'
            }));
            _infoWriter.WriteFileTag(Path.GetFileName(currFileOut));
        }
        /// <summary>
        /// isLastFileEmpty - if true - we delete last file and update previous,
        /// if false - we update last file as usual
        /// </summary>
        /// <param name="isLastFileEmpty"></param>
        private void UpdateFile(bool isLastFileEmpty)
        {
            bool anyVal = false; // << to encrease performance
            Dictionary<string, bool> addedTags = new Dictionary<string, bool>();

            if (isLastFileEmpty)
                _outFCount--;

            // to read split file
            StreamFeeder splitFFeeder = new StreamFeeder(currFileOut);
            TagParser splitFParser = new TagParser(splitFFeeder);

            // to create split file writer
            FileHelper.CheckPath(tempUpdFilePath, true);
            _writer = new StreamWriter(tempUpdFilePath);

            while (splitFParser.Next())
            {
                
                // read all TUs from last file and write to previous
                if (isLastFileEmpty && splitFParser.TagID == Tags.BodyEnd)
                {
                    _outFCount++;

                    StreamFeeder lastFFeeder = new StreamFeeder(currFileOut);
                    TagParser lastFParser = new TagParser(lastFFeeder);

                    while (lastFParser.Next())
                        if (lastFParser.IsInBody && lastFParser.TagID != Tags.BodyStart && lastFParser.TagID != Tags.BodyEnd)
                            WriteToFile(lastFParser.ParsedText);

                    // delete last file
                    if (isLastFileEmpty)
                        File.Delete(currFileOut);
                    _outFCount--;
                }

                // always write to file everything
                WriteToFile(splitFParser.ParsedText);

                // add tags in tag_defs
                if (splitFParser.TagID == Tags.TagDefsStart)
                {
                    addedTags = new Dictionary<string, bool>();

                    // find tags that need to be written
                    // write tag text to file
                    foreach (KeyValuePair<string, string> tag in _headerTags)
                    {
                        bool isTagToWrite = true;
                        if (_headerTagSubs != null && _headerTagSubs.ContainsKey(tag.Key))
                        {
                            string[] tagTUs = _headerTagSubs[tag.Key].Split('|');
                            foreach (string tagTU in tagTUs)
                                if (_splitFileTUs.TryGetValue(tagTU, out anyVal)) { }
                                else
                                { isTagToWrite = false; break;  }
                        }

                        if (isTagToWrite)
                        {
                            if (!isLastFileEmpty)
                            {
                                addedTags.Add(tag.Key, false);
                                WriteToFile(tag.Value);
                            }
                            else if (_prevFileTags != null && _prevFileTags.TryGetValue(tag.Key, out anyVal))
                            { }
                            else
                            {
                                addedTags.Add(tag.Key, false);
                                WriteToFile(tag.Value);
                            }
                        }
                    }
                }
            }

            // restore the list of tags added to file
            _prevFileTags = addedTags;

            splitFFeeder.Dispose();
            _writer.Dispose();

            RenameUpdFiles();
        }

        private void RenameUpdFiles()
        {
            if (File.Exists(tempUpdFilePath))
            {
                FileHelper.CheckPath(currFileOut, true);
                File.Move(tempUpdFilePath, currFileOut);
            }
        }
        private void RollbackFiles()
        {
            string dir = Path.GetDirectoryName(currFileOut);
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);

            _outFCount = -1;
        }
        #endregion

        private void ProgressSplit(double currProgress, int operationOrder)
        {
            if (this.OnProgress != null)
            {
                switch (operationOrder)
                {
                    case 1:
                        this.OnProgress(currProgress / 2);
                        break;
                    case 2:
                        if (_options.Criterion == SplitOptions.SplitType.EqualParts)
                            this.OnProgress(50 + currProgress / 2);
                        else this.OnProgress(currProgress);
                        break;
                }
            }
        }
        #endregion
    }

}
