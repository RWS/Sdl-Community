using System.ComponentModel;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMOptimizer.Control
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
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}