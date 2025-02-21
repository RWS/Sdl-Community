﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Helpers;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class WizardViewModel : INotifyPropertyChanged, IProgressHeader, IDisposable
	{
		private readonly Window _window;
		private const int WindowMargin = 40;
		private Size _iconSize = new Size(18, 18);
		private double _actualWidth;
		private IProgressHeaderItem _currentPage;
		private ObservableCollection<IProgressHeaderItem> _pages;
		private readonly IEventAggregatorService _eventAggregatorService;
		private RelayCommand _moveNextCommand;
		private RelayCommand _moveBackCommand;
		private RelayCommand _finishCommand;
		private RelayCommand _cancelCommand;
		private ICommand _selectedPageCommand;

		public WizardViewModel(Window window, ObservableCollection<IProgressHeaderItem> pages, IEventAggregatorService eventAggregatorService)
		{
			_window = window;

			if (_window != null)
			{
				_window.SizeChanged += Window_SizeChanged;
			}

			Pages = pages;
			_eventAggregatorService = eventAggregatorService;

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
			if (Pages is null || Pages.Count == 0)
			{
				return;
			}

			CurrentPage = Pages[index];
			UpdateVisitedPages();

			_window.Dispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		public void CalculateProgressHeaderItemsSize(double actualWidth)
		{
			if (_window is null || actualWidth <= 0 || Pages is null)
			{
				return;
			}

			_actualWidth = actualWidth;
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

				WindowTitle = PluginResources.Wizard_Name + " - " + CurrentPage.DisplayName;

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
			string.Format(PluginResources.Wizard_StepsCompleted, Pages.Count(page => page.IsVisited), Pages.Count);

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
				var viewModelBase = (WizardViewModelBase)page;
				viewModelBase.PropertyChanged += Pages_PropertyChanged;
			}

			pages.CollectionChanged += Pages_CollectionChanged;
		}

		private void RemoveEventhandlers(ObservableCollection<IProgressHeaderItem> pages)
		{
			foreach (var page in pages)
			{
				var viewModelBase = (WizardViewModelBase)page;
				viewModelBase.PropertyChanged -= Pages_PropertyChanged;
			}

			pages.CollectionChanged -= Pages_CollectionChanged;
		}

		private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (WizardViewModelBase viewModelBase in e.OldItems)
			{
				viewModelBase.PropertyChanged -= Pages_PropertyChanged;
			}

			foreach (WizardViewModelBase viewModelBase in e.NewItems)
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

		private bool CanCancel => CurrentPage != null && CurrentPage.CanCancel;

		private bool CanFinish => CurrentPage != null && CurrentPage.CanFinish;

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
				_eventAggregatorService?.PublishEvent(new CreateStudioProject());
			}
		}

		private bool CanMoveToNextPage => CurrentPage != null && CurrentPage.IsValid && !CurrentPage.IsLastPage;

		private bool CanMoveToPreviousPage => (CurrentPagePosition > 0) && !CurrentPage.IsComplete;

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
