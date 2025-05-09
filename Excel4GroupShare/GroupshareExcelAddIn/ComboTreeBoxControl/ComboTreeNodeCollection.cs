// A ComboBox with a TreeView Drop-Down
// Bradley Smith - 2010/11/04 (updated 2015/04/14)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GroupshareExcelAddIn.ComboTreeBoxControl
{
    /// <summary>
    /// Represents a collection of <see cref="ComboTreeNode"/> objects contained
    /// within a node or a <see cref="ComboTreeBox"/> control. Supports change
    /// notification through <see cref="INotifyCollectionChanged"/>. Implements
    /// the non-generic <see cref="IList"/> to provide design-time support.
    /// </summary>
    public class ComboTreeNodeCollection : IList<ComboTreeNode>, IList, INotifyCollectionChanged
    {
        private List<ComboTreeNode> _innerList;

        private ComboTreeNode _node;

        /// <summary>
        /// Initalises a new instance of ComboTreeNodeCollection and associates it with the specified ComboTreeNode.
        /// </summary>
        /// <param name="node"></param>
        internal ComboTreeNodeCollection(ComboTreeNode node)
        {
            _innerList = new List<ComboTreeNode>();
            _node = node;
        }

        /// <summary>
        /// Fired when the check state of a node in the collection (or one of its children) changes.
        /// </summary>
        [Browsable(false)]
        internal event EventHandler<ComboTreeNodeEventArgs> AfterCheck;

        /// <summary>
        /// Gets the node with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ComboTreeNode this[string name]
        {
            get
            {
                foreach (ComboTreeNode o in this)
                {
                    if (Object.Equals(o.Name, name)) return o;
                }

                throw new KeyNotFoundException();
            }
        }

        /// <summary>
        /// Creates a node and adds it to the collection.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ComboTreeNode Add(string text)
        {
            ComboTreeNode item = new ComboTreeNode(text);
            Add(item);
            return item;
        }

        /// <summary>
        /// Creates a node and adds it to the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ComboTreeNode Add(string name, string text)
        {
            ComboTreeNode item = new ComboTreeNode(name, text);
            Add(item);
            return item;
        }

        /// <summary>
        /// Adds a range of ComboTreeNode to the collection.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<ComboTreeNode> items)
        {
            foreach (ComboTreeNode item in items)
            {
                _innerList.Add(item);
                item.Parent = _node;
                AddEventHandlers(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determines whether the collection contains a node with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsKey(string name)
        {
            foreach (ComboTreeNode o in this)
            {
                if (Object.Equals(o.Name, name)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the <see cref="ComboTreeNode"/> with the specified node text.
        /// </summary>
        /// <param name="text">The text to match.</param>
        /// <param name="comparisonType">The type of string comparison performed.</param>
        /// <param name="recurse">Whether to search recursively through all child nodes.</param>
        /// <returns></returns>
        public ComboTreeNode Find(string text, StringComparison comparisonType, bool recurse)
        {
            IEnumerator<ComboTreeNode> nodes = recurse ? GetNodesRecursive(this, false) : GetEnumerator();

            while (nodes.MoveNext())
            {
                if (nodes.Current.Text.Equals(text, comparisonType))
                {
                    return nodes.Current;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the <see cref="ComboTreeNode"/> with the specified criteria.
        /// </summary>
        /// <param name="predicate">Function to use when matching nodes.</param>
        /// <param name="recurse">Whether to search recursively through all child nodes.</param>
        /// <returns></returns>
        public ComboTreeNode Find(Func<ComboTreeNode, bool> predicate, bool recurse)
        {
            IEnumerator<ComboTreeNode> nodes = recurse ? GetNodesRecursive(this, false) : GetEnumerator();

            while (nodes.MoveNext())
            {
                if (predicate(nodes.Current))
                {
                    return nodes.Current;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the index of the node with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < _innerList.Count; i++)
            {
                if (Object.Equals(_innerList[i].Name, name)) return i;
            }

            return -1;
        }

        /// <summary>
        /// Removes the node with the specified name from the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            for (int i = 0; i < _innerList.Count; i++)
            {
                if (Object.Equals(_innerList[i].Name, name))
                {
                    ComboTreeNode item = _innerList[i];
                    RemoveEventHandlers(item);
                    _innerList.RemoveAt(i);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                    return true;
                }
            }

            return false;
        }

        internal static IEnumerator<ComboTreeNode> GetNodesRecursive(ComboTreeNodeCollection collection, bool reverse)
        {
            if (!reverse)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    yield return collection[i];
                    IEnumerator<ComboTreeNode> e = GetNodesRecursive(collection[i].Nodes, reverse);
                    while (e.MoveNext()) yield return e.Current;
                }
            }
            else
            {
                for (int i = (collection.Count - 1); i >= 0; i--)
                {
                    IEnumerator<ComboTreeNode> e = GetNodesRecursive(collection[i].Nodes, reverse);
                    while (e.MoveNext()) yield return e.Current;
                    yield return collection[i];
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="ComboTreeNode"/> that corresponds to the specified path string.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <param name="useNodeNamesForPath">Whether the path is constructed from the name of the node instead of its text.</param>
        /// <returns>The node, or null if the path is empty.</returns>
        internal ComboTreeNode ParsePath(string path, string pathSeparator, bool useNodeNamesForPath)
        {
            ComboTreeNode select = null;

            string[] parts = path.Split(new string[] { pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                ComboTreeNodeCollection collection = ((select == null) ? this : select.Nodes);
                if (useNodeNamesForPath)
                {
                    try
                    {
                        select = collection[parts[i]];
                    }
                    catch (KeyNotFoundException ex)
                    {
                        throw new ArgumentException("Invalid path string.", "value", ex);
                    }
                }
                else
                {
                    bool found = false;
                    foreach (ComboTreeNode node in collection)
                    {
                        if (node.Text.Equals(parts[i], StringComparison.InvariantCultureIgnoreCase))
                        {
                            select = node;
                            found = true;
                            break;
                        }
                    }
                    if (!found) throw new ArgumentException("Invalid path string.", "value");
                }
            }

            return select;
        }

        /// <summary>
        /// Sorts the collection and its entire sub-tree using the specified comparer.
        /// </summary>
        /// <param name="comparer"></param>
        internal void Sort(IComparer<ComboTreeNode> comparer)
        {
            if (comparer == null) comparer = Comparer<ComboTreeNode>.Default;
            SortInternal(comparer);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Raises the <see cref="AfterCheck"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAfterCheck(ComboTreeNodeEventArgs e)
        {
            if (AfterCheck != null) AfterCheck(this, e);
        }

        /// <summary>
        /// Raises the CollectionChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null) CollectionChanged(this, e);
        }

        /// <summary>
        /// Adds event handlers to the specified node.
        /// </summary>
        /// <param name="item"></param>
        private void AddEventHandlers(ComboTreeNode item)
        {
            item.CheckStateChanged += item_CheckStateChanged;
            item.Nodes.CollectionChanged += CollectionChanged;
            item.Nodes.AfterCheck += AfterCheck;
        }

        private void item_CheckStateChanged(object sender, EventArgs e)
        {
            OnAfterCheck(new ComboTreeNodeEventArgs(sender as ComboTreeNode));
        }

        /// <summary>
        /// Removes event handlers from the specified node.
        /// </summary>
        /// <param name="item"></param>
        private void RemoveEventHandlers(ComboTreeNode item)
        {
            item.CheckStateChanged -= item_CheckStateChanged;
            item.Nodes.CollectionChanged -= CollectionChanged;
            item.Nodes.AfterCheck -= AfterCheck;
        }

        /// <summary>
        /// Recursive helper method for Sort(IComparer&lt;ComboTreeNode&gt;).
        /// </summary>
        /// <param name="comparer"></param>
        private void SortInternal(IComparer<ComboTreeNode> comparer)
        {
            _innerList.Sort(comparer);
            foreach (ComboTreeNode node in _innerList)
            {
                node.Nodes.Sort(comparer);
            }
        }

        #region ICollection<ComboTreeNode> Members

        /// <summary>
        /// Gets the number of nodes in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _innerList.Count;
            }
        }

        bool ICollection<ComboTreeNode>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a node to the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(ComboTreeNode item)
        {
            _innerList.Add(item);
            item.Parent = _node;
            AddEventHandlers(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            foreach (ComboTreeNode item in _innerList)
            {
                RemoveEventHandlers(item);
            }
            _innerList.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determines whether the collection contains the specified node.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ComboTreeNode item)
        {
            return _innerList.Contains(item);
        }

        /// <summary>
        /// Copies all the nodes from the collection to a compatible array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ComboTreeNode[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified node from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ComboTreeNode item)
        {
            if (_innerList.Remove(item))
            {
                RemoveEventHandlers(item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }

            return false;
        }

        #endregion ICollection<ComboTreeNode> Members

        #region IEnumerable<ComboTreeNode> Members

        /// <summary>
        /// Returns an enumerator which can be used to cycle through the nodes in the collection (non-recursive).
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ComboTreeNode> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        #endregion IEnumerable<ComboTreeNode> Members

        #region IList<ComboTreeNode> Members

        /// <summary>
        /// Gets or sets the node at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ComboTreeNode this[int index]
        {
            get
            {
                return _innerList[index];
            }
            set
            {
                ComboTreeNode oldItem = _innerList[index];
                _innerList[index] = value;
                value.Parent = _node;
                AddEventHandlers(value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem));
            }
        }

        /// <summary>
        /// Returns the index of the specified node.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ComboTreeNode item)
        {
            return _innerList.IndexOf(item);
        }

        /// <summary>
        /// Inserts a node into the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ComboTreeNode item)
        {
            _innerList.Insert(index, item);
            item.Parent = _node;
            AddEventHandlers(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Removes the node at the specified index from the collection.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            ComboTreeNode item = _innerList[index];
            RemoveEventHandlers(item);
            _innerList.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        #endregion IList<ComboTreeNode> Members

        #region IEnumerable Members (implemented explicitly)

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        #endregion IEnumerable Members (implemented explicitly)

        #region IList Members (implemented explicitly)

        bool System.Collections.IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (ComboTreeNode)value;
            }
        }

        int IList.Add(object value)
        {
            Add((ComboTreeNode)value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Contains((ComboTreeNode)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((ComboTreeNode)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (ComboTreeNode)value);
        }

        void IList.Remove(object value)
        {
            Remove((ComboTreeNode)value);
        }

        #endregion IList Members (implemented explicitly)

        #region ICollection Members (implemented explicitly)

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)_innerList).IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)_innerList).SyncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_innerList).CopyTo(array, index);
        }

        #endregion ICollection Members (implemented explicitly)

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Fired when the collection (sub-tree) changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion INotifyCollectionChanged Members
    }
}