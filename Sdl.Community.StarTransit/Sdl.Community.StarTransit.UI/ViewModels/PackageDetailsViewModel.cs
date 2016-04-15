using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sdl.Community.StarTransit.UI.Annotations;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class PackageDetailsViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _textLocation;

        public string TextLocation
        {
            get { return _textLocation; }
            set
            {
                if (Equals(value, _textLocation))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "TextLocation")
                {if(string.IsNullOrEmpty(TextLocation))
                        
                    return "Location in required";
                }
                return null;
            }
           
        }

        public string Error { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
