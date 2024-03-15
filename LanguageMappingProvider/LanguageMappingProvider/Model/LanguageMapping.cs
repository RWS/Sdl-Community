namespace LanguageMappingProvider
{
	public class LanguageMapping : BaseModel
    {
        private int _index;
        private string _name;
        private string _region;
        private string _tradosCode;
        private string _languageCode;

        public int Index
        {
            get => _index;
            set
            {
                if (_index == value) return;
                _index = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Region
        {
            get => _region;
            set
            {
                if (_region == value) return;
                _region = value;
                OnPropertyChanged();
            }
        }

        public string TradosCode
        {
            get => _tradosCode;
            set
            {
                if (_tradosCode == value) return;
                _tradosCode = value;
                OnPropertyChanged();
            }
        }

        public string LanguageCode
        {
            get => _languageCode;
            set
            {
                if (_languageCode == value) return;
                _languageCode = value;
                OnPropertyChanged();
            }
        }
    }
}