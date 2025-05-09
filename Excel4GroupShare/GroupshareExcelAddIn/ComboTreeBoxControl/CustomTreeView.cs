using System.Linq;
using System.Reflection;

namespace GroupshareExcelAddIn.ComboTreeBoxControl
{
    public class CustomTreeView : ComboTreeBox
    {
        private MethodInfo _displayProperty;
        private MethodInfo _recursiveProperty;

        public CustomTreeView() : base()
        {
        }

        public void SetDataSource(object dataSource, string recursiveProperty, string displayProperty)
        {
            _recursiveProperty = dataSource?.GetType().GetProperty(recursiveProperty)?.GetMethod;
            _displayProperty = dataSource?.GetType().GetProperty(displayProperty)?.GetMethod;

            var pNode = new ComboTreeNode(_displayProperty?.Invoke(dataSource, null) as string) { Tag = dataSource };
            MapOrganizationsToNodesRecursively(dataSource, pNode);
            Nodes.Add(pNode);
            Nodes.Insert(0, new ComboTreeNode("Any"));
            Enabled = true;
            ResizeControl();
        }

        private void MapOrganizationsToNodesRecursively(object dataSource, ComboTreeNode pNode)
        {
            dynamic childrenObjects = _recursiveProperty.Invoke(dataSource, null);
            if (childrenObjects == null) return;
            foreach (var cOrg in childrenObjects)
            {
                var cNode = new ComboTreeNode(_displayProperty.Invoke(cOrg, null) as string) { Tag = cOrg };
                if (childrenObjects.Count > 0)
                {
                    MapOrganizationsToNodesRecursively(cOrg, cNode);
                    pNode.Nodes.Add(cNode);
                }
                else
                {
                    pNode.Nodes.Add(cNode);
                }
            }
        }

        private void ResizeControl()
        {
            //find longest string and use its length for resizing the dropDown
            var greatestLength = AllNodes.Select(node => node.Text).Aggregate("", (max, current) => max.Length > current.Length ? max : current).Length;
            DropDownWidth = greatestLength * 6;
            DropDownHeight = greatestLength * 9;
        }
    }
}