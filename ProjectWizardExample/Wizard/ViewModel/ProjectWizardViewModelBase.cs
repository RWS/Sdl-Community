using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectWizardExample.Wizard.API;

namespace ProjectWizardExample.Wizard.ViewModel
{
	public abstract class ProjectWizardViewModelBase : IProgressHeaderItem, INotifyPropertyChanged
	{
        private bool _isVisited;
	    private bool _isComplete;	
	    private bool _isUpdated;
		private bool _nextIsVisited;
        private bool _previousIsVisited;        
        private bool _isCurrentPage;
        private double _itemLineWidth;
        private double _itemTextWidth;

		protected ProjectWizardViewModelBase(object view)
        {            
            View = view;         
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

		public int TotalPages { get; set; }

		public int PageIndex { get; set; }

		public bool IsFirstPage => PageIndex == 1;

		public bool IsLastPage => PageIndex == TotalPages;

		public object View { get; }

		public bool CurrentPageChanged { get; set; }

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

		public abstract bool OnChangePage(out string message);

		public abstract string DisplayName { get; set; }		

		public abstract bool IsValid { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
