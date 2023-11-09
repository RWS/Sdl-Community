using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace InterpretBank.Model
{
    public class TermModel : INotifyPropertyChanged
    {
        private string _firstComment;
        private Image _languageFlag;
        private string _secondComment;
        private string _term;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstComment
        {
            get => _firstComment;
            set => SetField(ref _firstComment, value);
        }

        public Image LanguageFlag
        {
            get => _languageFlag;
            set => SetField(ref _languageFlag, value);
        }

        public string LanguageName { get; set; }

        public bool Modified { get; set; }

        public string SecondComment
        {
            get => _secondComment;
            set => SetField(ref _secondComment, value);
        }

        public string Term
        {
            get => _term;
            set => SetField(ref _term, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TermModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_firstComment != null ? _firstComment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_secondComment != null ? _secondComment.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_term != null ? _term.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LanguageName != null ? LanguageName.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(TermModel other)
        {
            return _firstComment == other._firstComment && _secondComment == other._secondComment && _term == other._term && LanguageName == other.LanguageName;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            Modified = true;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}