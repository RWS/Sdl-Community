// A ComboBox with a TreeView Drop-Down
// Bradley Smith - 2010/11/04 (updated 2016/10/25)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.ComboTreeBoxControl
{
    /// <summary>
    /// Represents a control which provides ComboBox-like functionality, displaying its
    /// dropdown items (nodes) in a manner similar to a TreeView control.
    /// </summary>
    [ToolboxItem(true), DesignerCategory("")]
    public class ComboTreeBox : DropDownControlBase
    {
        internal const string DEFAULT_CHECKED_NODE_SEPARATOR = " | ";
        internal const string DEFAULT_PATH_SEPARATOR = @"\";
        internal const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.TextBoxControl | TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.PathEllipsis;
        private bool _cascadeCheckState;
        private string _checkedNodeSeparator;
        private ComboTreeDropDown _dropDown;
        private int _expandedImageIndex;
        private string _expandedImageKey;
        private int _imageIndex;
        private string _imageKey;
        private ImageList _images;
        private bool _isUpdating;
        private ComboTreeNodeCollection _nodes;
        private string _nullValue;
        private string _pathSeparator;
        private int _recurseDepth;
        private ComboTreeNode _selectedNode;
        private bool _showCheckBoxes;
        private bool _showGlyphs;
        private bool _showPath;
        private int _suspendCheckEvents;
        private bool _threeState;
        private bool _useNodeNamesForPath;

        /// <summary>
        /// Initalises a new instance of ComboTreeBox.
        /// </summary>
        public ComboTreeBox()
        {
            // default property values
            _nullValue = String.Empty;
            _pathSeparator = DEFAULT_PATH_SEPARATOR;
            _checkedNodeSeparator = DEFAULT_CHECKED_NODE_SEPARATOR;
            _expandedImageIndex = _imageIndex = 0;
            _expandedImageKey = _imageKey = String.Empty;
            _cascadeCheckState = true;
            _showGlyphs = true;

            // nodes collection
            Nodes = new ComboTreeNodeCollection(null);

            // dropdown portion
            _dropDown = new ComboTreeDropDown(this);
            _dropDown.Opened += new EventHandler(dropDown_Opened);
            _dropDown.Closed += new ToolStripDropDownClosedEventHandler(dropDown_Closed);
            _dropDown.UpdateVisibleItems();
        }

        /// <summary>
        /// Fired when the value of a node's <see cref="ComboTreeNode.CheckState"/> property changes.
        /// </summary>
        [Description("Occurs when a node checkbox is checked.")]
        public event EventHandler<ComboTreeNodeEventArgs> AfterCheck;

        /// <summary>
        /// Fired when a node is clicked, regardless of whether it can be selected.
        /// </summary>
        [Description("Occurs when a node is clicked, regardless of whether it can be selected.")]
        public event EventHandler<ComboTreeNodeEventArgs> NodeClick;

        /// <summary>
        /// Fired when the value of the <see cref="SelectedNode"/> property changes.
        /// </summary>
        [Description("Occurs when the SelectedNode property changes.")]
        public event EventHandler SelectedNodeChanged;

        /// <summary>
        /// Gets the (recursive) superset of the entire tree of nodes contained
        /// within the control.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<ComboTreeNode> AllNodes
        {
            get
            {
                IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, false);
                while (e.MoveNext()) yield return e.Current;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the check state of a node is
        /// determined by its child nodes, and vice versa. If set to true, this
        /// means that only the check state of leaf nodes is significant.
        /// </summary>
        [DefaultValue(true), Description("Determines whether the check state of a node is determined by its child nodes, and vice versa."), Category("Behavior")]
        public bool CascadeCheckState
        {
            get
            {
                return _cascadeCheckState;
            }
            set
            {
                bool diff = (_cascadeCheckState != value);
                _cascadeCheckState = value;

                if (diff && _cascadeCheckState)
                {
                    // apply cascading state
                    IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, true);
                    while (e.MoveNext())
                    {
                        if (e.Current.Nodes.Count > 0) e.Current.CheckState = e.Current.GetAggregateCheckState();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a (recursive) sequence containing the nodes whose
        /// <see cref="ComboTreeNode.CheckState"/> property is equal to
        /// <see cref="CheckState.Checked"/>. If the <see cref="CascadeCheckState"/>
        /// property is set to true, only leaf nodes are included.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<ComboTreeNode> CheckedNodes
        {
            get
            {
                IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, false);
                while (e.MoveNext())
                {
                    if (e.Current.Checked)
                    {
                        if (_cascadeCheckState && (e.Current.Nodes.Count > 0)) continue;
                        yield return e.Current;
                    }
                }
            }
            set
            {
                IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, false);
                while (e.MoveNext())
                {
                    e.Current.Checked = false;
                }
                foreach (ComboTreeNode node in value)
                {
                    node.Checked = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the string used to separate the checked nodes.
        /// </summary>
        [DefaultValue(DEFAULT_CHECKED_NODE_SEPARATOR), Description("The string used to separate the checked nodes."), Category("Appearance")]
        public string CheckedNodeSeparator
        {
            get { return _checkedNodeSeparator; }
            set { _checkedNodeSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the maximum height of the dropdown portion of the control.
        /// </summary>
        [DefaultValue(ComboTreeDropDown.DEFAULT_DROPDOWN_HEIGHT), Description("The maximum height of the dropdown portion of the control."), Category("Behavior")]
        public int DropDownHeight
        {
            get
            {
                return _dropDown.DropDownHeight;
            }
            set
            {
                _dropDown.DropDownHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the dropdown portion of the control.
        /// Value must be greater than or equal to the width of the control.
        /// </summary>
        [Description("The width of the dropdown portion of the control. Value must be greater than or equal to the width of the control."), Category("Behavior")]
        public int DropDownWidth
        {
            get
            {
                return Math.Max(_dropDown.DropDownWidth, Width);
            }
            set
            {
                _dropDown.DropDownWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the dropdown portion of the control is displayed.
        /// </summary>
        [Browsable(false)]
        public override bool DroppedDown
        {
            get
            {
                return base.DroppedDown;
            }
            set
            {
                SetDroppedDown(value, true);
            }
        }

        /// <summary>
        /// Gets or sets the index of the default image to use for nodes when expanded.
        /// </summary>
        [DefaultValue(0), Description("The index of the default image to use for nodes when expanded."), Category("Appearance")]
        public int ExpandedImageIndex
        {
            get { return _expandedImageIndex; }
            set
            {
                _expandedImageIndex = value;
                _dropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        /// Gets or sets the name of the default image to use for nodes when expanded.
        /// </summary>
        [DefaultValue(""), Description("The name of the default image to use for nodes when expanded."), Category("Appearance")]
        public string ExpandedImageKey
        {
            get { return _expandedImageKey; }
            set
            {
                _expandedImageKey = value;
                _dropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        /// Gets or sets the first visible ComboTreeNode in the drop-down portion of the control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboTreeNode FirstVisibleNode
        {
            get
            {
                return _dropDown.TopNode;
            }
            set
            {
                _dropDown.TopNode = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the default image to use for nodes.
        /// </summary>
        [DefaultValue(0), Description("The index of the default image to use for nodes."), Category("Appearance")]
        public int ImageIndex
        {
            get { return _imageIndex; }
            set
            {
                _imageIndex = value;
                _dropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        /// Gets or sets the name of the default image to use for nodes.
        /// </summary>
        [DefaultValue(""), Description("The name of the default image to use for nodes."), Category("Appearance")]
        public string ImageKey
        {
            get { return _imageKey; }
            set
            {
                _imageKey = value;
                _dropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        /// Gets or sets an ImageList component which provides the images displayed beside nodes in the control.
        /// </summary>
        [DefaultValue(null), Description("An ImageList component which provides the images displayed beside nodes in the control."), Category("Appearance")]
        public ImageList Images
        {
            get { return _images; }
            set
            {
                _images = value;
                _dropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        /// Gets or sets the collection of top-level nodes contained by the control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("The collection of top-level nodes contained by the control."), Category("Data")]
        public ComboTreeNodeCollection Nodes
        {
            get { return _nodes; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                if (_nodes != null)
                {
                    _nodes.AfterCheck -= nodes_AfterCheck;
                    _nodes.CollectionChanged -= nodes_CollectionChanged;
                }

                _nodes = value;
                _nodes.CollectionChanged += nodes_CollectionChanged;
                _nodes.AfterCheck += nodes_AfterCheck;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed in the editable portion of the control if the SelectedNode property is null.
        /// </summary>
        [DefaultValue(""), Description("The text displayed in the editable portion of the control if the SelectedNode property is null."), Category("Appearance")]
        public string NullValue
        {
            get { return _nullValue; }
            set
            {
                _nullValue = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the path to the selected node.
        /// </summary>
        [DefaultValue(""), Description("The path to the selected node."), Category("Behavior")]
        public string Path
        {
            get
            {
                if (_selectedNode != null)
                    return GetFullPath(_selectedNode);
                else
                    return String.Empty;
            }
            set
            {
                SelectedNode = _nodes.ParsePath(value, _pathSeparator, _useNodeNamesForPath);
            }
        }

        /// <summary>
        /// Gets or sets the string used to separate nodes in the Path property.
        /// </summary>
        [DefaultValue(DEFAULT_PATH_SEPARATOR), Description("The string used to separate nodes in the path string."), Category("Behavior")]
        public string PathSeparator
        {
            get { return _pathSeparator; }
            set
            {
                _pathSeparator = value;
                if (_showPath) Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the node selected in the control.
        /// </summary>
        [Browsable(false)]
        public ComboTreeNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (!OwnsNode(value)) throw new ArgumentException("Node does not belong to this control.", "value");
                if ((value != null) && !value.Selectable) throw new ArgumentException("Node is not selectable.", "value");
                SetSelectedNode(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a checkbox is shown beside each node.
        /// </summary>
        [DefaultValue(false), Description("Determines whether a checkbox is shown beside each node."), Category("Behavior")]
        public bool ShowCheckBoxes
        {
            get { return _showCheckBoxes; }
            set
            {
                _showCheckBoxes = value;
                _selectedNode = null;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether connector lines and expand/collapse glyphs are shown.
        /// </summary>
        [DefaultValue(true), Description("Determines whether connector lines and expand/collapse glyphs are shown.")]
        public bool ShowGlyphs
        {
            get
            {
                return _showGlyphs;
            }
            set
            {
                if (_showGlyphs != value)
                {
                    _showGlyphs = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Determines whether the full path to the selected node is displayed in the editable portion of the control.
        /// </summary>
        [DefaultValue(false), Description("Determines whether the path to the selected node is displayed in the editable portion of the control."), Category("Appearance")]
        public bool ShowPath
        {
            get { return _showPath; }
            set
            {
                _showPath = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Hides the Text property from the designer.
        /// </summary>
        [Browsable(false)]
        public override string Text
        {
            get
            {
                return String.Empty;
            }
            set
            {
                base.Text = String.Empty;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether node checkboxes move into the <see cref="CheckState.Indeterminate"/> state after the <see cref="CheckState.Checked"/> state.
        /// </summary>
        [DefaultValue(false), Description("Determines whether node checkboxes move into the indeterminate/mixed state after the checked state."), Category("Behavior")]
        public bool ThreeState
        {
            get { return _threeState; }
            set { _threeState = value; }
        }

        /// <summary>
        /// Determines whether the <see cref="ComboTreeNode.Name"/> property of the nodes is used to construct the path string.
        /// The default behaviour is to use the <see cref="ComboTreeNode.Text"/> property.
        /// </summary>
        [DefaultValue(false), Description("Determines whether the Name property of the nodes is used to construct the path string. The default behaviour is to use the Text property."), Category("Behavior")]
        public bool UseNodeNamesForPath
        {
            get { return _useNodeNamesForPath; }
            set
            {
                _useNodeNamesForPath = value;
                if (_showPath) Invalidate();
            }
        }

        /// <summary>
        /// Gets the number of ComboTreeNodes visible in the drop-down portion of the control.
        /// </summary>
        [Browsable(false)]
        public int VisibleCount
        {
            get
            {
                return _dropDown.VisibleCount;
            }
        }

        /// <summary>
        /// Gets whether the owning control is displaying focus cues.
        /// </summary>
        internal bool ShowsFocusCues
        {
            get
            {
                return base.ShowFocusCues;
            }
        }

        /// <summary>
        /// Gets a value indicating whether connector lines should be rendered.
        /// </summary>
        protected internal bool ConnectorsNeeded
        {
            get
            {
                if (ShowGlyphs)
                {
                    foreach (ComboTreeNode node in AllNodes)
                    {
                        if (node.Depth > 0) return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the toolstrip drop-down control that displays the nodes.
        /// </summary>
        protected ComboTreeDropDown DropDownControl
        {
            get
            {
                return _dropDown;
            }
        }

        /// <summary>
        /// Prevents the dropdown portion of the control from being updated until the EndUpdate method is called.
        /// </summary>
        public void BeginUpdate()
        {
            _isUpdating = true;
        }

        /// <summary>
        /// Checks all nodes in the tree.
        /// </summary>
        public void CheckAll()
        {
            if (!ShowCheckBoxes) return;
            foreach (ComboTreeNode node in AllNodes) node.Checked = true;
        }

        /// <summary>
        /// Collapses all nodes in the tree for when the dropdown portion of the control is reopened.
        /// </summary>
        public void CollapseAll()
        {
            foreach (ComboTreeNode node in AllNodes) node.Expanded = false;
        }

        /// <summary>
        /// Updates the dropdown portion of the control after being suspended by the BeginUpdate method.
        /// </summary>
        public void EndUpdate()
        {
            _isUpdating = false;
            if (!OwnsNode(_selectedNode)) SetSelectedNode(null);
            _dropDown.UpdateVisibleItems();
        }

        /// <summary>
        /// Expands all nodes in the tree for when the dropdown portion of the control is reopened.
        /// </summary>
        public void ExpandAll()
        {
            foreach (ComboTreeNode node in AllNodes) if (node.Nodes.Count > 0) node.Expanded = true;
        }

        /// <summary>
        /// Returns the full path to the specified <see cref="ComboTreeNode"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string GetFullPath(ComboTreeNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.GetFullPath(_pathSeparator, _useNodeNamesForPath);
        }

        /// <summary>
        /// Returns the next selectable node, relative to the selected node.
        /// </summary>
        /// <returns></returns>
        public ComboTreeNode GetNextSelectableNode()
        {
            bool started = false;
            IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, false);
            while (e.MoveNext())
            {
                if (started || (_selectedNode == null))
                {
                    if (IsNodeVisible(e.Current) && e.Current.Selectable) return e.Current;
                }
                else if (e.Current == _selectedNode)
                {
                    started = true;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the node at the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ComboTreeNode GetNodeAt(string path)
        {
            return _nodes.ParsePath(path, _pathSeparator, _useNodeNamesForPath);
        }

        /// <summary>
        /// Returns the previous selectable node, relative to the selected node.
        /// </summary>
        /// <returns></returns>
        public ComboTreeNode GetPrevSelectableNode()
        {
            bool started = false;
            IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, true);
            while (e.MoveNext())
            {
                if (started || (_selectedNode == null))
                {
                    if (IsNodeVisible(e.Current) && e.Current.Selectable) return e.Current;
                }
                else if (e.Current == _selectedNode)
                {
                    started = true;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified node belongs to this ComboTreeBox, and
        /// hence is a valid selection. For the purposes of this method, a null
        /// value is always a valid selection.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool OwnsNode(ComboTreeNode node)
        {
            if (node == null) return true;

            ComboTreeNode parent = node;
            while (parent.Parent != null) parent = parent.Parent;
            return _nodes.Contains(parent);
        }

        /// <summary>
        /// Sorts the contents of the tree using the default comparer.
        /// </summary>
        public void Sort()
        {
            Sort(null);
        }

        /// <summary>
        /// Sorts the contents of the tree using the specified comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<ComboTreeNode> comparer)
        {
            bool oldIsUpdating = _isUpdating;
            _isUpdating = true;
            _nodes.Sort(comparer);
            if (!oldIsUpdating) EndUpdate();
        }

        /// <summary>
        /// Un-checks all nodes in the tree.
        /// </summary>
        public void UncheckAll()
        {
            foreach (ComboTreeNode node in AllNodes) node.Checked = false;
        }

        /// <summary>
        /// Returns the image associated with the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="images"></param>
        /// <param name="imageIndex"></param>
        /// <param name="imageKey"></param>
        /// <param name="expandedImageIndex"></param>
        /// <param name="expandedImageKey"></param>
        /// <returns></returns>
        internal static Image GetNodeImage(ComboTreeNode node, ImageList images, int imageIndex, string imageKey, int expandedImageIndex, string expandedImageKey)
        {
            if ((images != null) && (node != null))
            {
                if (node.Expanded && (node.Nodes.Count > 0))
                {
                    if (images.Images.ContainsKey(node.ExpandedImageKey))
                        return images.Images[node.ExpandedImageKey];        // node's key
                    else if (node.ExpandedImageIndex >= 0)
                        return images.Images[node.ExpandedImageIndex];      // node's index
                    else if (images.Images.ContainsKey(expandedImageKey))
                        return images.Images[expandedImageKey];             // default key
                    else if ((expandedImageIndex >= 0) && (expandedImageIndex < images.Images.Count))
                        return images.Images[expandedImageIndex];           // default index
                }
                else
                {
                    if (images.Images.ContainsKey(node.ImageKey))
                        return images.Images[node.ImageKey];        // node's key
                    else if (node.ImageIndex >= 0)
                        return images.Images[node.ImageIndex];      // node's index
                    else if (images.Images.ContainsKey(imageKey))
                        return images.Images[imageKey];             // default key
                    else if ((imageIndex >= 0) && (imageIndex < images.Images.Count))
                        return images.Images[imageIndex];           // default index
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the image referenced by the specified node in the ImageList component associated with this control.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal Image GetNodeImage(ComboTreeNode node)
        {
            return GetNodeImage(node, _images, _imageIndex, _imageKey, _expandedImageIndex, _expandedImageKey);
        }

        /// <summary>
        /// Determines whether the specified node should be displayed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal bool IsNodeVisible(ComboTreeNode node)
        {
            bool displayed = true;
            ComboTreeNode parent = node;
            while ((parent = parent.Parent) != null)
            {
                if (ShowGlyphs && !parent.Expanded)
                {
                    displayed = false;
                    break;
                }
            }
            return displayed;
        }

        /// <summary>
        /// Resumes processing of node check events.
        /// </summary>
        internal void ResumeCheckEvents()
        {
            _suspendCheckEvents--;
            if (_suspendCheckEvents < 0) _suspendCheckEvents = 0;
        }

        /// <summary>
        /// Sets the value of the DroppedDown property, optionally without raising any events.
        /// </summary>
        /// <param name="droppedDown"></param>
        /// <param name="raiseEvents"></param>
        internal void SetDroppedDown(bool droppedDown, bool raiseEvents)
        {
            base.DroppedDown = droppedDown;

            if (raiseEvents)
            {
                if (droppedDown)
                    _dropDown.Open();
                else
                    _dropDown.Close();
            }
        }

        /// <summary>
        /// Suspends processing of node check events.
        /// </summary>
        internal void SuspendCheckEvents()
        {
            _suspendCheckEvents++;
        }

        /// <summary>
        /// Raises the <see cref="AfterCheck"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnAfterCheck(ComboTreeNodeEventArgs e)
        {
            if (AfterCheck != null) AfterCheck(this, e);
        }

        /// <summary>
        /// Raises the <see cref="NodeClick"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnNodeClick(ComboTreeNodeEventArgs e)
        {
            if (NodeClick != null) NodeClick(this, e);
        }

        /// <summary>
        /// Disposes of the control and its dropdown.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) _dropDown.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the dropdown's font when the control's font changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _dropDown.Font = Font;
        }

        /// <summary>
        /// Handles keyboard shortcuts.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = e.SuppressKeyPress = true;

            if (e.Alt && (e.KeyCode == Keys.Down))
            {
                DroppedDown = true;
            }
            else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Left))
            {
                ComboTreeNode prev = GetPrevSelectableNode();
                if (prev != null) SetSelectedNode(prev);
            }
            else if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Right))
            {
                ComboTreeNode next = GetNextSelectableNode();
                if (next != null) SetSelectedNode(next);
            }
            else
            {
                e.Handled = e.SuppressKeyPress = false;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Closes the dropdown portion of the control when it loses focus.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (!_dropDown.Focused) _dropDown.Close();
        }

        /// <summary>
        /// Toggles the visibility of the dropdown portion of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left) DroppedDown = !DroppedDown;
        }

        /// <summary>
        /// Scrolls between adjacent nodes, or scrolls the drop-down portion of
        /// the control in response to the mouse wheel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            HandledMouseEventArgs he = (HandledMouseEventArgs)e;
            he.Handled = true;

            base.OnMouseWheel(e);

            if (DroppedDown)
                _dropDown.ScrollDropDown(-(e.Delta / 120) * SystemInformation.MouseWheelScrollLines);
            else if (e.Delta > 0)
            {
                ComboTreeNode prev = GetPrevSelectableNode();
                if (prev != null) SetSelectedNode(prev);
            }
            else if (e.Delta < 0)
            {
                ComboTreeNode next = GetNextSelectableNode();
                if (next != null) SetSelectedNode(next);
            }
        }

        /// <summary>
        /// Paints the selected node in the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintContent(DropDownPaintEventArgs e)
        {
            base.OnPaintContent(e);

            Image img = GetNodeImage(_selectedNode);
            string text = _nullValue;
            if (_showCheckBoxes)
                text = GetCheckedNodeString();
            else if (_selectedNode != null)
                text = (_showPath) ? Path : _selectedNode.Text;

            Rectangle imgBounds = (img == null) ? new Rectangle(1, 0, 0, 0) : new Rectangle(4, e.Bounds.Height / 2 - img.Height / 2, img.Width, img.Height);
            Rectangle txtBounds = new Rectangle(imgBounds.Right, 0, e.Bounds.Width - imgBounds.Right - 3, e.Bounds.Height);

            if (img != null) e.Graphics.DrawImage(img, imgBounds);

            TextRenderer.DrawText(e.Graphics, text, Font, txtBounds, Enabled ? ForeColor : SystemColors.GrayText, TEXT_FORMAT_FLAGS);

            // focus rectangle
            if (Focused && ShowFocusCues && !DroppedDown) e.DrawFocusRectangle();
        }

        /// <summary>
        /// Raises the <see cref="SelectedNodeChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedNodeChanged(EventArgs e)
        {
            if (SelectedNodeChanged != null) SelectedNodeChanged(this, e);
        }

        /// <summary>
        /// Facilitates various keyboard shortcuts.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F4)
            {
                DroppedDown = !DroppedDown;
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            OnDropDownClosed(EventArgs.Empty);
        }

        private void dropDown_Opened(object sender, EventArgs e)
        {
            OnDropDown(EventArgs.Empty);
        }

        /// <summary>
        /// Returns a string containing the concatenated text of the checked nodes.
        /// </summary>
        /// <returns></returns>
        private string GetCheckedNodeString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (ComboTreeNode node in CheckedNodes)
            {
                if (_showPath)
                    sb.Append(GetFullPath(node));
                else
                    sb.Append(node.Text);

                sb.Append(_checkedNodeSeparator);
            }

            if (sb.Length > 0) sb.Remove(sb.Length - _checkedNodeSeparator.Length, _checkedNodeSeparator.Length);

            return sb.ToString();
        }

        /// <summary>
        /// Returns the next displayable node, relative to the selected node.
        /// </summary>
        /// <returns></returns>
        private ComboTreeNode GetNextDisplayedNode()
        {
            bool started = false;
            IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, false);
            while (e.MoveNext())
            {
                if (started || (_selectedNode == null))
                {
                    if (IsNodeVisible(e.Current)) return e.Current;
                }
                else if (e.Current == _selectedNode)
                {
                    started = true;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the previous displayable node, relative to the selected node.
        /// </summary>
        /// <returns></returns>
        private ComboTreeNode GetPrevDisplayedNode()
        {
            bool started = false;
            IEnumerator<ComboTreeNode> e = ComboTreeNodeCollection.GetNodesRecursive(_nodes, true);
            while (e.MoveNext())
            {
                if (started || (_selectedNode == null))
                {
                    if (IsNodeVisible(e.Current)) return e.Current;
                }
                else if (e.Current == _selectedNode)
                {
                    started = true;
                }
            }

            return null;
        }

        private void nodes_AfterCheck(object sender, ComboTreeNodeEventArgs e)
        {
            if (_cascadeCheckState)
            {
                _recurseDepth++;

                if (_recurseDepth == 1)
                {
                    foreach (ComboTreeNode child in e.Node.Nodes)
                    {
                        if (_threeState)
                            child.CheckState = e.Node.CheckState;
                        else
                            child.Checked = e.Node.Checked;
                    }

                    if (e.Node.Parent != null)
                    {
                        e.Node.Parent.CheckState = e.Node.Parent.GetAggregateCheckState();
                    }
                }

                _recurseDepth--;
            }

            if (!_isUpdating)
            {
                if (_suspendCheckEvents == 0) OnAfterCheck(e);
                Invalidate();
                _dropDown.UpdateVisibleItems();
            }
        }

        private void nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_isUpdating)
            {
                // verify that selected node still belongs to the tree
                if (!OwnsNode(_selectedNode)) SetSelectedNode(null);

                // rebuild the view
                _dropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        /// Sets the value of the SelectedNode property and raises the SelectedNodeChanged event.
        /// </summary>
        /// <param name="node"></param>
        private void SetSelectedNode(ComboTreeNode node)
        {
            if ((_selectedNode != node) && !_showCheckBoxes && ((node == null) || node.Selectable))
            {
                _selectedNode = node;
                Invalidate();
                OnSelectedNodeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the value of the <see cref="DropDownWidth"/>
        /// property should be serialized by the designer.
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeDropDownWidth()
        {
            // don't serialize the value unless it exceeds the control's width
            return _dropDown.DropDownWidth > Width;
        }
    }
}