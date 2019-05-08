using System;
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

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class WizardViewModel : INotifyPropertyChanged, IDisposable
	{
		private const int WindowMargin = 40;

		private Size _iconSize = new Size(18, 18);

		private WizardViewModelBase _currentPage;
		private ObservableCollection<WizardViewModelBase> _pages;

		private RelayCommand _moveNextCommand;
		private RelayCommand _moveBackCommand;
		private RelayCommand _finishCommand;
		private RelayCommand _cancelCommand;

		private readonly Window _window;

		public WizardViewModel(Window window, ObservableCollection<WizardViewModelBase> pages)
		{
			_window = window;

			if (_window != null)
			{
				_window.SizeChanged += Window_SizeChanged;
			}
			Pages = pages;

			if (_window != null)
			{
				CalculateProjectNodeSizes(_window.ActualWidth);
			}

			SetCurrentPage(Pages[0]);
		}

		private void SetCurrentPage(WizardViewModelBase currentPage)
		{
			CurrentPage = currentPage;
			_window.Dispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			CalculateProjectNodeSizes(_window.ActualWidth);
		}

		private void CalculateProjectNodeSizes(double actualWidth)
		{
			if (_window == null || actualWidth <= 0 || Pages == null)
			{
				return;
			}

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
				if (page.IsOnFirstPage || page.IsOnLastPage)
				{
					page.LabelLineWidth = fixedLineWidth - fixedLineWidth / 2;
					page.LabelTextWidth = (fixedLineWidth - fixedLineWidth / 2) + _iconSize.Width;
				}
				else
				{
					page.LabelLineWidth = fixedLineWidth / 2;
					page.LabelTextWidth = fixedLineWidth + _iconSize.Width;
				}
			}
		}

		public string WindowTitle { get; private set; }


		public WizardViewModelBase CurrentPage
		{
			get => _currentPage;
			private set
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

				WindowTitle = PluginResources.Plugin_Name + " - " + CurrentPage.DisplayName;

				// move focus to the page in the wizard early
				OnPropertyChanged(nameof(CurrentPage));

				if (_currentPage != null)
				{
					_currentPage.IsVisited = true;
					_currentPage.IsCurrentPage = true;
				}

				OnPropertyChanged(nameof(CurrentPage));
				OnPropertyChanged(nameof(IsOnLastPage));
				OnPropertyChanged(nameof(WindowTitle));
				OnPropertyChanged(nameof(CompletedSteps));
			}
		}

		public ObservableCollection<WizardViewModelBase> Pages
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

		private void AddEventhandlers(ObservableCollection<WizardViewModelBase> pages)
		{
			foreach (var viewModelBase in pages)
			{
				viewModelBase.PropertyChanged += Pages_PropertyChanged;
			}

			pages.CollectionChanged += Pages_CollectionChanged;
		}

		private void RemoveEventhandlers(ObservableCollection<WizardViewModelBase> pages)
		{
			foreach (var viewModelBase in pages)
			{
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
				OnPropertyChanged(nameof(CompletedSteps));
			}
		}

		private int CurrentPageIndex
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

		public bool IsOnLastPage => CurrentPageIndex == Pages.Count - 1;

		public bool IsComplete => IsOnLastPage && CurrentPage.IsComplete;

		public string CompletedSteps =>
			string.Format(PluginResources.HelixWizard_StepsCompleted, Pages.Count(page => page.IsVisited), Pages.Count);

		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelWizard, () => CanCancel));

		private void CancelWizard()
		{
			//HelixModel = null;
			OnRequestClose();
		}

		private bool CanCancel => CurrentPage != null
		                          && (IsOnLastPage && CurrentPage.IsComplete) ? false : true;

		public ICommand FinishCommand => _finishCommand ?? (_finishCommand = new RelayCommand(FinishWizard, () => CanFinish));

		private bool CanFinish
		{
			get
			{
				return CurrentPage != null
					   && ((CurrentPageIndex == 0 && CurrentPage.IsValid)
							|| (IsOnLastPage && CurrentPage.IsComplete)
							|| (CurrentPageIndex > 0 && !IsOnLastPage));
			}
		}

		private void FinishWizard()
		{

			if (!IsOnLastPage)
			{
				var lastPage = Pages[Pages.Count - 1];
				for (var i = CurrentPageIndex; i < lastPage.PageIndex; i++)
				{
					Pages[i].PreviousIsVisited = true;
					Pages[i].IsVisited = true;
					Pages[i].NextIsVisited = true;
				}

				SetCurrentPage(Pages[Pages.Count - 1]);
			}

			else
			{
				//HelixModel = null;
				OnRequestClose();
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

		private void MoveToNextPage()
		{
			if (!CanMoveToNextPage)
			{
				return;
			}

			if (CurrentPageIndex < Pages.Count - 1)
			{
				_currentPage.NextIsVisited = true;
				OnPropertyChanged(nameof(CurrentPage));

				SetCurrentPage(Pages[CurrentPageIndex + 1]);
			}
			else
			{
				OnRequestClose();
			}
		}

		private bool CanMoveToNextPage => CurrentPage != null && CurrentPage.IsValid && !CurrentPage.IsOnLastPage;

		public ICommand MoveBackCommand
		{
			get
			{
				return _moveBackCommand ?? (_moveBackCommand = new RelayCommand(
						   MoveToPreviousPage,
						   () => CanMoveToPreviousPage));
			}
		}

		private bool CanMoveToPreviousPage
		{
			get
			{
				return (CurrentPageIndex > 0 && !CurrentPage.IsOnLastPage)
					   || (CurrentPage.IsOnLastPage && !CurrentPage.IsComplete);
			}
		}

		private void MoveToPreviousPage()
		{
			if (CanMoveToPreviousPage)
			{
				_currentPage.PreviousIsVisited = true;
				OnPropertyChanged(nameof(CurrentPage));

				SetCurrentPage(Pages[CurrentPageIndex - 1]);
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
