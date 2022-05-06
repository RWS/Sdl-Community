﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sdl.Community.Reports.Viewer.API.Example.Controls
{
	public class SortAwareDataGrid : SelectedItemsAwareDataGrid
	{
		public const string EmptyColumnName = "[none]";

		public SortAwareDataGrid()
		{
			DefaultColumnName = EmptyColumnName;
			SelectionChanged += SortAwareDataGrid_SelectionChanged;
			Loaded += SortAwareDataGrid_Loaded;
		}

		private void SortAwareDataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= SortAwareDataGrid_Loaded;
			//SelectedItem = Items.Count > 0 ? Items[0] : null;
		}

		private void SortAwareDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemsList = SelectedItems;
		}

		public IList SelectedItemsList
		{
			get => (IList)GetValue(SelectedItemsListProperty);
			set => SetValue(SelectedItemsListProperty, value);
		}

		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(SortAwareDataGrid), new PropertyMetadata(null));

		public string DefaultColumnName { get; set; }

		public ListSortDirection DefaultSortDirection { get; set; }

	
		private List<SortDescription> _sortDescriptions;

		protected override void OnSorting(DataGridSortingEventArgs eventArgs)
		{
			base.OnSorting(eventArgs);

			UpdateSorting();
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

			if (_sortDescriptions == null)
			{
				return;
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
		}

		private void UpdateSorting()
		{
			if (ItemsSource == null)
			{
				return;
			}

			var view = CollectionViewSource.GetDefaultView(ItemsSource);

			if (_sortDescriptions == null)
			{
				_sortDescriptions = new List<SortDescription>();
			}
			else
			{
				_sortDescriptions.Clear();
			}

			foreach (var sortDescription in view.SortDescriptions)
			{
				_sortDescriptions.Add(new SortDescription(sortDescription.PropertyName, sortDescription.Direction));
			}
		}

		private void SetDefaultSortDescriptions()
		{
			if (string.IsNullOrEmpty(DefaultColumnName) || DefaultColumnName == EmptyColumnName)
			{
				return;
			}

			_sortDescriptions = new List<SortDescription>
			{
				new SortDescription(DefaultColumnName, DefaultSortDirection)
			};
		}
	}
}
