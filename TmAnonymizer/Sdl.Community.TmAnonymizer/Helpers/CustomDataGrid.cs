using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public class CustomDataGrid: DataGrid
	{
		public CustomDataGrid()
		{
			SelectionChanged += CustomDataGrid_SelectionChanged;
		}
		void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedItemsList = SelectedItems;
		}
		public IList SelectedItemsList
		{
			get { return (IList)GetValue(SelectedItemsListProperty); }
			set { SetValue(SelectedItemsListProperty, value); }
		}
		public static readonly DependencyProperty SelectedItemsListProperty =
			DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGrid), new PropertyMetadata(null));
	}
}
