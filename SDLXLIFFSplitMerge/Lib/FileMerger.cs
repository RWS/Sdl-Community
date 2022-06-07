using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    public class FileMerger
    {
        private const string _filesExt = "sdlxliff";
        private string _infoFileExt = Properties.StringResource.extSplitInfo;
        private string _fileBackupExt = Properties.StringResource.extBackup;

        public delegate void OnProgressDelegate(double progress);
        public event OnProgressDelegate OnProgress;

        #region Fields
        private string _origFilePath;
        private string _infoFilePath;
        private string _inFilesPath;

        StreamFeeder _infoFFeeder;
        TagParser _infoFParser;

        private StreamWriter _writer;
        private string _fileOrigHash;
        private string[] _inFiles;

        private Dictionary<string, string> _headerComments = new Dictionary<string, string>();
        private List<string> _bodyCommentIDs = new List<string>();
        private List<string> _fileCommentIDs = new List<string>();
        private string _origFileCommentID;
        private bool _isFileCommentNew;
        private bool _isCmtToAdd;

        private Dictionary<string, string> _headerNewTags = new Dictionary<string, string>();
        private Dictionary<string, bool> _fileTagIDs = new Dictionary<string, bool>();
        private bool _isTagToAdd;

        private Dictionary<string, string> _addedFmts= new Dictionary<string, string>();
        private Dictionary<string, string> _headerNewFmts = new Dictionary<string, string>();
        private Dictionary<string, string> _splitFFmts;
        private int _origFileFmtID;

        private int? _outFCount;
        private List<string> _mergedFName;
        #endregion

        #region Props
        public int? FilesCount
        {
            get { return _outFCount; }
        }

        private string tempWriteFilePath
        {
            get
            {
                return string.Format(@"{0}\{1}{2}", Path.GetDirectoryName(_origFilePath),
                    _fileOrigHash, Path.GetExtension(_origFilePath));
            }
        }
        private string tempUpdFilePath
        {
            get
            {
                return string.Format(@"{0}\{1}_comm{2}", Path.GetDirectoryName(_origFilePath),
                    _fileOrigHash, Path.GetExtension(_origFilePath));
            }
        }
        #endregion

        #region Constructors
        public FileMerger(FileParser fileParser)
        {
            _origFilePath = fileParser.FilePath;
            _inFilesPath = fileParser.OutPath;
            _infoFilePath = string.Format(@"{0}\{1}.{2}", _inFilesPath, Path.GetFileNameWithoutExtension(_origFilePath), _infoFileExt);
        }
        public FileMerger(string origFilePath, string inFilesPath, string infoFilePath)
        {
            _origFilePath = origFilePath;
            _inFilesPath = inFilesPath;
            _infoFilePath = infoFilePath;
        }
        #endregion

        #region Public
        public void Merge()
        {
            // get hash from original file
            _fileOrigHash = FileHelper.SHA1HashFile(_origFilePath);

            ValidateFields();

            // TODO Sort files if needed
            // get all split files
            _inFiles = Directory.GetFiles(_inFilesPath, string.Format("*{0}.{1}", _fileOrigHash, _filesExt));
            DoMerge();

            if (_headerComments.Count > 0 || _headerNewTags.Count > 0)
                UpdateFile();

            // if succeeded - get number of files processed for merging
            _outFCount = _mergedFName.Count;
        }
        #endregion

        #region Private
        private void DoMerge()
        {
            string fileName = "";
            bool isTUFound = false;
            bool isNewTagFound = false;
            bool anyVal = false; // << to encrease performance
            int tuNumber = 0;
            int tuCount = 0;
            int wordCount = 0;
            _mergedFName = new List<string>();
            _origFileCommentID = Guid.NewGuid().ToString();

            // create perser for split info file
            _infoFFeeder = new StreamFeeder(_infoFilePath);
            _infoFParser = new TagParser(_infoFFeeder);

            // to create merged file writer
            FileHelper.CheckPath(tempWriteFilePath, true);
            _writer = new StreamWriter(tempWriteFilePath);

            // to read original file
            StreamFeeder origFFeeder = new StreamFeeder(_origFilePath);
            TagParser origFParser = new TagParser(origFFeeder);

            // to read split files
            string splitFName = Path.GetFileName(_inFiles[0]);
            StreamFeeder splitFFeeder = new StreamFeeder(string.Format(@"{0}\{1}", _inFilesPath, splitFName));
            TagParser splitFParser = new TagParser(splitFFeeder);
            _splitFFmts = new Dictionary<string, string>();
            _mergedFName.Add(splitFName);

            while (origFParser.Next())
            {
                if (!origFParser.IsInTransUnit && !origFParser.IsInCmtDef)
                {
                    // always write to file everything but trans-units and comments as exception
                    WriteToFile(origFParser.ParsedText);

                    // read comment id in header (file level comments)
                    if (origFParser.IsInFileCmtClosed && !string.IsNullOrEmpty(origFParser.FileCmtIDAttr))
                        _origFileCommentID = origFParser.FileCmtIDAttr;

                    // read tag id in header
                    if (origFParser.TagID == Tags.TagStart && !string.IsNullOrEmpty(origFParser.TagIDAttr))
                        AddTagIDToList(origFParser.TagIDAttr);

                    // read fmt-def id in header
                    if (origFParser.TagID == Tags.FmtDefStart && !string.IsNullOrEmpty(origFParser.FmtDefIDAttr))
                        int.TryParse(origFParser.FmtDefIDAttr, out _origFileFmtID);

                    // read fmt-defs written to merged file
                    if (origFParser.IsInFmtDef)
                        AddFmtDefOrigToDict(origFParser.FmtDefIDAttr, origFParser.ParsedText);
                }
                else if (origFParser.TagID == Tags.TransUnitStart)
                {
                    // check if current TU number >= number of all TUs in split file
                    if (tuCount == 0 || tuNumber >= tuCount)
                    {
                        // find next split file name
                        fileName = getFileNameFromSplitInfo();
                        tuCount = getTUCountFromSplitInfo();
                        wordCount = getWordCountFromSplitInfo();
                        tuNumber = 0;

                        if (wordCount > 0)
                        {
                            splitFName = "";

                            // validate
                            if (string.IsNullOrEmpty(fileName) || !File.Exists(getInFilePath(fileName)))
                                throw new FileNotFoundException(string.Format(Properties.StringResource.errSplitFileNotFound, fileName));
                            else if (getInFileHash(fileName) != _fileOrigHash)
                                throw new InvalidDataException(Properties.StringResource.errCanNotMerge);
                            else
                            {
                                // open new split file
                                splitFName = fileName;
                                splitFParser.Dispose();
                                splitFFeeder = new StreamFeeder(string.Format(@"{0}\{1}", _inFilesPath, splitFName));
                                splitFParser = new TagParser(splitFFeeder);
                                _splitFFmts = new Dictionary<string, string>();

                                if (!_mergedFName.Contains(splitFName))
                                    _mergedFName.Add(splitFName);
                            }
                        }
                    }

                    isTUFound = false;
                    isNewTagFound = false;
                    if (splitFName.Length > 0)
                        while (splitFParser.Next())
                        {
                            // read trans-units
                            if (splitFParser.IsInTransUnit)
                            {
                                // write to file trans-units from split file
                                WriteToFile(splitFParser.ParsedText);

                                // read comment ids in body (segment level comments)
                                if (splitFParser.IsInMrkComm && splitFParser.TagID == Tags.MrkStart && !string.IsNullOrEmpty(splitFParser.MrkCommCIDAttr))
                                    AddCommentIDToList(splitFParser.MrkCommCIDAttr);

                                if (splitFParser.TagID == Tags.TransUnitEnd)
                                { isTUFound = true; break; }
                            }
                            // read comments in doc-info
                            else if (splitFParser.IsInCmtDef && !string.IsNullOrEmpty(splitFParser.CmtDefIDAttr))
                                AddCommentToDict(splitFParser.CmtDefIDAttr, splitFParser.ParsedText, (splitFParser.TagID == Tags.CmtDefStart));

                            // read comment ids in header (file level comments)
                            else if (splitFParser.IsInFileCmtClosed && !string.IsNullOrEmpty(splitFParser.FileCmtIDAttr))
                                AddHdrCommentIDToList(splitFParser.FileCmtIDAttr);

                            // read tags in header -- optimized
                            else if (splitFParser.IsInTag)
                            {
                                if (splitFParser.TagID == Tags.TagStart)
                                {
                                    if (_fileTagIDs.TryGetValue(splitFParser.TagIDAttr, out anyVal))
                                        isNewTagFound = false;
                                    else isNewTagFound = true;
                                }
                                //else if (splitFParser.IsInTag && !_fileTagIDs.Contains(splitFParser.TagIDAttr))
                                if (isNewTagFound)
                                {
                                    string pText = splitFParser.ParsedText;
                                    if (splitFParser.IsInFmtClosed)
                                    {
                                        // change format tag id
                                        pText = string.Format("{0} {1}=\"{2}\"/>", TagDirectory.FmtStart, TagDirectory.AttrID, FindFmtDef(splitFParser.FmtIDAttr));
                                    }
                                    AddTagToDict(splitFParser.TagIDAttr, pText, (splitFParser.TagID == Tags.TagStart));
                                }
                                if (splitFParser.TagID == Tags.TagEnd)
                                    isNewTagFound = false;
                            }

                            // read fmt-defs in header
                            else if (splitFParser.IsInFmtDef && !string.IsNullOrEmpty(splitFParser.FmtDefIDAttr))
                                AddFmtDefToDict(splitFParser.FmtDefIDAttr, splitFParser.ParsedText);

                        }

                    if (!isTUFound)
                        throw new InvalidDataException(string.Format(Properties.StringResource.errTUNotFound,
                            origFParser.TransUnitIDAttr,
                            splitFName, tuNumber));

                    tuNumber++;

                    #region older version
                    //// get trans-unit id
                    //tuID = origFParser.TransUnitIDAttr;

                    //// find split file name for current trans-unit
                    //tuFileName = getFileNameFromSplitInfo(tuID);
                    //if (string.IsNullOrEmpty(tuFileName) || !File.Exists(getInFilePath(tuFileName)))
                    //    throw new FileNotFoundException(string.Format("Split file '{0}' not found.", tuFileName));
                    //else if (getInFileHash(tuFileName) != _fileOrigHash)
                    //    throw new InvalidDataException("Cannot merge files. Possible reasons: " +
                    //          "1 - Original file was modified after the split. 2 - Split files do not correspond the indicated original file.");
                    //else
                    //{
                    //    // create reader for new split file
                    //    if (!string.Equals(tuFileName, splitFName))
                    //    {
                    //        splitFName = tuFileName;
                    //        splitFParser.Dispose();
                    //        splitFFeeder = new StreamFeeder(string.Format(@"{0}\{1}", _inFilesPath, splitFName));
                    //        splitFParser = new TagParser(splitFName);

                    //        if (!_mergedFName.Contains(splitFName))
                    //            _mergedFName.Add(splitFName);
                    //    }

                    //    // find trans-unit with current id
                    //    // write the whole trans-unit to merged file
                    //    isTUFound = false;
                    //    fileReads = 0;
                    //    while (!isTUFound && fileReads < 2)
                    //    {
                    //        while (splitFParser.Next())
                    //        {
                    //            // read trans-units
                    //            if (splitFParser.IsInTransUnit && splitFParser.TransUnitIDAttr == tuID)
                    //            {
                    //                // write to file trans-units from split file
                    //                WriteToFile(splitFParser.ParsedText);

                    //                // read comment ids in body (segment level comments)
                    //                if (splitFParser.IsInMrkComm && splitFParser.TagID == Tags.MrkStart && !string.IsNullOrEmpty(splitFParser.MrkCommCIDAttr))
                    //                    AddCommentIDToList(splitFParser.MrkCommCIDAttr);

                    //                if (splitFParser.TagID == Tags.TransUnitEnd)
                    //                { isTUFound = true; break; }
                    //            }
                    //            // read comments in doc-info
                    //            else if (splitFParser.IsInCmtDef && !string.IsNullOrEmpty(splitFParser.CmtDefIDAttr))
                    //                AddCommentToDict(splitFParser.CmtDefIDAttr, splitFParser.ParsedText, (splitFParser.TagID == Tags.CmtDefStart));

                    //            // read comment ids in header (file level comments)
                    //            else if (splitFParser.IsInFileCmtClosed && !string.IsNullOrEmpty(splitFParser.FileCmtIDAttr))
                    //                AddHdrCommentIDToList(splitFParser.FileCmtIDAttr);
                    //        }

                    //        if (!isTUFound)
                    //        {
                    //            // start reading this file again
                    //            fileReads++;
                    //            splitFParser = new TagParser(splitFFeeder);
                    //        }
                    //    }

                    //    // if trans-unit not found - throw ex
                    //    if (!isTUFound)
                    //        throw new InvalidDataException(string.Format("Trans-unit '{0}' not found in file '{1}'.", tuID, splitFName));
                    //}
                    #endregion
                }

                // report current operation progress
                ProgressMerge(origFParser.Progress());
            }

            origFParser.Dispose();
            splitFParser.Dispose();

            _infoFParser.Dispose();
            _writer.Dispose();

            RenameFiles();
        }

        private void ValidateFields()
        {
            // input file path/name
            if (!Directory.Exists(Path.GetDirectoryName(_origFilePath)) || !File.Exists(_origFilePath))
                throw new FileNotFoundException(string.Format(Properties.StringResource.errFileNotExist, _origFilePath));

            // info file path/name
            if (!Directory.Exists(Path.GetDirectoryName(_infoFilePath)) || !File.Exists(_infoFilePath))
                throw new FileNotFoundException(string.Format(Properties.StringResource.errFileNotExist, _infoFilePath));

            // input (split) files path
            if (!Directory.Exists(_inFilesPath))
                throw new DirectoryNotFoundException(string.Format(Properties.StringResource.errDirectoryNotExist, _inFilesPath));
            if (Directory.GetFiles(_inFilesPath, string.Format("*{0}.{1}", _fileOrigHash, _filesExt)).Length < 1)
                throw new FileNotFoundException(string.Format(Properties.StringResource.errNoFilesToMerge, _inFilesPath));
        }

        private void AddCommentToDict(string key, string value, bool isStart)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (isStart)
                    _isCmtToAdd = (_headerComments.ContainsKey(key) ? false : true);

                if (_isCmtToAdd)
                    if (_headerComments.ContainsKey(key)) _headerComments[key] += value;
                    else _headerComments.Add(key, value);
            }
        }
        private void AddTagToDict(string key, string value, bool isStart)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (isStart)
                    _isTagToAdd = (_headerNewTags.ContainsKey(key) ? false : true);

                if (_isTagToAdd)
                    if (_headerNewTags.ContainsKey(key)) _headerNewTags[key] += value;
                    else _headerNewTags.Add(key, value);
            }
        }
        private void AddFmtDefToDict(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (_splitFFmts.ContainsKey(key)) _splitFFmts[key] += value;
                else _splitFFmts.Add(key, value);
            }
        }
        private void AddFmtDefOrigToDict(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (_addedFmts.ContainsKey(key)) _addedFmts[key] += value;
                else _addedFmts.Add(key, value);
            }
        }
        private void AddCommentIDToList(string cmtID)
        {
            if (!_bodyCommentIDs.Contains(cmtID))
                _bodyCommentIDs.Add(cmtID);
        }
        private void AddHdrCommentIDToList(string cmtID)
        {
            if (!_fileCommentIDs.Contains(cmtID))
                _fileCommentIDs.Add(cmtID);
        }
        private void AddTagIDToList(string tagID)
        {
            if (!_fileTagIDs.ContainsKey(tagID))
                _fileTagIDs.Add(tagID, false);
        }

        private string FindFmtDef(string ID)
        {
            string newID = "";
            if (_splitFFmts.ContainsKey(ID))
            {
                // try to find format among added
                foreach (KeyValuePair<string, string> _addedFmt in _addedFmts)
                    if (_addedFmt.Value.Replace(string.Format(" {0}=\"{1}\">", TagDirectory.AttrID, _addedFmt.Key), string.Format(" {0}=\"{1}\">", TagDirectory.AttrID, ID)) == _splitFFmts[ID])
                    { newID = _addedFmt.Key; break; }
                if (newID.Length == 0)
                {
                    // add new format to add
                    newID = (++_origFileFmtID).ToString();
                    _headerNewFmts.Add(newID, _splitFFmts[ID].Replace(string.Format(" {0}=\"{1}\">", TagDirectory.AttrID, ID),
                        string.Format(" {0}=\"{1}\">", TagDirectory.AttrID, newID)));
                    AddFmtDefOrigToDict(newID, _headerNewFmts[newID]);
                }
            }
            return newID;
        }

        private void UpdateFile()
        {
            // to read original file
            StreamFeeder origFFeeder = new StreamFeeder(_origFilePath);
            TagParser origFParser = new TagParser(origFFeeder);

            // to create merged file writer
            FileHelper.CheckPath(tempUpdFilePath, true);
            _writer = new StreamWriter(tempUpdFilePath);

            NormalizeFileComments();

            bool isCmtFound = false;
            bool isTagFound = false;
            bool isFmtFound = false;
            while (origFParser.Next())
            {
                // add comments in header
                // add fmt-defs in header
                // add tags in header
                if (origFParser.TagID == Tags.HeaderEnd)
                {
                    if (!isFmtFound && _headerNewFmts.Count > 0)
                    {
                        WriteToFile(string.Format("{0}>", TagDirectory.FmtDefsStart));
                        WriteFmtsToFile();
                        WriteToFile(TagDirectory.FmtDefsEnd);
                    }
                    if (!isTagFound && _headerNewTags.Count > 0)
                    {
                        WriteToFile(string.Format("{0}>", TagDirectory.TagDefsStart));
                        WriteTagsToFile();
                        WriteToFile(TagDirectory.TagDefsEnd);
                    }
                    if (_isFileCommentNew)
                        WriteToFile(string.Format("{0} {1}=\"{2}\" />", TagDirectory.FileCmtStart,
                            TagDirectory.AttrID, _origFileCommentID));
                }

                // add comments in doc-info
                if (origFParser.TagID == Tags.DocInfoEnd && _headerComments.Count > 0 && !isCmtFound)
                {
                    WriteToFile(string.Format("{0}>", TagDirectory.CmtsStart));
                    WriteCmtsToFile();
                    WriteToFile(TagDirectory.CmtsEnd);
                    isCmtFound = true;
                }
                if (origFParser.TagID == Tags.FileStart && _headerComments.Count > 0 && !isCmtFound)
                {
                    WriteToFile(string.Format("{0}  {1}=\"{2}\">", TagDirectory.DocInfoStart, TagDirectory.AttrXmlNs, TagDirectory.AttrXmlNsValue));
                    WriteToFile(string.Format("{0}>", TagDirectory.CmtsStart));
                    WriteCmtsToFile();
                    WriteToFile(TagDirectory.CmtsEnd);
                    WriteToFile(TagDirectory.DocInfoEnd);
                }

                // add fmts in header
                if (origFParser.TagID == Tags.FmtDefsEnd && _headerNewFmts.Count > 0)
                {
                    WriteFmtsToFile();
                    isFmtFound = true;
                }

                // --- always write to file everything ---
                WriteToFile(origFParser.ParsedText);

                // add comments in doc-info
                if (origFParser.TagID == Tags.CmtsStart && _headerComments.Count > 0)
                {
                    WriteCmtsToFile(); 
                    isCmtFound = true;
                }
                // add tags in header
                if (origFParser.TagID == Tags.TagDefsStart && _headerNewTags.Count > 0)
                {
                    WriteTagsToFile();
                    isTagFound = true;
                }
            }

            origFParser.Dispose();
            _writer.Dispose();

            RenameUpdFiles();
        }
        private void NormalizeFileComments()
        {
            Dictionary<string, string> fileCommentTag = new Dictionary<string, string>();
            XmlDocument cmtDoc;
            XmlElement cmtRoot;

            string fileCmtKey = "";
            if (_fileCommentIDs.Count > 0)
                foreach (KeyValuePair<string, string> _comment in _headerComments)
                    if (_fileCommentIDs.Contains(_comment.Key))
                    {
                        fileCmtKey = _comment.Key;

                        // analyze comment & add unique only to temp dictionary of comments
                        cmtDoc = new XmlDocument();
                        cmtDoc.LoadXml(_comment.Value);

                        cmtRoot = cmtDoc.DocumentElement;
                        XmlNodeList cmtNodes = cmtRoot.SelectNodes(string.Format("/{0}/{1}/{2}",
                            TagDirectory.CmtDefStart.NodeStartName(),
                            TagDirectory.CommentsStart.NodeStartName(),
                            TagDirectory.CommentStart.NodeStartName()));

                        string cmtKey = "";
                        foreach (XmlNode cmt in cmtNodes)
                        {
                            cmtKey = ((XmlElement)cmt).GetAttribute(TagDirectory.AttrUser) + ((XmlElement)cmt).GetAttribute(TagDirectory.AttrDate);
                            if (!fileCommentTag.ContainsKey(cmtKey))
                                fileCommentTag.Add(cmtKey, cmt.OuterXml);
                        }
                    }

            if (!string.IsNullOrEmpty(fileCmtKey))
            {
                StringBuilder comments = new StringBuilder();
                foreach (KeyValuePair<string, string> fileCmt in fileCommentTag)
                    comments.Append(fileCmt.Value);

                // add file comment generated to list
                cmtDoc = new XmlDocument();
                cmtDoc.LoadXml(_headerComments[fileCmtKey]);
                cmtRoot = cmtDoc.DocumentElement;

                XmlNode cmts = cmtRoot.SelectSingleNode(string.Format("/{0}/{1}",
                    TagDirectory.CmtDefStart.NodeStartName(),
                    TagDirectory.CommentsStart.NodeStartName()));
                cmts.InnerXml = comments.ToString();

                // add real file comments item
                if (_headerComments.ContainsKey(_origFileCommentID))
                    _headerComments[_origFileCommentID] = cmtDoc.InnerXml.Replace(fileCmtKey, _origFileCommentID);
                else
                {
                    _isFileCommentNew = true;
                    _headerComments.Add(_origFileCommentID, cmtDoc.InnerXml.Replace(fileCmtKey, _origFileCommentID));
                }
            }
        }

        private void WriteToFile(string content)
        {
            _writer.Write(content);
        }
        private void WriteCmtsToFile()
        {
            foreach (KeyValuePair<string, string> _comment in _headerComments)
                if (_bodyCommentIDs.Contains(_comment.Key) || _comment.Key == _origFileCommentID)
                    WriteToFile(_comment.Value);
        }
        private void WriteTagsToFile()
        {
            foreach (KeyValuePair<string, string> _tag in _headerNewTags)
                WriteToFile(_tag.Value);
        }
        private void WriteFmtsToFile()
        {
            foreach (KeyValuePair<string, string> _fmt in _headerNewFmts)
                WriteToFile(_fmt.Value);
        }
        private void RenameFiles()
        {
            if (File.Exists(tempWriteFilePath))
            {
                // rename original file to backup
                string backupFile = string.Format("{0}.{1}", _origFilePath, _fileBackupExt);
                FileHelper.CheckPath(backupFile, true);
                File.Move(_origFilePath, backupFile);

                // rename merged file to original file name
                File.Move(tempWriteFilePath, _origFilePath);
            }
        }
        private void RenameUpdFiles()
        {
            if (File.Exists(tempUpdFilePath))
            {
                FileHelper.CheckPath(_origFilePath, true);
                File.Move(tempUpdFilePath, _origFilePath);
            }
        }

        //private string getFileNameFromSplitInfo(string transUnitID)
        //{
        //    string fName = "";

        //    int infoFileReads = 0;
        //    bool isInfoTUFound = false;
        //    while (!isInfoTUFound && infoFileReads < 2)
        //    {
        //        while (_infoFParser.Next())
        //            if (_infoFParser.IsInTransUnitClosed && _infoFParser.TransUnitIDAttr == transUnitID)
        //            {
        //                fName = _infoFParser.FileNameAttr;
        //                isInfoTUFound = true; break;
        //            }

        //        if (!isInfoTUFound)
        //        {
        //            // start reading this file again
        //            infoFileReads++;
        //            _infoFParser = new TagParser(_infoFFeeder);
        //        }
        //    }

        //    return fName;
        //}
        private string getFileNameFromSplitInfo()
        {
            string fName = "";

            bool isFileFound = false;
            while (!isFileFound && _infoFParser.Next())
            {
                if (_infoFParser.TagID == Tags.FileStart && !string.IsNullOrEmpty(_infoFParser.FileNameAttr))
                {
                    fName = _infoFParser.FileNameAttr;
                    isFileFound = true; break;
                }
            }

            return fName;
        }
        private int getTUCountFromSplitInfo()
        {
            int tusCount = 0;

            bool isFileFound = false;
            while (!isFileFound && _infoFParser.Next())
            {
                if (_infoFParser.IsInTransUnits && !_infoFParser.IsTag)
                {
                    int.TryParse(_infoFParser.ParsedText, out tusCount);
                    isFileFound = true; break;
                }
            }

            return tusCount;
        }
        private int getWordCountFromSplitInfo()
        {
            int wordsCount = 0;

            bool isFileFound = false;
            while (!isFileFound && _infoFParser.Next())
            {
                if (_infoFParser.IsInWords && !_infoFParser.IsTag)
                {
                    int.TryParse(_infoFParser.ParsedText, out wordsCount);
                    isFileFound = true; break;
                }
            }

            return wordsCount;
        }

        private string getInFilePath(string fName)
        {
            return string.Format(@"{0}\{1}", _inFilesPath, fName);
        }
        private string getInFileHash(string fName)
        { 
            string[] fNameParts =  Path.GetFileNameWithoutExtension(getInFilePath(fName)).Split('_');
            return fNameParts.Length > 1 ? fNameParts[1] : "";
        }

        private void ProgressMerge(double currProgress)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(currProgress);
            }
        }
        #endregion
    }
}
