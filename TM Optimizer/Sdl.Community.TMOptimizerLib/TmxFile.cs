using System.ComponentModel;

namespace Sdl.Community.TMOptimizerLib
{
	public class TmxFile : INotifyPropertyChanged
    {
        public TmxFile(string filePath)
        {
            FilePath = filePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string FilePath
        {
            get;
            private set;
        }

        private bool _isDetecting = false;
        public bool IsDetecting
        {
            get
            {
                return _isDetecting;
            }
            private set
            {
                _isDetecting = value;
                OnPropertyChanged("IsDetecting");
                Status = IsDetecting ? "Analyzing..." : (DetectInfo != null ? "Ready" : "");
            }
        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            private set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private DetectInfo _detectInfo;
        public DetectInfo DetectInfo
        {
            get
            {
                return _detectInfo;
            }
            set
            {
                _detectInfo = value;
                OnPropertyChanged("DetectInfo");
            }
        }
        
        public DetectInfo GetDetectInfo()
        {
            if (DetectInfo == null)
            {
                Detect();
            }

            return DetectInfo;
        }

        public void Detect()
        {
            IsDetecting = true;
            try
            {
                Detector detector = new Detector(FilePath);
                DetectInfo = detector.Detect();
            }
            finally
            {
                IsDetecting = false;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}