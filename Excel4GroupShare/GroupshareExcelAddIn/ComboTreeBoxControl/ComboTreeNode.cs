// A ComboBox with a TreeView Drop-Down
// Bradley Smith - 2010/11/04 (updated 2016/07/08)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.ComboTreeBoxControl
{
    /// <summary>
    /// Represents a node in the ComboTreeBox. A node may have a name, text, font style, image and
    /// may contain child nodes. If so, it can be expanded or collapsed.
    /// </summary>
    [DefaultProperty("Text")]
    public class ComboTreeNode : IComparable<ComboTreeNode>, ICloneable
    {
        private CheckState _checkState;
        private Cursor _cursor;
        private bool _expanded;
        private int _expandedImageIndex;
        private string _expandedImageKey;
        private FontStyle _fontStyle;
        private Color _foreColor;
        private int _imageIndex;
        private string _imageKey;
        private string _name;
        private ComboTreeNodeCollection _nodes;
        private ComboTreeNode _parent;
        private bool _selectable;
        private object _tag;
        private string _text;
        private string _toolTip;

        /// <summary>
        /// Initialises a new instance of ComboTreeNode using default (empty) values.
        /// </summary>
        public ComboTreeNode()
        {
            _nodes = new ComboTreeNodeCollection(this);
            _name = _text = String.Empty;
            _fontStyle = FontStyle.Regular;
            _foreColor = Color.Empty;
            _expandedImageIndex = _imageIndex = -1;
            _expandedImageKey = _imageKey = String.Empty;
            _expanded = false;
            _selectable = true;
        }

        /// <summary>
        /// Initialises a new instance of ComboTreeNode with the specified text.
        /// </summary>
        /// <param name="text"></param>
        public ComboTreeNode(string text) : this()
        {
            this._text = text;
        }

        /// <summary>
        /// Initialises a new instance of ComboTreeNode with the specified name and text.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public ComboTreeNode(string name, string text) : this()
        {
            this._text = text;
            this._name = name;
        }

        /// <summary>
        /// Fired when the value of the <see cref="CheckState"/> property changes.
        /// </summary>
        [Browsable(false)]
        internal event EventHandler CheckStateChanged;

        /// <summary>
        /// Gets or sets the checked state when the <see cref="ComboTreeBox.ShowCheckBoxes"/> property is set to true.
        /// </summary>
        [DefaultValue(false), Category("Appearance")]
        public bool Checked
        {
            get { return (_checkState == CheckState.Checked); }
            set
            {
                CheckState = value ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        /// <summary>
        /// Gets or sets the check state when the <see cref="ComboTreeBox.ShowCheckBoxes"/> property is set to true.
        /// </summary>
        [DefaultValue(CheckState.Unchecked), Category("Appearance")]
        public CheckState CheckState
        {
            get { return _checkState; }
            set
            {
                bool diff = (_checkState != value);
                _checkState = value;
                if (diff) OnCheckStateChanged();
            }
        }

        /// <summary>
        /// Gets or sets the cursor displayed when the user moves the mouse over the node.
        /// </summary>
        [DefaultValue(null), Description("The cursor displayed when the user moves the mouse over the node.")]
        public Cursor Cursor
        {
            get
            {
                return _cursor;
            }
            set
            {
                _cursor = value;
            }
        }

        /// <summary>
        /// Determines the zero-based depth of the node, relative to the ComboTreeBox control.
        /// </summary>
        [Browsable(false)]
        public int Depth
        {
            get
            {
                int depth = 0;
                ComboTreeNode node = this;
                while ((node = node._parent) != null) depth++;
                return depth;
            }
        }

        /// <summary>
        /// Gets or sets whether the node is expanded (i.e. its child nodes are visible). Changes are not reflected in the dropdown portion of the
        /// control until the next time it is opened.
        /// </summary>
        [Browsable(false)]
        public bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        /// <summary>
        /// Gets or sets the index of the image to use for this node when expanded.
        /// </summary>
        [DefaultValue(-1), Description("The index of the image to use for this node when expanded."), Category("Appearance")]
        public int ExpandedImageIndex
        {
            get { return _expandedImageIndex; }
            set { _expandedImageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the name of the image to use for this node when expanded.
        /// </summary>
        [DefaultValue(""), Description("The name of the image to use for this node when expanded."), Category("Appearance")]
        public string ExpandedImageKey
        {
            get { return _expandedImageKey; }
            set { _expandedImageKey = value; }
        }

        /// <summary>
        /// Gets or sets the font style to use when painting the node.
        /// </summary>
        [DefaultValue(FontStyle.Regular), Description("The font style to use when painting the node."), Category("Appearance")]
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        /// <summary>
        /// Gets or sets the colour of the node. If empty, the control's foreground colour is used.
        /// </summary>
        [DefaultValue(typeof(Color), "Empty"), Description("The colour of the node. If empty, the control's foreground colour is used.")]
        public Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the image (in the ImageList on the ComboTreeBox control) to use for this node.
        /// </summary>
        [DefaultValue(-1), Description("The index of the image (in the ImageList on the ComboTreeBox control) to use for this node."), Category("Appearance")]
        public int ImageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the name of the image to use for this node.
        /// </summary>
        [DefaultValue(""), Description("The name of the image to use for this node."), Category("Appearance")]
        public string ImageKey
        {
            get { return _imageKey; }
            set { _imageKey = value; }
        }

        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        [Description("The name of the node."), DefaultValue(""), Category("Design")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets a collection of the child nodes for this node.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("The collection of the child nodes for this node."), Category("Data")]
        public ComboTreeNodeCollection Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Gets or sets the node that owns this node, or null for a top-level node.
        /// </summary>
        [Browsable(false)]
        public ComboTreeNode Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node can be selected by the user.
        /// </summary>
        [DefaultValue(true), Description("Determines whether the node can be selected by the user.")]
        public bool Selectable
        {
            get
            {
                return _selectable;
            }
            set
            {
                if (_selectable != value)
                {
                    _selectable = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a user-defined object associated with this ComboTreeNode.
        /// </summary>
        [Description("User-defined object associated with this ComboTreeNode."), DefaultValue(""), Category("Data"), TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Gets or sets the text displayed on the node.
        /// </summary>
        [DefaultValue("ComboTreeNode"), Description("The text displayed on the node."), Category("Appearance")]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets or sets the tooltip text associated with this node.
        /// </summary>
        [DefaultValue(""), Description("The tooltip text associated with this node."), Category("Appearance")]
        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; }
        }

        /// <summary>
        /// Returns a copy of the node, optionally including all child nodes.
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public ComboTreeNode Clone(bool recursive)
        {
            ComboTreeNode that = Clone();

            if (recursive)
            {
                foreach (ComboTreeNode node in this.Nodes)
                {
                    that.Nodes.Add(node.Clone(recursive));
                }
            }

            return that;
        }

        /// <summary>
        /// Returns a copy of the node.
        /// </summary>
        /// <returns></returns>
        public virtual ComboTreeNode Clone()
        {
            ComboTreeNode that = (ComboTreeNode)Activator.CreateInstance(GetType());

            that.Name = this.Name;
            that.Text = this.Text;
            that.CheckState = this.CheckState;
            that.Expanded = this.Expanded;
            that.ExpandedImageIndex = this.ExpandedImageIndex;
            that.ExpandedImageKey = this.ExpandedImageKey;
            that.ForeColor = this.ForeColor;
            that.FontStyle = this.FontStyle;
            that.ImageIndex = this.ImageIndex;
            that.ImageKey = this.ImageKey;
            that.Selectable = this.Selectable;
            that.Tag = this.Tag;
            that.Cursor = this.Cursor;

            return that;
        }

        /// <summary>
        /// Returns the full path to this node, using the specified path separator.
        /// </summary>
        /// <param name="pathSeparator">
        /// Separator between the elements that make up the path.
        /// </param>
        /// <param name="useNodeNamesForPath">
        /// Whether to construct the path from the <see cref="Name"/> property
        /// instead of the <see cref="Text"/> property.
        /// </param>
        /// <returns>The path string.</returns>
        public string GetFullPath(string pathSeparator, bool useNodeNamesForPath)
        {
            StringBuilder s = new StringBuilder();
            ComboTreeNode node = this;

            s.Append(useNodeNamesForPath ? node.Name : node.Text);

            while ((node = node.Parent) != null)
            {
                s.Insert(0, pathSeparator);
                s.Insert(0, useNodeNamesForPath ? node.Name : node.Text);
            }

            return s.ToString();
        }

        /// <summary>
        /// Paints the node content.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="font"></param>
        /// <param name="textBounds"></param>
        /// <param name="textColor"></param>
        /// <param name="flags"></param>
        public virtual void Paint(Graphics g, Font font, Rectangle textBounds, Color textColor, TextFormatFlags flags)
        {
            TextRenderer.DrawText(g, Text, font, textBounds, textColor, flags);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="ComboTreeNode"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(_name))
                return String.Format("\"{0}\"", _text);
            else
                return String.Format("{0} \"{1}\"", _name, _text);
        }

        /// <summary>
        /// Returns the aggregate check state of this node's children.
        /// </summary>
        /// <returns></returns>
        internal CheckState GetAggregateCheckState()
        {
            CheckState state = CheckState.Unchecked;
            bool all = true;
            bool any = false;
            bool chk = false;

            foreach (ComboTreeNode child in Nodes)
            {
                if (child.CheckState != CheckState.Unchecked) any = true;
                if (child.CheckState != CheckState.Checked) all = false;
                if (child.CheckState == CheckState.Checked) chk = true;
            }

            if (all & chk)
                state = CheckState.Checked;
            else if (any)
                state = CheckState.Indeterminate;

            return state;
        }

        /// <summary>
        /// Raises the <see cref="CheckStateChanged"/> event.
        /// </summary>
        protected virtual void OnCheckStateChanged()
        {
            if (CheckStateChanged != null) CheckStateChanged(this, EventArgs.Empty);
        }

        #region IComparable<ComboTreeNode> Members

        /// <summary>
        /// Compares two ComboTreeNode objects using a culture-invariant, case-insensitive comparison of the Text property.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ComboTreeNode other)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(this._text, other._text);
        }

        #endregion IComparable<ComboTreeNode> Members

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion ICloneable Members
    }

    /// <summary>
    /// Arguments for the <see cref="ComboTreeBox.AfterCheck"/> event.
    /// </summary>
    [Serializable]
    public class ComboTreeNodeEventArgs : EventArgs
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ComboTreeNodeEventArgs"/> class using the specified node.
        /// </summary>
        /// <param name="node"></param>
        public ComboTreeNodeEventArgs(ComboTreeNode node)
        {
            Node = node;
        }

        /// <summary>
        /// Gets the affected node.
        /// </summary>
        public ComboTreeNode Node
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Arguments for the <see cref="ComboTreeDropDown.DrawNode"/> event.
    /// </summary>
    [Serializable]
    public class ComboTreeNodePaintEventArgs : EventArgs
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ComboTreeNodePaintEventArgs"/> class using the specified values.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="node"></param>
        /// <param name="bounds"></param>
        /// <param name="textBounds"></param>
        /// <param name="font"></param>
        /// <param name="isHighlighted"></param>
        public ComboTreeNodePaintEventArgs(Graphics graphics, ComboTreeNode node, Rectangle bounds, Rectangle textBounds, Font font, bool isHighlighted)
        {
            Graphics = graphics;
            Node = node;
            Bounds = bounds;
            TextBounds = textBounds;
            Font = font;
            IsHighlighed = isHighlighted;
            DrawDefault = true;
        }

        /// <summary>
        /// Gets the bounds within which the content is drawn.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to draw the node using default logic.
        /// </summary>
        public bool DrawDefault { get; set; }

        /// <summary>
        /// Gets the <see cref="System.Drawing.Font"/> used on the node.
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// Gets the <see cref="Graphics"/> surface on which the content is drawn.
        /// </summary>
        public Graphics Graphics { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the node is currently highlighted.
        /// </summary>
        public bool IsHighlighed { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComboTreeNode"/> to paint.
        /// </summary>
        public ComboTreeNode Node { get; private set; }

        /// <summary>
        /// Gets the bounds within which the text is drawn.
        /// </summary>
        public Rectangle TextBounds { get; private set; }

        /// <summary>
        /// Gets the format flags used for the text on the node.
        /// </summary>
        public TextFormatFlags TextFormatFlags
        {
            get
            {
                return ComboTreeDropDown.TEXT_FORMAT_FLAGS;
            }
        }
    }
}