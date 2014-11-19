using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sdl.Community.TMOptimizer
{
    public class TranslationMemoryReference : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private FileBasedTranslationMemory _tm;

        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (_filePath != value)
                {
                    _tm = null;
                }

                _filePath = value;

                OnPropertyChanged("FilePath");
            }
        }

        public FileBasedTranslationMemory TranslationMemory
        {
            get
            {
                if (_tm == null)
                {
                    _tm = new FileBasedTranslationMemory(FilePath);
                }
                return _tm;
            }
            set
            {
                _tm = value;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
