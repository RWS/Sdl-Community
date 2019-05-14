using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public abstract class ProjectWizardViewModelBase : INotifyPropertyChanged
	{
		private bool _isVisited;
		private bool _isComplete;
		private bool _isUpdated;
		private bool _nextIsVisited;
		private bool _previousIsVisited;
		private bool _isCurrentPage;
		private double _labelLineWidth;
		private double _labelTextWidth;

		protected ProjectWizardViewModelBase(object view)
		{
			View = view;
		}
		public double LabelLineWidth
		{
			get => _labelLineWidth;
			set
			{
				if (_labelLineWidth == value)
				{
					return;
				}

				_labelLineWidth = value;
				OnPropertyChanged(nameof(LabelLineWidth));
			}
		}

		public double LabelTextWidth
		{
			get => _labelTextWidth;
			set
			{
				if (_labelTextWidth == value)
				{
					return;
				}

				_labelTextWidth = value;
				OnPropertyChanged(nameof(LabelTextWidth));
			}
		}

		public int TotalPages { get; set; }

		public int PageIndex { get; set; }

		public bool IsOnFirstPage => PageIndex == 1;

		public bool IsOnLastPage => PageIndex == TotalPages;

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

		public abstract string DisplayName { get; }

		public abstract bool IsValid { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
