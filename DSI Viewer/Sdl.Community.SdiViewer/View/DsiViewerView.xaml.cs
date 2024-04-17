using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.DsiViewer.View
{
	/// <summary>
	/// Interaction logic for SdiWpfControl.xaml
	/// </summary>
	public partial class DsiViewerView : UserControl, IUIControl
    {
        public DsiViewerView()
        {
            InitializeComponent();
        }

		public void Dispose() { }

		private void TabKeyNavigation(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Tab)
			{
				return;
			}

			var currentUIElement = sender as UIElement;
			var container = VisualTreeHelper.GetParent(currentUIElement) as UIElement;
			var searchBackwards = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
			HandleTabKeyNavigation(container, currentUIElement, e, searchBackwards);

			if (!e.Handled)
			{
				HandleTabKeyNavigationAtEdges(container, e, searchBackwards);
			}
		}

		private void HandleTabKeyNavigationAtEdges(UIElement container, KeyEventArgs e, bool searchBackwards)
		{
			var childrenCount = VisualTreeHelper.GetChildrenCount(container);
			var startIndex = searchBackwards ? childrenCount - 1 : 0;
			var focusableChild = FindFocusableChild(container, startIndex, null, searchBackwards);
			focusableChild?.Focus();
			e.Handled = true;
		}

		private void HandleTabKeyNavigation(UIElement container, UIElement uiElement, KeyEventArgs e, bool searchBackwards)
		{
			var startIndex = GetCurrentUIElementIndex(container, uiElement);
			if (FindFocusableChild(container, startIndex, uiElement, searchBackwards) is UIElement focusableElement)
			{
				focusableElement.Focus();
				e.Handled = true;
				return;
			}

			if (container == MainGrid)
			{
				return;
			}

			uiElement = container;
			container = VisualTreeHelper.GetParent(container) as UIElement;
			HandleTabKeyNavigation(container, uiElement, e, searchBackwards);
		}

		private UIElement FindFocusableChild(UIElement uiElement, int startIndex, UIElement skipElement, bool searchBackwards = false)
		{
			if (uiElement == skipElement)
			{
				return null;
			}

			if (uiElement is UIElement { IsVisible: true, Focusable: true, IsEnabled: true })
			{
				return uiElement;
			}

			var childrenCount = VisualTreeHelper.GetChildrenCount(uiElement);
			var endIndex = searchBackwards ? -1 : childrenCount;
			var increment = searchBackwards ? -1 : 1;
			for (var i = startIndex; i != endIndex; i += increment)
			{
				if (VisualTreeHelper.GetChild(uiElement, i) is not UIElement child)
				{
					continue;
				}

				var nextStartIndex = searchBackwards ? VisualTreeHelper.GetChildrenCount(child) - 1 : 0;
				var focusableChild = FindFocusableChild(child, nextStartIndex, skipElement, searchBackwards);
				if (focusableChild is not null)
				{
					return focusableChild;
				}
			}

			return null;
		}

		private int GetCurrentUIElementIndex(UIElement container, UIElement uiElement)
		{
			var childrenCount = VisualTreeHelper.GetChildrenCount(container);
			for (var i = 0; i < childrenCount; i++)
			{
				if (VisualTreeHelper.GetChild(container, i) == uiElement)
				{
					return i;
				}
			}

			return -1;
		}

		private void DataGridGotFocus(object sender, RoutedEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			var currentCell = dataGrid.CurrentCell;
			if (currentCell.Column is not null
			 || dataGrid == null
			 || dataGrid.Items.Count <= 0
			 || dataGrid.Columns.Count <= 0)
			{
				return;
			}

			var cellInfo = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[0]);
			dataGrid.CurrentCell = cellInfo;
			dataGrid.ScrollIntoView(dataGrid.Items[0]);
			dataGrid.BeginEdit();
			e.Handled = true;
		}
	}
}