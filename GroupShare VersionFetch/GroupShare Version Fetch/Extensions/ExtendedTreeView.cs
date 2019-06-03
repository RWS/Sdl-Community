using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.GSVersionFetch.Extensions
{
	public class ExtendedTreeView : TreeView
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			var childItem = ExtendedTreeViewItem.CreateItemWithBinding();

			childItem.OnHierarchyMouseUp += OnChildItemMouseLeftButtonUp;

			return childItem;
		}

		private void OnChildItemMouseLeftButtonUp(object sender, MouseEventArgs e)
		{
			OnHierarchyMouseUp?.Invoke(this, e);
		}

		public event MouseEventHandler OnHierarchyMouseUp;
	}
}
