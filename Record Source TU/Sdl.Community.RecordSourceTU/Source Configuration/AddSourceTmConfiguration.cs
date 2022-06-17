using System;

namespace Sdl.Community.RecordSourceTU
{
    public class AddSourceTmConfiguration
    {
        private bool _hasChanges;
        private Uri _providerUri;
        private string _fileNameField;
        private string _fullPathField;
        private string _projectNameField;
        private bool _storeFilename;
        private bool _storeFullPath;
        private bool _storeProjectName;

        public AddSourceTmConfiguration()
        {
            _hasChanges = false;
        }

        public Uri ProviderUri
        {
            get { return _providerUri; }
            set
            {
                if (_providerUri == value) return;
                _providerUri = value;
                _hasChanges = true;
            }
        }

        public string FileNameField
        {
            get { return _fileNameField; }
            set
            {
                if (_fileNameField == value) return;
                _fileNameField = value;
                _hasChanges = true;
            }
        }

        public string FullPathField
        {
            get { return _fullPathField; }
            set
            {
                if (_fullPathField == value) return;
                _fullPathField = value;
                _hasChanges = true;
            }
        }

        public string ProjectNameField
        {
            get { return _projectNameField; }
            set
            {
                if (_projectNameField == value) return;
                _projectNameField = value;
                _hasChanges = true;
            }
        }

        public bool StoreFilename
        {
            get { return _storeFilename; }
            set
            {
                if (_storeFilename == value) return;
                _storeFilename = value;
                _hasChanges = true;
            }
        }

        public bool StoreFullPath
        {
            get { return _storeFullPath; }
            set
            {
                if (_storeFullPath == value) return;
                _storeFullPath = value;
                _hasChanges = true;
            }
        }

        public bool StoreProjectName
        {
            get { return _storeProjectName; }
            set
            {
                if (_storeProjectName == value) return;
                _storeProjectName = value;
                _hasChanges = true;
            }
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void SaveChanges()
        {
            _hasChanges = false;
        }
    }
}
