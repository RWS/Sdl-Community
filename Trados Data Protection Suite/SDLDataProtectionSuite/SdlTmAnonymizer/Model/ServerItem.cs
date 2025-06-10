namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
    public class ServerItem : ModelBase
    {
        private bool _isSelected;
        private string _uri;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected)
                    return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string Uri
        {
            get => _uri;
            set
            {
                if (value == _uri)
                    return;
                _uri = value;
                OnPropertyChanged();
            }
        }
    }
}