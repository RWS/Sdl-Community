using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Sdl.Community.GSVersionFetch.Extensions
{
	public class ExtendedTreeViewItem : TreeViewItem
	{
		public ExtendedTreeViewItem()
		{
			MouseLeftButtonUp += OnMouseLeftButtonUp;
		}

		void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			OnHierarchyMouseUp?.Invoke(this, e);
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			var childItem = CreateItemWithBinding();

			childItem.MouseLeftButtonUp += OnMouseLeftButtonUp;

			return childItem;
		}

		public static ExtendedTreeViewItem CreateItemWithBinding()
		{
			var tvi = new ExtendedTreeViewItem();

			var expandedBinding = new Binding("IsExpanded")
			{
				Mode = BindingMode.TwoWay
			};
			tvi.SetBinding(IsExpandedProperty, expandedBinding);

			var selectedBinding = new Binding("IsSelected")
			{
				Mode = BindingMode.TwoWay
			};
			tvi.SetBinding(IsSelectedProperty, selectedBinding);

			return tvi;
		}

		public event MouseEventHandler OnHierarchyMouseUp;
	}
}
