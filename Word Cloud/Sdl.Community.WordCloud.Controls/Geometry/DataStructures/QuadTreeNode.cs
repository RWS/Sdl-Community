#region

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

#endregion

namespace Sdl.Community.WordCloud.Controls.Geometry.DataStructures
{
    public class QuadTreeNode<T> where T : LayoutItem
    {
        private readonly Stack<T> m_Contents = new Stack<T>();
        private RectangleF m_Bounds;

        private QuadTreeNode<T>[] m_Nodes = new QuadTreeNode<T>[0];

        public QuadTreeNode(RectangleF bounds)
        {
            m_Bounds = bounds;
        }

        public bool IsEmpty
        {
            get { return m_Bounds.IsEmpty || m_Nodes.Length == 0; }
        }

        public RectangleF Bounds
        {
            get { return m_Bounds; }
        }

        public int Count
        {
            get
            {
                int count = 0;

                foreach (QuadTreeNode<T> node in m_Nodes)
                    count += node.Count;

                count += this.Contents.Count;

                return count;
            }
        }

        public IEnumerable<T> SubTreeContents
        {
            get
            {
                IEnumerable<T> results = new T[0];

                foreach (QuadTreeNode<T> node in m_Nodes)
                    results = results.Concat(node.SubTreeContents);

                results = results.Concat(this.Contents);
                return results;
            }
        }

        public Stack<T> Contents
        {
            get { return m_Contents; }
        }


        public bool HasContent(RectangleF queryArea)
        {
            IEnumerable<T> queryResult = Query(queryArea);
            return IsEmptyEnumerable(queryResult);
        }

        private static bool IsEmptyEnumerable(IEnumerable<T> queryResult)
        {
            using (var enumerator = queryResult.GetEnumerator())
            {
                return enumerator.MoveNext();
            }
        }

        /// <summary>
        ///   Query the QuadTree for items that are in the given area
        /// </summary>
        /// <param name = "queryArea">
        ///   <returns></returns></param>
        public IEnumerable<T> Query(RectangleF queryArea)
        {
            // this quad contains items that are not entirely contained by
            // it's four sub-quads. Iterate through the items in this quad 
            // to see if they intersect.
            foreach (T item in this.Contents)
            {
                if (queryArea.IntersectsWith(item.Rectangle))
                    yield return item;
            }

            foreach (QuadTreeNode<T> node in m_Nodes)
            {
                if (node.IsEmpty)
                    continue;

                // Case 1: search area completely contained by sub-quad
                // if a node completely contains the query area, go down that branch
                // and skip the remaining nodes (break this loop)
                if (node.Bounds.Contains(queryArea))
                {
                    IEnumerable<T> subResults = node.Query(queryArea);
                    foreach (var subResult in subResults)
                    {
                        yield return subResult;
                    }
                    break;
                }

                // Case 2: Sub-quad completely contained by search area 
                // if the query area completely contains a sub-quad,
                // just add all the contents of that quad and it's children 
                // to the result set. You need to continue the loop to test 
                // the other quads
                if (queryArea.Contains(node.Bounds))
                {
                    IEnumerable<T> subResults = node.SubTreeContents;
                    foreach (var subResult in subResults)
                    {
                        yield return subResult;
                    }
                    continue;
                }

                // Case 3: search area intersects with sub-quad
                // traverse into this quad, continue the loop to search other
                // quads
                if (node.Bounds.IntersectsWith(queryArea))
                {
                    IEnumerable<T> subResults = node.Query(queryArea);
                    foreach (var subResult in subResults)
                    {
                        yield return subResult;
                    }
                }
            }
        }


        public void Insert(T item)
        {
            // if the item is not contained in this quad, there's a problem
            if (!m_Bounds.Contains(item.Rectangle))
            {
                Trace.TraceWarning("feature is out of the bounds of this quadtree node");
                return;
            }

            // if the subnodes are null create them. may not be sucessfull: see below
            // we may be at the smallest allowed size in which case the subnodes will not be created
            if (m_Nodes.Length == 0)
                CreateSubNodes();

            // for each subnode:
            // if the node contains the item, add the item to that node and return
            // this recurses into the node that is just large enough to fit this item
            foreach (QuadTreeNode<T> node in m_Nodes)
            {
                if (node.Bounds.Contains(item.Rectangle))
                {
                    node.Insert(item);
                    return;
                }
            }

            // if we make it to here, either
            // 1) none of the subnodes completely contained the item. or
            // 2) we're at the smallest subnode size allowed 
            // add the item to this node's contents.
            this.Contents.Push(item);
        }

        public void ForEach(QuadTree<T>.QuadTreeAction action)
        {
            action(this);

            // draw the child quads
            foreach (QuadTreeNode<T> node in this.m_Nodes)
                node.ForEach(action);
        }

        private void CreateSubNodes()
        {
            // the smallest subnode has an area 
            if ((m_Bounds.Height*m_Bounds.Width) <= 10)
                return;

            float halfWidth = (m_Bounds.Width/2f);
            float halfHeight = (m_Bounds.Height/2f);

            m_Nodes = new QuadTreeNode<T>[4];
            m_Nodes[0] = (new QuadTreeNode<T>(new RectangleF(m_Bounds.Location, new SizeF(halfWidth, halfHeight))));
            m_Nodes[1] = (new QuadTreeNode<T>(new RectangleF(new PointF(m_Bounds.Left, m_Bounds.Top + halfHeight), new SizeF(halfWidth, halfHeight))));
            m_Nodes[2] = (new QuadTreeNode<T>(new RectangleF(new PointF(m_Bounds.Left + halfWidth, m_Bounds.Top), new SizeF(halfWidth, halfHeight))));
            m_Nodes[3] =
                (new QuadTreeNode<T>(new RectangleF(new PointF(m_Bounds.Left + halfWidth, m_Bounds.Top + halfHeight), new SizeF(halfWidth, halfHeight))));
        }
    }
}