using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Interface;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class ProjectWizardViewModel : INotifyPropertyChanged, IProgressHeader, IDisposable
	{
		private readonly Window _window;
		private const int WindowMargin = 40;
		private Size _iconSize = new Size(18, 18);
		private double _actualWidth;
		private IProgressHeaderItem _currentPage;
		private ObservableCollection<IProgressHeaderItem> _pages;
		private RelayCommand _moveNextCommand;
		private RelayCommand _moveBackCommand;
		private RelayCommand _finishCommand;
		private RelayCommand _cancelCommand;
		private ICommand _selectedPageCommand;

		public ProjectWizardViewModel(Window window, ObservableCollection<IProgressHeaderItem> pages)
		{
			_window = window;

			if (_window != null)
			{
				_window.SizeChanged += Window_SizeChanged;
			}

			Pages = pages;

			if (_window != null)
			{
				CalculateProgressHeaderItemsSize(_window.ActualWidth);
			}

			SetCurrentPage(0);
		}

		public event EventHandler<SelectedPageEventArgs> SelectedPageChanged;

		public bool CanMoveToPage(int position, out string message)
		{
			message = string.Empty;

			var currentPagePosition = CurrentPagePosition;
			var currentPage = CurrentPage;

			if (currentPagePosition == position)
			{
				return false;
			}

			if (!currentPage.OnChangePage(position, out var outMessage))
			{
				message = outMessage;
				return false;
			}

			if (position > currentPagePosition && !currentPage.IsLastPage)
			{
				var notValid = new List<IProgressHeaderItem>();
				for (var i = currentPagePosition + 1; i < position; i++)
				{
					var page = Pages[i];
					if (!page.IsValid)
					{
						notValid.Add(page);
					}
				}

				if (notValid.Count > 0)
				{
					var pageNames = string.Empty;
					foreach (var page in notValid)
					{
						pageNames += (string.IsNullOrEmpty(pageNames) ? string.Empty : Environment.NewLine) + " * " + page.DisplayName;
					}
					message = PluginResources.UnableToNavigateToSelectedPage + Environment.NewLine + Environment.NewLine +
					          PluginResources.DataOnTheFollowingPagesAreNotValid + Environment.NewLine +
							  $"{pageNames}";
					return false;
				}
			}

			return true;
		}

		public ICommand SelectedPageCommand => _selectedPageCommand ?? (_selectedPageCommand = new MouseDownCommand(this));

		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelWizard, () => CanCancel));

		public ICommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(FinishWizard, () => CanFinish));

		public void UpdateCurrentPageState(bool isValid, bool isComplete)
		{
			foreach (var page in Pages)
			{
				page.IsCurrentPage = false;
				page.IsComplete = IsComplete;
			}

			CurrentPage.IsCurrentPage = true;
			CurrentPage.IsVisited = true;
			CurrentPage.IsValid = isValid;
			CurrentPage.IsComplete = isComplete;


			OnPropertyChanged(nameof(CompletedProgressStepsMessage));
			CalculateProgressHeaderItemsSize(_actualWidth);
		}

		public void MoveToSelectedPage(IProgressHeaderItem item)
		{
			SelectedPageChanged?.Invoke(this, new SelectedPageEventArgs
			{
				ProgressHeaderItem = item,
				PagePosition = GetCurrentPagePosition(item)
			});
		}

		public void SetCurrentPage(int index)
		{
			// check if null or empty collection
			if (Pages == null || Pages.Count == 0)
			{
				return;
			}

			CurrentPage = Pages[index];
			UpdateVisitedPages();

			_window.Dispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		public void CalculateProgressHeaderItemsSize(double actualWidth)
		{
			if (_window == null || actualWidth <= 0 || Pages == null)
			{
				return;
			}

			_actualWidth = actualWidth;

			// (ICON)[LINE]
			// [TEXT------]

			// [LINE](ICON)[LINE]
			// [------TEXT------]

			// [LINE](ICON)
			// [------TEXT]

			var totalItems = Pages.Count;

			var controlActualWidth = actualWidth - WindowMargin;
			var totalIconWidth = _iconSize.Width * totalItems;

			var totalLineWidth = controlActualWidth - totalIconWidth;
			var fixedLineWidth = totalLineWidth / (Pages.Count - 1);

			foreach (var page in Pages)
			{
				if (page.IsFirstPage || page.IsLastPage)
				{
					page.ItemLineWidth = fixedLineWidth - fixedLineWidth / 2;
					page.ItemTextWidth = (fixedLineWidth - fixedLineWidth / 2) + _iconSize.Width;
				}
				else
				{
					page.ItemLineWidth = fixedLineWidth / 2;
					page.ItemTextWidth = fixedLineWidth + _iconSize.Width;
				}
			}
		}

		public string WindowTitle { get; private set; }

		public IProgressHeaderItem CurrentPage
		{
			get => _currentPage;
			set
			{
				if (value == _currentPage)
				{
					return;
				}

				if (_currentPage != null)
				{
					_currentPage.IsCurrentPage = false;
				}

				_currentPage = value;

				WindowTitle = PluginResources.ProjectWizard_Create_a_New_Project + " - " + CurrentPage.DisplayName;

				// move focus to the page in the wizard early
				OnPropertyChanged(nameof(CurrentPage));

				if (_currentPage != null)
				{
					_currentPage.IsVisited = true;
					_currentPage.IsCurrentPage = true;
				}

				OnPropertyChanged(nameof(CurrentPage));
				OnPropertyChanged(nameof(IsLastPage));
				OnPropertyChanged(nameof(WindowTitle));
				OnPropertyChanged(nameof(CompletedProgressStepsMessage));
			}
		}

		public ObservableCollection<IProgressHeaderItem> Pages
		{
			get => _pages;
			set
			{
				if (_pages == value)
				{
					return;
				}

				if (_pages != null)
				{
					RemoveEventhandlers(_pages);
				}

				_pages = value;

				AddEventhandlers(_pages);

				OnPropertyChanged(nameof(Pages));
			}
		}

		public int CurrentPagePosition
		{
			get
			{
				if (CurrentPage == null)
				{
					Debug.Fail("The current page is null!");
				}

				return Pages.IndexOf(CurrentPage);
			}
		}

		public bool IsLastPage => CurrentPagePosition == Pages.Count - 1;

		public bool IsComplete => IsLastPage && CurrentPage.IsComplete;

		public string CompletedProgressStepsMessage =>
			string.Format(PluginResources.ProjectWizard_StepsCompleted, Pages.Count(page => page.IsVisited), Pages.Count);

		public void MoveToNextPage()
		{
			if (!CanMoveToNextPage)
			{
				return;
			}

			if (CurrentPagePosition < Pages.Count - 1)
			{
				_currentPage.NextIsVisited = true;
				OnPropertyChanged(nameof(CurrentPage));

				SetCurrentPage(CurrentPagePosition + 1);
			}
			else
			{
				OnRequestClose();
			}
		}

		public ICommand MoveBackCommand
		{
			get
			{
				return _moveBackCommand ?? (_moveBackCommand = new RelayCommand(
						   MoveToPreviousPage,
						   () => CanMoveToPreviousPage));
			}
		}

		public ICommand MoveNextCommand
		{
			get
			{
				return _moveNextCommand ??
					   (_moveNextCommand = new RelayCommand(
						   MoveToNextPage,
						   () => CanMoveToNextPage));
			}
		}

		public void MoveToPreviousPage()
		{
			if (CanMoveToPreviousPage)
			{
				_currentPage.PreviousIsVisited = true;
				OnPropertyChanged(nameof(CurrentPage));

				SetCurrentPage(CurrentPagePosition - 1);
			}
		}

		private void UpdateVisitedPages()
		{
			if (CurrentPage == null)
			{
				return;
			}

			foreach (var item in Pages)
			{
				item.IsVisited = true;
				item.PreviousIsVisited = true;

				// assigned true only when the next page is visited.
				if (item.PageIndex <= CurrentPagePosition)
				{
					item.NextIsVisited = true;
				}

				// stop assigning the visited properties when the page index is equal or greater than 
				// the last PageIndex visited that is nearest to completion of the wizard
				if (item.PageIndex >= CurrentPagePosition)
				{
					break;
				}
			}
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			CalculateProgressHeaderItemsSize(_window.ActualWidth);
		}

		private void AddEventhandlers(ObservableCollection<IProgressHeaderItem> pages)
		{
			foreach (var page in pages)
			{
				var viewModelBase = (ProjectWizardViewModelBase)page;
				viewModelBase.PropertyChanged += Pages_PropertyChanged;
			}

			pages.CollectionChanged += Pages_CollectionChanged;
		}

		private void RemoveEventhandlers(ObservableCollection<IProgressHeaderItem> pages)
		{
			foreach (var page in pages)
			{
				var viewModelBase = (ProjectWizardViewModelBase)page;
				viewModelBase.PropertyChanged -= Pages_PropertyChanged;
			}

			pages.CollectionChanged -= Pages_CollectionChanged;
		}

		private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (ProjectWizardViewModelBase viewModelBase in e.OldItems)
			{
				viewModelBase.PropertyChanged -= Pages_PropertyChanged;
			}

			foreach (ProjectWizardViewModelBase viewModelBase in e.NewItems)
			{
				viewModelBase.PropertyChanged += Pages_PropertyChanged;
			}
		}

		private void Pages_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsValid")
			{
				OnPropertyChanged(nameof(CompletedProgressStepsMessage));
			}
		}

		private int GetCurrentPagePosition(IProgressHeaderItem item)
		{
			var index = 0;
			foreach (var progressHeaderItem in Pages)
			{
				if (progressHeaderItem.Equals(item))
				{
					return index;
				}

				index++;
			}
			return -1;
		}

		private void CancelWizard()
		{
			OnRequestClose();
		}

		private bool CanCancel
		{
			get
			{
				return CurrentPage == null || !IsLastPage || !CurrentPage.IsComplete;
			}
		}

		private bool CanFinish
		{
			get
			{
				return CurrentPage != null
					   && ((CurrentPagePosition == 0 && CurrentPage.IsValid)
						   || (IsLastPage && CurrentPage.IsComplete)
						   || (CurrentPagePosition > 0 && !IsLastPage));
			}
		}

		private void FinishWizard()
		{

			if (!IsLastPage)
			{
				var lastPage = Pages[Pages.Count - 1];
				for (var i = CurrentPagePosition; i < lastPage.PageIndex; i++)
				{
					Pages[i].PreviousIsVisited = true;
					Pages[i].IsVisited = true;
					Pages[i].NextIsVisited = true;
				}

				SetCurrentPage(Pages.Count - 1);
			}

			else
			{
				OnRequestClose();
			}
		}

		private bool CanMoveToNextPage => CurrentPage != null && CurrentPage.IsValid && !CurrentPage.IsLastPage;

		private bool CanMoveToPreviousPage
		{
			get
			{
				return (CurrentPagePosition > 0 && !CurrentPage.IsLastPage)
					   || (CurrentPage.IsLastPage && !CurrentPage.IsComplete);
			}
		}

		public event EventHandler RequestClose;

		private void OnRequestClose()
		{
			RequestClose?.Invoke(this, EventArgs.Empty);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Dispose()
		{
			if (_window != null)
			{
				_window.SizeChanged -= Window_SizeChanged;
			}

			if (Pages != null)
			{
				RemoveEventhandlers(Pages);
			}
		}
	}
}
