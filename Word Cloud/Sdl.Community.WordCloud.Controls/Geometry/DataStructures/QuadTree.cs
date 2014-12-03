#region

using System.Collections.Generic;
using System.Drawing;

#endregion

namespace Sdl.Community.WordCloud.Controls.Geometry.DataStructures
{
    /// <summary>
    ///   A Quadtree is a structure designed to partition space so
    ///   that it's faster to find out what is inside or outside a given 
    ///   area. See http://en.wikipedia.org/wiki/Quadtree
    ///   This QuadTree contains items that have an area (RectangleF)
    ///   it will store a reference to the item in the quad 
    ///   that is just big enough to hold it. Each quad has a bucket that 
    ///   contain multiple items.
    /// </summary>
    public class QuadTree<T>  where T : LayoutItem
    {
        #region Delegates

        public delegate void QuadTreeAction(QuadTreeNode<T> obj);

        #endregion

        private readonly RectangleF m_Rectangle;
        private readonly QuadTreeNode<T> m_Root;

        public QuadTree(RectangleF rectangle)
        {
            m_Rectangle = rectangle;
            m_Root = new QuadTreeNode<T>(m_Rectangle);
        }

        #region IQuadTree<T> Members

        public int Count
        {
            get { return m_Root.Count; }
        }

        public void Insert(T item)
        {
            m_Root.Insert(item);
        }

        public IEnumerable<T> Query(RectangleF area)
        {
            return m_Root.Query(area);
        }

        public bool HasContent(RectangleF area)
        {
            bool result = m_Root.HasContent(area);
            return result;
        }

        #endregion

        public void ForEach(QuadTreeAction action)
        {
            m_Root.ForEach(action);
        }
    }
}