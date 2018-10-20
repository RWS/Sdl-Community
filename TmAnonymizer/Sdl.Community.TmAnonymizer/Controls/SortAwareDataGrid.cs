using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public class SortAwareDataGrid : DataGrid, IDisposable
	{
		public SortAwareDataGrid()
		{
			SelectionChanged += CustomDataGrid_SelectionChanged;
			Loaded += SortAwareDataGrid_Loaded;
		}

		public string DefaultColumnName { get; set; }

		public ListSortDirection DefaultSortDirection { get; set; }

		public IList SelectedItemsList
		{
			get => (IList)GetValue(SelectedItemsListProperty);
			set => SetValue(SelectedItemsListProperty, value);
		}

		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(SortAwareDataGrid), new PropertyMetadata(null));

		private List<SortDescription> _sortDescriptions;

		protected override void OnSorting(DataGridSortingEventArgs eventArgs)
		{
			base.OnSorting(eventArgs);

			UpdateSorting();
		}

		private void SortAwareDataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			SelectedItem = Items.Count > 0 ? Items[0] : null;
		}

		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			base.OnItemsSourceChanged(oldValue, newValue);

			if (newValue == null)
			{
				return;
			}

			var view = CollectionViewSource.GetDefaultView(newValue);
			view.SortDescriptions.Clear();

			if (_sortDescriptions == null || _sortDescriptions.Count == 0)
			{
				SetDefaultSortDescriptions();
			}

			foreach (var sortDescription in _sortDescriptions)
			{
				view.SortDescriptions.Add(sortDescription);

				var column = Columns.FirstOrDefault(c => c.SortMemberPath == sortDescription.PropertyName);
				if (column != null)
				{
					column.SortDirection = sortDescription.Direction;
				}
			}

			AddSegmentNumberSorter(view);
		}

		private void UpdateSorting()
		{
			if (ItemsSource == null)
			{
				return;
			}

			_sortDescriptions.Clear();

			var view = CollectionViewSource.GetDefaultView(ItemsSource);
			
			AddSegmentNumberSorter(view);

			foreach (var sortDescription in view.SortDescriptions)
			{
				_sortDescriptions.Add(new SortDescription(sortDescription.PropertyName, sortDescription.Direction));
			}
		}
		
		private void SetDefaultSortDescriptions()
		{
			if (string.IsNullOrEmpty(DefaultColumnName))
			{
				return;
			}

			_sortDescriptions = new List<SortDescription>
			{
				new SortDescription(DefaultColumnName, DefaultSortDirection)
			};
		}

		private static void AddSegmentNumberSorter(ICollectionView view)
		{
			if (view.SortDescriptions.Count > 0 &&
			    view.SortDescriptions[0].PropertyName == "SegmentNumber" &&
			    view.CurrentItem is ContentSearchResult)
			{
				var collection = (ListCollectionView)view;
				collection.CustomSort = new SegmentNumberSorter(view.SortDescriptions[0]);
			}
		}


		private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemsList = SelectedItems;
		}

		public void Dispose()
		{
			SelectionChanged -= CustomDataGrid_SelectionChanged;
			Loaded -= SortAwareDataGrid_Loaded;
		}
	}

	public class SegmentNumberSorter : IComparer
	{
		private readonly SortDescription _sortDescription;

		public SegmentNumberSorter(SortDescription sortDescription)
		{
			_sortDescription = sortDescription;
		}

		public int Compare(object x, object y)
		{
			if (!(x is ContentSearchResult searchResultX) || !(y is ContentSearchResult searchResultY))
			{
				return 0;
			}

			try
			{
				var intx = searchResultX.TranslationUnit.ResourceId.Id;
				var inty = searchResultY.TranslationUnit.ResourceId.Id;

				return _sortDescription.Direction == ListSortDirection.Ascending 
					? intx.CompareTo(inty) 
					: inty.CompareTo(intx);
			}
			catch
			{
				// don't raise exception here
			}

			return _sortDescription.Direction == ListSortDirection.Ascending
				? string.Compare(searchResultX.TranslationUnit.ResourceId.Id.ToString(), searchResultY.TranslationUnit.ResourceId.Id.ToString(), StringComparison.Ordinal)
				: string.Compare(searchResultY.TranslationUnit.ResourceId.Id.ToString(), searchResultX.TranslationUnit.ResourceId.Id.ToString(), StringComparison.Ordinal);			
		}
	}
}
