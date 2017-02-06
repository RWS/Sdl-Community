using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Sdl.Community.PostEdit.Compare.Core;

namespace PostEdit.Compare.Model
{

    public class Model : IModel
    {
        private readonly List<DataNode> _mDataPool;

        private readonly Dictionary<string, string> _fileTypeNames;

        public Model()
        {
            _mDataPool = new List<DataNode>(64);
            _fileTypeNames = new Dictionary<string, string>();
        }

        public List<DataNode> DataPool
        {
            get { return _mDataPool; }
        }

        public long TotalFoldersLeft { get; set; }
        public long TotalFoldersRight { get; set; }
        public long TotalFilesLeft { get; set; }
        public long TotalFilesRight { get; set; }


        ProgressObject ProgressObject { get; set; }
        private int ProgressPercentage { get; set; }
        private bool SetProgressStatus { get; set; }

        private readonly System.Timers.Timer _timer = new System.Timers.Timer();

        private enum FolderCompareType
        {
            LeftSide,
            RightSide,
            BothSides
        }

        public List<Settings.FileAlignment> FileAlignments { get; set; }


        private string StartDirectoryPathLeft { get; set; }
        private string StartDirectoryPathRight { get; set; }


        public void ImportDataModel(string directoryPathLeft, string directoryPathRight)
        {

            StartDirectoryPathLeft = directoryPathLeft;
            StartDirectoryPathRight = directoryPathRight;

            TotalFoldersLeft = 0;
            TotalFoldersRight = 0;
            TotalFilesLeft = 0;
            TotalFilesRight = 0;

            ProgressPercentage = 0;
            SetProgressStatus = true;
            _timer.Interval = 250;
            _timer.Elapsed+= timer_Elapsed;

            try
            {
                _timer.Start();

                if (string.Compare(directoryPathLeft, directoryPathRight, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ObtainModel(directoryPathLeft, FolderCompareType.BothSides);
                }
                else
                {
                    ObtainModel(directoryPathLeft, FolderCompareType.LeftSide);
                    ObtainModel(directoryPathRight, FolderCompareType.RightSide);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _timer.Stop();
            }

        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ProgressPercentage != 100)
                ProgressPercentage++;
            else
                ProgressPercentage = 1;

            SetProgressStatus = true;
        }


        private void ObtainModel(string directoryPath, FolderCompareType folderCompareType)
        {
            var level = 0;

            var directories = new List<string>();
            var _directories = Directory.GetDirectories(directoryPath);
            directories.AddRange(_directories);
            directories.Sort();

            var files = new List<string>();
            var _files = Directory.GetFiles(directoryPath);
            files.AddRange(_files);
            files.Sort();


            #region  |  directories |

            
            foreach (var directory in directories)
            {

                if (folderCompareType == FolderCompareType.LeftSide || folderCompareType == FolderCompareType.BothSides)
                    TotalFoldersLeft++;
                else
                    TotalFoldersRight++;


                #region  |  progressObject  |


                if (SetProgressStatus)
                {
                    SetProgressStatus = false;

                    ProgressObject = new ProgressObject
                    {
                        ProgessTitle = "Processing Folders...",
                        CurrentProcessingMessage =
                            String.Format("Searching/Aligning... folders/files (") +
                            (folderCompareType == FolderCompareType.LeftSide ||
                             folderCompareType == FolderCompareType.BothSides
                                ? "left side"
                                : "right side") + ")",
                        TotalProgressValueMessage =
                            string.Format("Searched/Aligned {0} directories and {1} files",
                                (TotalFoldersLeft + TotalFoldersRight), (TotalFilesLeft + TotalFilesRight))
                    };


                    WaitingWindow.WaitingDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);
                }

                #endregion


                long folderSize = 0;


                try
                {
                    var dirInfo = new DirectoryInfo(directory);
                    const bool expanded = false;


                    var folderCompareState = DataNode.CompareStates.None;
                    const DataNode.ItemType itemType = DataNode.ItemType.Folder;

                    DataNode m;

                    switch (folderCompareType)
                    {
                        case FolderCompareType.LeftSide:

                            m = new DataNode(dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , string.Empty, string.Empty, -1, dirInfo.LastWriteTime
                                , folderCompareState, level, expanded, itemType);
                            break;
                        case FolderCompareType.RightSide:
                            m = new DataNode(string.Empty, string.Empty, -1, dirInfo.LastWriteTime
                                , dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , folderCompareState, level, expanded, itemType);
                            break;
                        default:
                            m = new DataNode(dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , folderCompareState, level, expanded, itemType);
                            break;
                    }



                    var found = false;
                    if (folderCompareType == FolderCompareType.RightSide)
                    {
                        foreach (var _m in DataPool)
                        {
                            if (string.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                                continue;
                            found = true;
                            _m.NameRight = m.NameRight;
                            _m.PathRight = m.PathRight;
                            _m.SizeRight = m.SizeRight;
                            _m.ModifiedRight = m.ModifiedRight;

                            _m.CompareState = _m.CompareState | DataNode.CompareStates.Equal;



                            folderCompareState = _m.CompareState;

                            m = _m;


                            break;
                        }
                    }


                    CollectNodes(m, dirInfo.FullName, ref level, folderCompareType, ref folderSize, ref folderCompareState);
                    switch (folderCompareType)
                    {
                        case FolderCompareType.LeftSide:
                            if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                                m.CompareState = DataNode.CompareStates.OrphansLeftside;
                            break;
                        case FolderCompareType.RightSide:

                            #region  |  set the folder compare state  |

                            m.CompareState = DataNode.CompareStates.None;

                            if (m.Children.Count == 0)
                            {
                                if (m.NameLeft.Trim() != string.Empty && m.NameRight.Trim() != string.Empty)
                                    m.CompareState = DataNode.CompareStates.Equal;
                                else if (m.NameLeft.Trim() == string.Empty && m.NameRight.Trim() != string.Empty)
                                    m.CompareState = DataNode.CompareStates.OrphansRightside;
                                else if (m.NameLeft.Trim() != string.Empty && m.NameRight.Trim() == string.Empty)
                                    m.CompareState = DataNode.CompareStates.OrphansLeftside;
                            }
                            else
                            {
                                foreach (var dn in m.Children)
                                {
                                    if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    {
                                        if ((m.CompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch
                                            && (m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside
                                            && (m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside
                                            && (m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside
                                            && (m.CompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                                        {
                                            m.CompareState = m.CompareState | DataNode.CompareStates.Equal;
                                        }
                                    }
                                    {
                                        if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.Mismatch;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.OrphansLeftside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.OrphansRightside) !=
                                            DataNode.CompareStates.OrphansRightside) continue;
                                        if ((m.CompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                                            m.CompareState = m.CompareState | DataNode.CompareStates.OrphansRightside;

                                        if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                            m.CompareState &= ~DataNode.CompareStates.Equal;
                                    }
                                }
                            }
                            break;

                            #endregion

                        default:
                            if ((m.CompareState & DataNode.CompareStates.Equal) != DataNode.CompareStates.Equal)
                                m.CompareState = DataNode.CompareStates.Equal;
                            break;
                    }



                    if (folderCompareType == FolderCompareType.LeftSide)
                        m.SizeLeft = folderSize;
                    else if (folderCompareType == FolderCompareType.RightSide)
                        m.SizeRight = folderSize;
                    else
                    {
                        m.SizeLeft = folderSize;
                        m.SizeRight = folderSize;

                    }

                    #region  |  found  |

                    if (!found)
                    {



                        if (folderCompareType == FolderCompareType.RightSide)
                        {
                            var index = 0;
                            foreach (var _m in DataPool)
                            {
                                if (_m.Type == 0)
                                {
                                    if (String.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) > 0)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (index == 0)
                                        index = -1;
                                    break;
                                }
                                index++;
                            }
                            if (index > 0)
                            {
                                DataPool.Insert(index, m);
                            }
                            else
                            {

                                DataPool.Add(m);
                            }
                        }
                        else
                        {
                            DataPool.Add(m);
                        }
                    }
                    #endregion


                }
                catch (Exception ex)
                {
                    level--;
                    Console.WriteLine(ex.Message);
                }
            }
            #endregion


            #region  |  files  |
            foreach (var file in files)
            {
                switch (folderCompareType)
                {
                    case FolderCompareType.LeftSide:
                        TotalFilesLeft++;
                        break;
                    case FolderCompareType.RightSide:
                        TotalFilesRight++;
                        break;
                    default:
                        TotalFilesLeft++;
                        TotalFilesRight++;
                        break;
                }




                #region  |  progressObject  |


                if (SetProgressStatus)
                {
                    SetProgressStatus = false;

                    ProgressObject = new ProgressObject
                    {
                        ProgessTitle = "Processing Folders...",
                        CurrentProcessingMessage =
                            "Searching/Aligning... folders/files (" +
                            (folderCompareType == FolderCompareType.LeftSide ||
                             folderCompareType == FolderCompareType.BothSides
                                ? "left side"
                                : "right side") + ")",
                        TotalProgressValueMessage =
                            string.Format("Searched/Aligned {0} directories and {1} files",
                                (TotalFoldersLeft + TotalFoldersRight), (TotalFilesLeft + TotalFilesRight))
                    };




                    WaitingWindow.WaitingDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);
                }

                #endregion


                var fileInfo = new FileInfo(file);

              


                const bool expanded = false;
               
                const DataNode.CompareStates fileCompareState = DataNode.CompareStates.None;
                const DataNode.ItemType itemType = DataNode.ItemType.File;

                DataNode m;


                switch (folderCompareType)
                {
                    case FolderCompareType.LeftSide:
                    {
                        var properties = string.Empty;
                        m = new DataNode(fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , string.Empty, string.Empty, -1, fileInfo.LastWriteTime
                            , fileCompareState, level, expanded, itemType);

                        #region  |  attributes  (left_side)  |
                        if ((fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "A";
                        }
                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "S";
                        }
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "H";
                        }
                        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "R";
                        }
                        #endregion
                    }
                        break;
                    case FolderCompareType.RightSide:
                    {
                        var properties = string.Empty;
                        m = new DataNode(string.Empty, string.Empty, -1, fileInfo.LastWriteTime
                            , fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , fileCompareState, level, expanded, itemType);

                        #region  |  attributes  (right_side)  |
                        if ((fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "A";
                        }
                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "S";
                        }
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "H";
                        }
                        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "R";
                        }
                        #endregion
                    }
                        break;
                    default:
                    {
                        var properties = string.Empty;
                        m = new DataNode(fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , fileCompareState, level, expanded, itemType);

                        #region  |  attributes  (both_side)  |
                        if ((fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "A";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "A";
                        }
                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "S";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "S";
                        }
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "H";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "H";
                        }
                        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "R";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "R";
                        }
                        #endregion
                    }
                        break;
                }

                if (m.FileType.Trim() == string.Empty)
                {
                    if (_fileTypeNames.ContainsKey(fileInfo.Extension))
                    {
                        m.FileType = _fileTypeNames[fileInfo.Extension];
                    }
                    else
                    {
                        var info = new FileType.SHFILEINFO();
                        const uint dwFileAttributes = FileType.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
                        const uint uFlags = (uint)(FileType.SHGFI.SHGFI_TYPENAME | FileType.SHGFI.SHGFI_USEFILEATTRIBUTES);

                        FileType.SHGetFileInfo(file, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
                     
                        m.FileType = (info.szDisplayName.Trim() != string.Empty ? info.szDisplayName : info.szTypeName);

                        _fileTypeNames.Add(fileInfo.Extension, m.FileType);  
                    }
                }

                var found = false;
                if (folderCompareType== FolderCompareType.RightSide)
                {
                    foreach (var _m in DataPool)
                    {

                        if (_m.PathRight.Trim() == string.Empty)
                        {
                            if (FileAlignments.Any(fa => string.Compare(_m.PathLeft, fa.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(m.PathRight, fa.PathRight, StringComparison.OrdinalIgnoreCase) == 0))
                            {
                                found = true;
                                _m.NameRight = m.NameRight;
                                _m.PathRight = m.PathRight;
                                _m.SizeRight = m.SizeRight;
                                _m.ModifiedRight = m.ModifiedRight;

                                if ((_m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                                    _m.CompareState &= ~DataNode.CompareStates.None;


                                if ((_m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                    _m.CompareState &= ~DataNode.CompareStates.OrphansLeftside;

                                if ((_m.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                    _m.CompareState &= ~DataNode.CompareStates.OrphansRightside;


                                if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) == 0
                                    && _m.ModifiedLeft == _m.ModifiedRight)
                                {
                                    _m.CompareState = _m.CompareState | DataNode.CompareStates.Equal;// 1;//0=equal; 1=different; 2=exist only on left; 3=exists only on right; 4=similar
                                }
                                else
                                {

                                    if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0)
                                    {
                                        _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;
                                    }
                                    else if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                                    {
                                        _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;
                                    }

                                    if (_m.SizeLeft != _m.SizeRight)
                                    {
                                        _m.CompareState = _m.CompareState | DataNode.CompareStates.Mismatch;
                                    }
                                    else
                                    {
                                        if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0
                                            || DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                                            _m.CompareState = _m.CompareState | DataNode.CompareStates.Similar;
                                    }
                                }

                                m = _m;
                            }
                        }

                        if (found) continue;
                        if (string.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                            continue;
                        found = true;
                        _m.NameRight = m.NameRight;
                        _m.PathRight = m.PathRight;
                        _m.SizeRight = m.SizeRight;
                        _m.ModifiedRight = m.ModifiedRight;

                        if ((_m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                            _m.CompareState &= ~DataNode.CompareStates.None;


                        if ((_m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                            _m.CompareState &= ~DataNode.CompareStates.OrphansLeftside;

                        if ((_m.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                            _m.CompareState &= ~DataNode.CompareStates.OrphansRightside;


                        if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) == 0
                            && _m.ModifiedLeft == _m.ModifiedRight)
                        {
                            _m.CompareState = _m.CompareState | DataNode.CompareStates.Equal;// 1;//0=equal; 1=different; 2=exist only on left; 3=exists only on right; 4=similar
                        }
                        else
                        {

                            if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0)
                            {
                                _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;
                            }
                            else if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                            {
                                _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;
                            }

                            if (_m.SizeLeft != _m.SizeRight)
                            {
                                _m.CompareState = _m.CompareState | DataNode.CompareStates.Mismatch;
                            }
                            else
                            {
                                if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0
                                    || DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                                    _m.CompareState = _m.CompareState | DataNode.CompareStates.Similar;
                            }
                        }

                        m = _m;


                        break;
                    }
                }

                #region  |  found  |

                if (found) continue;
                {
                    if ((m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                        m.CompareState &= ~DataNode.CompareStates.None;

                    switch (folderCompareType)
                    {
                        case FolderCompareType.RightSide:
                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                m.CompareState &= ~DataNode.CompareStates.Equal;

                            if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                m.CompareState &= ~DataNode.CompareStates.OrphansLeftside;

                            m.CompareState = m.CompareState | DataNode.CompareStates.OrphansRightside;//= 3;//0=equal; 1=different; 2=exist only on left; 3=exists only on right
                            break;
                        case FolderCompareType.LeftSide:
                            m.CompareState = m.CompareState | DataNode.CompareStates.OrphansLeftside;// 2;//0=equal; 1=different; 2=exist only on left; 3=exists only on right
                            break;
                        default:
                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                m.CompareState &= ~DataNode.CompareStates.Equal;
                            break;
                    }

                    if (folderCompareType== FolderCompareType.RightSide)
                    {
                        var index = 0;
                        foreach (var _m in DataPool)
                        {
                            if (_m.Type == DataNode.ItemType.File)
                            {
                                if (string.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    break;
                                }
                            }
                            index++;
                        }
                        if (index > 0)
                        {
                            DataPool.Insert(index, m);
                        }
                        else
                        {

                            DataPool.Add(m);
                        }
                    }
                    else
                    {
                        DataPool.Add(m);
                    }
                }

                #endregion

             
            }
            #endregion

        }


        private void CollectNodes(DataNode mbr, string directoryPath, ref int level, FolderCompareType folderCompareType, ref long folderSize, ref DataNode.CompareStates parentFolderCompareState)
        {

            level++;

            var directories = new List<string>();
            var _directories = Directory.GetDirectories(directoryPath);
            directories.AddRange(_directories);
            directories.Sort();

            var files = new List<string>();
            var _files = Directory.GetFiles(directoryPath);
            files.AddRange(_files);
            files.Sort();

            folderSize += files.Select(file => new FileInfo(file)).Select(fileInfo => fileInfo.Length).Sum();

            #region  |  directories  |
            foreach (var directory in directories)
            {
                switch (folderCompareType)
                {
                    case FolderCompareType.LeftSide:
                        TotalFoldersLeft++;
                        break;
                    case FolderCompareType.RightSide:
                        TotalFoldersRight++;
                        break;
                    default:
                        TotalFoldersLeft++;
                        TotalFoldersRight++;
                        break;
                }

                #region  |  progressObject  |


                if (SetProgressStatus)
                {
                    SetProgressStatus = false;

                    ProgressObject = new ProgressObject
                    {
                        ProgessTitle = "Processing Folders...",
                        CurrentProcessingMessage =
                            string.Format("Searching/Aligning... folders/files (") +
                            (folderCompareType == FolderCompareType.LeftSide ||
                             folderCompareType == FolderCompareType.BothSides
                                ? "left side"
                                : "right side") + ")",
                        TotalProgressValueMessage =
                            string.Format("Searched/Aligned {0} directories and {1} files",
                                (TotalFoldersLeft + TotalFoldersRight), (TotalFilesLeft + TotalFilesRight))
                    };




                    WaitingWindow.WaitingDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);
                }

                #endregion
                
                long folderSizeNew = 0;

                try
                {
                    var dirInfo = new DirectoryInfo(directory);
                    const bool expanded = false;


                    var folderCompareState = DataNode.CompareStates.None;
                    const DataNode.ItemType itemType = DataNode.ItemType.Folder;


                    DataNode m;

                    switch (folderCompareType)
                    {
                        case FolderCompareType.LeftSide:
                            m = new DataNode(dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , string.Empty, string.Empty, -1, dirInfo.LastWriteTime
                                , folderCompareState, level, expanded, itemType);
                            break;
                        case FolderCompareType.RightSide:
                            m = new DataNode(string.Empty, string.Empty, -1, dirInfo.LastWriteTime
                                , dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , folderCompareState, level, expanded, itemType);
                            break;
                        default:
                            m = new DataNode(dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , dirInfo.Name, dirInfo.FullName, -1, dirInfo.LastWriteTime
                                , folderCompareState, level, expanded, itemType);
                            break;
                    }


                    var found = false;
                    if (folderCompareType== FolderCompareType.RightSide)
                    {
                        foreach (var _m in mbr.Children)
                        {
                            if (string.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                                continue;
                            found = true;
                            _m.NameRight = m.NameRight;
                            _m.PathRight = m.PathRight;
                            _m.SizeRight = m.SizeRight;
                            _m.ModifiedRight = m.ModifiedRight;

                            _m.CompareState = _m.CompareState | DataNode.CompareStates.Equal;

                            folderCompareState = _m.CompareState;

                            m = _m;

                            break;
                        }
                    }

                    CollectNodes(m, dirInfo.FullName, ref level, folderCompareType, ref folderSizeNew, ref folderCompareState);

                    switch (folderCompareType)
                    {
                        case FolderCompareType.LeftSide:
                            if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                                m.CompareState = DataNode.CompareStates.OrphansLeftside;
                            break;
                        case FolderCompareType.RightSide:

                            #region  |  set the folder compare state  |

                            m.CompareState = DataNode.CompareStates.None;

                            if (m.Children.Count == 0)
                            {
                                if (m.NameLeft.Trim() != string.Empty && m.NameRight.Trim() != string.Empty)
                                    m.CompareState = DataNode.CompareStates.Equal;
                                else if (m.NameLeft.Trim() == string.Empty && m.NameRight.Trim() != string.Empty)
                                    m.CompareState = DataNode.CompareStates.OrphansRightside;
                                else if (m.NameLeft.Trim() != string.Empty && m.NameRight.Trim() == string.Empty)
                                    m.CompareState = DataNode.CompareStates.OrphansLeftside;
                            }
                            else
                            {
                                foreach (var dn in m.Children)
                                {
                                    if ((dn.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                    {
                                        if ((m.CompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch
                                            && (m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside
                                            && (m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside
                                            && (m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside
                                            && (m.CompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                                        {
                                            m.CompareState = m.CompareState | DataNode.CompareStates.Equal;
                                        }
                                    }
                                    {
                                        if ((dn.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.Mismatch;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.OrphansLeftside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                        if ((dn.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                        {
                                            if ((m.CompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                                                m.CompareState = m.CompareState | DataNode.CompareStates.OrphansRightside;

                                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                                m.CompareState &= ~DataNode.CompareStates.Equal;
                                        }
                                    }
                                }
                            }
                            break;

                            #endregion

                        default:
                            if ((m.CompareState & DataNode.CompareStates.Equal) != DataNode.CompareStates.Equal)
                                m.CompareState = DataNode.CompareStates.Equal;
                            break;
                    }


                    if (folderCompareType == FolderCompareType.LeftSide)
                        m.SizeLeft = folderSizeNew;
                    else if (folderCompareType == FolderCompareType.RightSide)
                        m.SizeRight = folderSizeNew;
                    else
                    {
                        m.SizeLeft = folderSizeNew;
                        m.SizeRight = folderSizeNew;
                    }



                    #region  |  found  |

                    if (!found)
                    {

                        if (folderCompareType == FolderCompareType.RightSide)
                        {
                            var index = 0;
                            foreach (var _m in mbr.Children)
                            {
                                if (_m.Type == 0)
                                {
                                    if (String.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) > 0)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (index == 0)
                                        index = -1;
                                    break;
                                }
                                index++;
                            }
                            if (index > 0)
                            {
                                mbr.AddChild(index, m);
                            }
                            else
                            {

                                mbr.AddChild(m);
                            }
                        }
                        else
                        {
                            mbr.AddChild(m);
                        }
                    }
                    #endregion


                    folderSize += folderSizeNew;

                    if ((m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                    {
                        m.CompareState &= ~DataNode.CompareStates.None;
                    }


                    if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                    {
                        if ((parentFolderCompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch
                            && (parentFolderCompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside
                            && (parentFolderCompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside
                            && (parentFolderCompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside
                            && (parentFolderCompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                        {
                            parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.Equal;
                        }
                    }

                    if ((m.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                    {
                        if ((parentFolderCompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch)
                            parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.Mismatch;

                        if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                            parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                    }
                    if ((m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                    {
                        if ((parentFolderCompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside)
                            parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.MismatchesNewerLeftside;

                        if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                            parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                    }
                    if ((m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                    {
                        if ((parentFolderCompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside)
                            parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.MismatchesNewerRightside;

                        if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                            parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                    }
                    if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                    {
                        if ((parentFolderCompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                            parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.OrphansLeftside;

                        if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                            parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                    }
                    if ((m.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                    {
                        if ((parentFolderCompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                            parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.OrphansRightside;

                        if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                            parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                    }
                }
                catch (Exception ex)
                {
                    level--;
                    throw ex;
                }
            }
            #endregion

            #region  |  files  |

            foreach (var file in files)
            {
                switch (folderCompareType)
                {
                    case FolderCompareType.LeftSide:
                        TotalFilesLeft++;
                        break;
                    case FolderCompareType.RightSide:
                        TotalFilesRight++;
                        break;
                    default:
                        TotalFilesLeft++;
                        TotalFilesRight++;
                        break;
                }


                #region  |  progressObject  |


                if (SetProgressStatus)
                {
                    SetProgressStatus = false;

                    ProgressObject = new ProgressObject
                    {
                        ProgessTitle = "Processing Folders...",
                        CurrentProcessingMessage =
                            string.Format("Searching/Aligning... folders/files (") +
                            (folderCompareType == FolderCompareType.LeftSide ||
                             folderCompareType == FolderCompareType.BothSides
                                ? "left side"
                                : "right side") + ")",
                        TotalProgressValueMessage =
                            string.Format("Searched/Aligned {0} directories and {1} files",
                                (TotalFoldersLeft + TotalFoldersRight), (TotalFilesLeft + TotalFilesRight))
                    };





                    WaitingWindow.WaitingDialogWorker.ReportProgress(ProgressPercentage, ProgressObject);
                }

                #endregion


                var fileInfo = new FileInfo(file);
                const bool expanded = false;

                const DataNode.CompareStates fileCompareState = DataNode.CompareStates.None;
                const DataNode.ItemType itemType = DataNode.ItemType.File;

                DataNode m;

                switch (folderCompareType)
                {
                    case FolderCompareType.LeftSide:
                        m = new DataNode(fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , string.Empty, string.Empty, -1, fileInfo.LastWriteTime
                            , fileCompareState, level, expanded, itemType);

                        #region  |  attributes  (left_side)  |
                        if ((fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "A";
                        }
                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "S";
                        }
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "H";
                        }
                        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "R";
                        }
                        break;

                        #endregion

                    case FolderCompareType.RightSide:
                        m = new DataNode(string.Empty, string.Empty, -1, fileInfo.LastWriteTime
                            , fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , fileCompareState, level, expanded, itemType);

                        #region  |  attributes  (right_side)  |
                        if ((fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "A";
                        }
                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "S";
                        }
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "H";
                        }
                        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "R";
                        }
                        break;

                        #endregion

                    default:
                        m = new DataNode(fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , fileInfo.Name, fileInfo.FullName, fileInfo.Length, fileInfo.LastWriteTime
                            , fileCompareState, level, expanded, itemType);

                        #region  |  attributes  (both_side)  |
                        if ((fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "A";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "A";
                        }
                        if ((fileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "S";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "S";
                        }
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "H";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "H";
                        }
                        if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            m.PropertiesLeft += (m.PropertiesLeft.Trim() != string.Empty ? ";" : string.Empty) + "R";
                            m.PropertiesRight += (m.PropertiesRight.Trim() != string.Empty ? ";" : string.Empty) + "R";
                        }
                        break;

                        #endregion
                }


                if (m.FileType.Trim() == string.Empty)
                {
                    if (_fileTypeNames.ContainsKey(fileInfo.Extension))
                    {
                        m.FileType = _fileTypeNames[fileInfo.Extension];
                    }
                    else
                    {
                        var info = new FileType.SHFILEINFO();
                        var dwFileAttributes = FileType.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
                        var uFlags = (uint)(FileType.SHGFI.SHGFI_TYPENAME | FileType.SHGFI.SHGFI_USEFILEATTRIBUTES);

                        FileType.SHGetFileInfo(file, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);

                        m.FileType = (info.szDisplayName.Trim() != string.Empty ? info.szDisplayName : info.szTypeName);

                        _fileTypeNames.Add(fileInfo.Extension, m.FileType);
                    }
                }
                var found = false;
                if (folderCompareType== FolderCompareType.RightSide)
                {
                    foreach (var _m in mbr.Children)
                    {
                        if (_m.PathRight.Trim() == string.Empty)
                        {
                            if (FileAlignments.Any(fa => string.Compare(_m.PathLeft, fa.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(m.PathRight, fa.PathRight, StringComparison.OrdinalIgnoreCase) == 0))
                            {
                                found = true;
                                _m.NameRight = m.NameRight;
                                _m.PathRight = m.PathRight;
                                _m.SizeRight = m.SizeRight;
                                _m.ModifiedRight = m.ModifiedRight;

                                if ((_m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                                    _m.CompareState &= ~DataNode.CompareStates.None;

                                if ((_m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                    _m.CompareState &= ~DataNode.CompareStates.OrphansLeftside;

                                if ((_m.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                                    _m.CompareState &= ~DataNode.CompareStates.OrphansRightside;



                                if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) == 0
                                    && _m.ModifiedLeft == _m.ModifiedRight)
                                {
                                    _m.CompareState = _m.CompareState | DataNode.CompareStates.Equal;// 1;//0=equal; 1=different; 2=exist only on left; 3=exists only on right; 4=similar
                                }
                                else
                                {
                                    if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0)
                                    {
                                        _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;
                                    }
                                    else if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                                    {
                                        _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;
                                    }

                                    if (_m.SizeLeft != _m.SizeRight)
                                    {
                                        _m.CompareState = _m.CompareState | DataNode.CompareStates.Mismatch;
                                    }
                                    else
                                    {
                                        if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0
                                            || DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                                            _m.CompareState = _m.CompareState | DataNode.CompareStates.Similar;
                                    }
                                }
                                m = _m;
                            }
                        }
                        if (found) continue;
                        if (string.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                            continue;
                        found = true;
                        _m.NameRight = m.NameRight;
                        _m.PathRight = m.PathRight;
                        _m.SizeRight = m.SizeRight;
                        _m.ModifiedRight = m.ModifiedRight;

                        if ((_m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                            _m.CompareState &= ~DataNode.CompareStates.None;

                        if ((_m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                            _m.CompareState &= ~DataNode.CompareStates.OrphansLeftside;

                        if ((_m.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                            _m.CompareState &= ~DataNode.CompareStates.OrphansRightside;



                        if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) == 0
                            && _m.ModifiedLeft == _m.ModifiedRight)
                        {
                            _m.CompareState = _m.CompareState | DataNode.CompareStates.Equal;// 1;//0=equal; 1=different; 2=exist only on left; 3=exists only on right; 4=similar
                        }
                        else
                        {
                            if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0)
                            {
                                _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerLeftside;
                            }
                            else if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                            {
                                _m.CompareState = _m.CompareState | DataNode.CompareStates.MismatchesNewerRightside;
                            }

                            if (_m.SizeLeft != _m.SizeRight)
                            {
                                _m.CompareState = _m.CompareState | DataNode.CompareStates.Mismatch;
                            }
                            else
                            {
                                if (DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) > 0
                                    || DateTime.Compare(_m.ModifiedLeft, _m.ModifiedRight) < 0)
                                    _m.CompareState = _m.CompareState | DataNode.CompareStates.Similar;
                            }
                        }
                        m = _m;
                    }
                }

                #region  |  found  |


                if (!found)
                {                 
                    if ((m.CompareState & DataNode.CompareStates.None) == DataNode.CompareStates.None)
                        m.CompareState &= ~DataNode.CompareStates.None;

                    switch (folderCompareType)
                    {
                        case FolderCompareType.RightSide:
                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                m.CompareState &= ~DataNode.CompareStates.Equal;

                            if ((m.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                                m.CompareState &= ~DataNode.CompareStates.Mismatch;

                            if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                                m.CompareState &= ~DataNode.CompareStates.OrphansLeftside;

                            m.CompareState = m.CompareState | DataNode.CompareStates.OrphansRightside;
                            break;
                        case FolderCompareType.LeftSide:
                            m.CompareState = m.CompareState | DataNode.CompareStates.OrphansLeftside;
                            break;
                        default:
                            if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                                m.CompareState &= ~DataNode.CompareStates.Equal;
                            break;
                    }


                    if (folderCompareType== FolderCompareType.RightSide)
                    {
                        var index = 0;
                        foreach (var _m in mbr.Children)
                        {
                            if (_m.Type == DataNode.ItemType.File) //file
                            {
                                if (string.Compare(_m.NameLeft, m.NameRight, StringComparison.OrdinalIgnoreCase) > 0)
                                {
                                    break;
                                }
                            }
                            index++;
                        }
                        if (index > 0)
                        {
                            mbr.AddChild(index, m);
                        }
                        else
                        {

                            mbr.AddChild(m);
                        }
                    }
                    else
                    {
                        mbr.AddChild(m);
                    }
                }






                if ((m.CompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                {
                    if ((parentFolderCompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch
                        && (parentFolderCompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside
                        && (parentFolderCompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside
                        && (parentFolderCompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside
                        && (parentFolderCompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                    {
                        parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.Equal;
                    }
                }

                if ((m.CompareState & DataNode.CompareStates.Mismatch) == DataNode.CompareStates.Mismatch)
                {
                    if ((parentFolderCompareState & DataNode.CompareStates.Mismatch) != DataNode.CompareStates.Mismatch)
                        parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.Mismatch;

                    if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                        parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                }
                if ((m.CompareState & DataNode.CompareStates.MismatchesNewerLeftside) == DataNode.CompareStates.MismatchesNewerLeftside)
                {
                    if ((parentFolderCompareState & DataNode.CompareStates.MismatchesNewerLeftside) != DataNode.CompareStates.MismatchesNewerLeftside)
                        parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.MismatchesNewerLeftside;

                    if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                        parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                }
                if ((m.CompareState & DataNode.CompareStates.MismatchesNewerRightside) == DataNode.CompareStates.MismatchesNewerRightside)
                {
                    if ((parentFolderCompareState & DataNode.CompareStates.MismatchesNewerRightside) != DataNode.CompareStates.MismatchesNewerRightside)
                        parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.MismatchesNewerRightside;

                    if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                        parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                }
                if ((m.CompareState & DataNode.CompareStates.OrphansLeftside) == DataNode.CompareStates.OrphansLeftside)
                {
                    if ((parentFolderCompareState & DataNode.CompareStates.OrphansLeftside) != DataNode.CompareStates.OrphansLeftside)
                        parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.OrphansLeftside;

                    if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                        parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                }
                if ((m.CompareState & DataNode.CompareStates.OrphansRightside) == DataNode.CompareStates.OrphansRightside)
                {
                    if ((parentFolderCompareState & DataNode.CompareStates.OrphansRightside) != DataNode.CompareStates.OrphansRightside)
                        parentFolderCompareState = parentFolderCompareState | DataNode.CompareStates.OrphansRightside;

                    if ((parentFolderCompareState & DataNode.CompareStates.Equal) == DataNode.CompareStates.Equal)
                        parentFolderCompareState &= ~DataNode.CompareStates.Equal;
                }


                #endregion
            }
            #endregion


            level--;
        }



    }

}