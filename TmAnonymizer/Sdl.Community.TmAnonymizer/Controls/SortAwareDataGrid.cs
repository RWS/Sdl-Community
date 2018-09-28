using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public class SortAwareDataGrid : DataGrid
	{
		public SortAwareDataGrid()
		{
			SelectionChanged += CustomDataGrid_SelectionChanged;
		}

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

			_sortDescriptions.Clear();
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

		private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemsList = SelectedItems;
		}

		public IList SelectedItemsList
		{
			get { return (IList)GetValue(SelectedItemsListProperty); }
			set { SetValue(SelectedItemsListProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(SortAwareDataGrid), new PropertyMetadata(null));
	}
}
