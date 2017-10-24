using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sdl.Community.XmlReader.View
{
    public class TreeViewMultipleSelection : TreeView
    {
        public TreeViewMultipleSelection()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            _selectedNodes = new List<TreeNode>();
            base.SelectedNode = null;
        }

        private List<TreeNode> _selectedNodes = null;
        public List<TreeNode> SelectedNodes
        {
            get
            {
                return _selectedNodes;
            }
            set
            {
                ClearSelectedNodes();
                if (value != null)
                {
                    foreach (TreeNode node in value)
                    {
                        ToggleNode(node, true);
                    }
                }
            }
        }

        private TreeNode _selectedNode;
        public new TreeNode SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                ClearSelectedNodes();
                if (value != null)
                {
                    SelectNode(value);
                }
            }
        }

        #region Managed flickering of the treeView
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }
        // Pinvoke:
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        #endregion

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // If the user clicks on a node that was not
            // previously selected, select it now.
            try
            {
                base.SelectedNode = null;

                TreeNode node = this.GetNodeAt(e.Location);
                if (node != null)
                {
                    //Allow user to click on image
                    int leftBound = node.Bounds.X; // - 20; 
                                                   // Give a little extra room
                    int rightBound = node.Bounds.Right + 10;
                    if (e.Location.X > leftBound && e.Location.X < rightBound)
                    {
                        if (e.Button == MouseButtons.Right)
                        {

                        }
                        else if (e.Button == MouseButtons.Left && _selectedNodes.Contains(node))
                        {
                            SelectNode(node);
                            //SelectSingleNode(node);
                        }
                        else
                        {
                            SelectNode(node);
                        }
                    }
                }

                base.OnMouseDown(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // If you clicked on a node that WAS previously
            // selected then, reselect it now. This will clear
            // any other selected nodes. e.g. A B C D are selected
            // the user clicks on B, now A C & D are no longer selected.
            try
            {
                // Check to see if a node was clicked on
                TreeNode node = this.GetNodeAt(e.Location);
                if (node != null)
                {
                    if (ModifierKeys == Keys.None && _selectedNodes.Contains(node))
                    {
                        // Allow user to click on image
                        int leftBound = node.Bounds.X; // - 20; 
                                                       // Give a little extra room
                        int rightBound = node.Bounds.Right + 10;
                        if (e.Location.X > leftBound && e.Location.X < rightBound)
                        {
                            if (e.Button == MouseButtons.Right)
                                ToggleNode(node, true);
                        }
                    }
                }

                base.OnMouseUp(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            // Never allow base.SelectedNode to be set!
            try
            {
                base.SelectedNode = null;
                e.Cancel = true;

                base.OnBeforeSelect(e);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            // Never allow base.SelectedNode to be set!
            try
            {
                base.OnAfterSelect(e);
                base.SelectedNode = null;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void SelectNode(TreeNode node)
        {
            try
            {
                this.BeginUpdate();

                if (_selectedNode == null || ModifierKeys == Keys.Control)
                {
                    // Ctrl+Click selects an unselected node, 
                    // or unselects a selected node.
                    bool bIsSelected = _selectedNodes.Contains(node);
                    ToggleNode(node, !bIsSelected);
                }
                else
                {
                    // Just clicked a node, select it
                    SelectSingleNode(node);
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        private void ClearSelectedNodes()
        {
            try
            {
                foreach (TreeNode node in _selectedNodes)
                {
                    node.BackColor = this.BackColor;
                    node.ForeColor = this.ForeColor;
                }
            }
            finally
            {
                _selectedNodes.Clear();
                _selectedNode = null;
            }
        }

        private void SelectSingleNode(TreeNode node)
        {
            if (node == null) { return; }

            ClearSelectedNodes();
            ToggleNode(node, true);
            node.EnsureVisible();
        }

        private void ToggleNode(TreeNode node, bool bSelectNode)
        {
            if (bSelectNode)
            {
                _selectedNode = node;
                if (!_selectedNodes.Contains(node))
                {
                    _selectedNodes.Add(node);
                }
                node.BackColor = SystemColors.Highlight;
                node.ForeColor = SystemColors.HighlightText;
            }
            else
            {
                _selectedNodes.Remove(node);
                node.BackColor = this.BackColor;
                node.ForeColor = this.ForeColor;
            }
        }

        private void HandleException(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
