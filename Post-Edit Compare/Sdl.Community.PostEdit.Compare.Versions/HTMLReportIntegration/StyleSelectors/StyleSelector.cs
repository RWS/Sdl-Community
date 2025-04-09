using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.StyleSelectors
{
    public class TreeViewItemStyleSelector : StyleSelector
    {
        public Style? ParentItemStyle { get; set; }
        public Style? ChildItemStyle { get; set; }

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            if (container is TreeViewItem treeViewItem)
            {
                var parent = ItemsControl.ItemsControlFromItemContainer(treeViewItem);
                return parent is TreeView ? ParentItemStyle : ChildItemStyle;
            }
            return base.SelectStyle(item, container);
        }
    }

}