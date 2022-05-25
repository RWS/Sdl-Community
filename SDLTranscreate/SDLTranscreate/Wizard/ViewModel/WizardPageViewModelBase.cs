using System;
using System.Windows;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel
{
	public abstract class WizardPageViewModelBase : BaseModel
	{
		private bool _isVisited;
		private bool _isComplete;
		private bool _isUpdated;
		private bool _isProcessing;
		private bool _nextIsVisited;
		private bool _previousIsVisited;
		private bool _isCurrentPage;
		private double _labelLineWidth;
		private double _labelTextWidth;

		protected WizardPageViewModelBase(Window owner, object view, TaskContext taskContext)
		{			
			Owner = owner;
			View = view;
			TaskContext = taskContext;
			IsProcessing = false;
		}

		public event EventHandler LoadPage;

		public event EventHandler LeavePage;

		public TaskContext TaskContext { get; set; }

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

		public Window Owner { get; }

		public object View { get; }
		
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

				if (_isCurrentPage)
				{
					LoadPage?.Invoke(this, EventArgs.Empty);
				}
				else
				{
					LeavePage?.Invoke(this, EventArgs.Empty);
				}
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
				{
					return;
				}

				_isComplete = value;
				OnPropertyChanged(nameof(IsComplete));
			}
		}

		public bool IsProcessing
		{
			get => _isProcessing;
			set
			{
				if (_isProcessing == value)
				{
					return;
				}

				_isProcessing = value;
				OnPropertyChanged(nameof(IsProcessing));
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
	}
}
