using System;
using System.Collections.Generic;



namespace PostEdit.Compare.Model
{


 
    public class DataNode
    {
        [Flags]
        public enum CompareStates
        {
            None = 0,
            Equal = 1,
            Mismatch = 2,
            MismatchesNewerLeftside = 4,
            MismatchesNewerRightside = 8,
            OrphansLeftside = 16,
            OrphansRightside = 32,
            Similar = 64
            
        }

        public enum ItemType
        {
            Folder = 0,
            File = 1
        }

        public enum Selection
        {
            Right,
            Left,
            Middle,
            None
        }

        internal List<DataNode> MChildren;

        private string _mNameLeft;
        private string _mPathLeft;
        private long _mSizeLeft;
        private string _mPropertiesLeft;
        private DateTime _mModifiedLeft;

        private string _mNameRight;
        private string _mPathRight;
        private long _mSizeRight;
        private string _mPropertiesRight;
        private DateTime _mModifiedRight;
     
        private bool _mExpanded;
        private int _mLevel;
        private ItemType _mType;
        private CompareStates _mCompareState;
        private Selection _mSelection;
        private string _mFileType;
        

        public DataNode(string nameLeft, string pathLeft, long sizeLeft, DateTime modifiedLeft,
            string nameRight, string pathRight, long sizeRight, DateTime modifiedRight
            , CompareStates compareState, int level, bool expanded, ItemType type)
        {
            MChildren = new List<DataNode>(16);
            _mNameLeft = nameLeft;
            _mPathLeft = pathLeft;
            _mSizeLeft = sizeLeft;
            _mModifiedLeft = modifiedLeft;

            _mNameRight = nameRight;
            _mPathRight = pathRight;
            _mSizeRight = sizeRight;
            _mModifiedRight = modifiedRight;

            _mCompareState = compareState;
            _mLevel = level;
            _mType = type;
            _mExpanded = expanded;

            _mFileType = string.Empty;
            _mSelection = Selection.None;

            _mPropertiesLeft = string.Empty;
            _mPropertiesRight = string.Empty;

        }


        public void AddChild(DataNode child)
        {
            MChildren.Add(child);            
        }
        public void AddChild(int index, DataNode child)
        {
            MChildren.Insert(index, child);
        }

        public string FileType
        {
            get { return _mFileType; }
            set { _mFileType = value; }
        }


        public string NameLeft
        {
            get { return _mNameLeft; }
            set { _mNameLeft = value; }
        }
        public string PathLeft
        {
            get { return _mPathLeft; }
            set { _mPathLeft = value; }
        }
        public long SizeLeft
        {
            get { return _mSizeLeft; }
            set { _mSizeLeft = value; }
        }
        public string PropertiesLeft
        {
            get { return _mPropertiesLeft; }
            set { _mPropertiesLeft = value; }
        }
        public DateTime ModifiedLeft
        {
            get { return _mModifiedLeft; }
            set { _mModifiedLeft = value; }
        }


        public string NameRight
        {
            get { return _mNameRight; }
            set { _mNameRight = value; }
        }
        public string PathRight
        {
            get { return _mPathRight; }
            set { _mPathRight = value; }
        }
        public long SizeRight
        {
            get { return _mSizeRight; }
            set { _mSizeRight = value; }
        }
        public string PropertiesRight
        {
            get { return _mPropertiesRight; }
            set { _mPropertiesRight = value; }
        }
        public DateTime ModifiedRight
        {
            get { return _mModifiedRight; }
            set { _mModifiedRight = value; }
        }

        public int CountChildren
        {
            get { return MChildren.Count; }
        }


        public List<DataNode> Children
        {
            get { return MChildren; }
        }

        public bool Expanded
        {
            get { return _mExpanded; }
            set { _mExpanded = value; }
        }

        public int Level
        {
            get { return _mLevel; }
            set { _mLevel = value; }
        }

        public ItemType Type
        {
            get { return _mType; }
            set { _mType = value; }
        }
        public Selection SelectionType
        {
            get { return _mSelection; }
            set { _mSelection = value; }
        }
        

        public CompareStates CompareState
        {
            get { return _mCompareState; }
            set { _mCompareState = value; }
        }

    
    }
}