using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public class SelectAwareDataGrid : DataGrid, IDisposable
	{
		public SelectAwareDataGrid()
		{
			SelectionChanged += SelectAwareDataGrid_SelectionChanged;
			Loaded += SelectAwareDataGrid_Loaded;
		}

		public IList SelectedItemsList
		{
			get => (IList)GetValue(SelectedItemsListProperty);
			set => SetValue(SelectedItemsListProperty, value);
		}

		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(SelectAwareDataGrid), new PropertyMetadata(null));

		private void SelectAwareDataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= SelectAwareDataGrid_Loaded;
			SelectedItem = Items.Count > 0 ? Items[0] : null;
		}

		private void SelectAwareDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemsList = SelectedItems;
		}

		public void Dispose()
		{
			SelectionChanged -= SelectAwareDataGrid_SelectionChanged;
			Loaded -= SelectAwareDataGrid_Loaded;
		}
	}
}
