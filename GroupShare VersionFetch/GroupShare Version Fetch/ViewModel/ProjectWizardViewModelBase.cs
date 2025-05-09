using Sdl.Community.GSVersionFetch.Interface;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public abstract class ProjectWizardViewModelBase : IProgressHeaderItem, INotifyPropertyChanged
    {
        private bool _isComplete;
        private bool _isCurrentPage;
        private bool _isUpdated;
        private bool _isVisited;
        private double _itemLineWidth;
        private double _itemTextWidth;
        private bool _nextIsVisited;
        private bool _previousIsVisited;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CurrentPageChanged { get; set; }

        public abstract string DisplayName { get; set; }

        public bool IsComplete
        {
            get => _isComplete;
            set
            {
                if (value == _isComplete)
                    return;

                _isComplete = value;
                OnPropertyChanged(nameof(IsComplete));
            }
        }

        public bool IsCurrentPage
        {
            get => _isCurrentPage;
            set
            {
                if (value == _isCurrentPage)
                {
                    return;
                }

                _isCurrentPage = value;
                OnPropertyChanged(nameof(IsCurrentPage));
                OnPropertyChanged(nameof(CurrentPageChanged));
            }
        }

        public bool IsFirstPage => PageIndex == 1;

        public bool IsLastPage => PageIndex == TotalPages;

        public bool IsUpdated
        {
            get => _isUpdated;
            set
            {
                if (value == _isUpdated)
                {
                    return;
                }

                _isUpdated = value;
                OnPropertyChanged(nameof(IsUpdated));
            }
        }

        public abstract bool IsValid { get; set; }

        public bool IsVisited
        {
            get => _isVisited;
            set
            {
                if (value == _isVisited)
                {
                    return;
                }

                _isVisited = value;
                OnPropertyChanged(nameof(IsVisited));
            }
        }

        public double ItemLineWidth
        {
            get => _itemLineWidth;
            set
            {
                if (_itemLineWidth == value)
                {
                    return;
                }

                _itemLineWidth = value;
                OnPropertyChanged(nameof(ItemLineWidth));
            }
        }

        public double ItemTextWidth
        {
            get => _itemTextWidth;
            set
            {
                if (_itemTextWidth == value)
                {
                    return;
                }

                _itemTextWidth = value;
                OnPropertyChanged(nameof(ItemTextWidth));
            }
        }

        public bool NextIsVisited
        {
            get => _nextIsVisited;
            set
            {
                if (value == _nextIsVisited)
                {
                    return;
                }

                _nextIsVisited = value;
                OnPropertyChanged(nameof(NextIsVisited));
            }
        }

        public int PageIndex { get; set; }

        public bool PreviousIsVisited
        {
            get => _previousIsVisited;
            set
            {
                if (value == _previousIsVisited)
                {
                    return;
                }

                _previousIsVisited = value;
                OnPropertyChanged(nameof(PreviousIsVisited));
            }
        }

        public int TotalPages { get; set; }
        public object View { get; }

        public abstract bool OnChangePage(int position, out string message);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}