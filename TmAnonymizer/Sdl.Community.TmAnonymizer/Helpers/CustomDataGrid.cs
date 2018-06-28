using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
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
