using System;
using System.Collections.Generic;
using System.Drawing;
using Sdl.Community.ProjectTerms.Controls.Geometry.DataStructures;
using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Controls.Geometry
{
    public abstract class BaseLayout : ILayout
    {
        protected QuadTree<LayoutItem> QuadTree { get; set; }
        protected PointF Center { get; set; }
        protected RectangleF Surface { get; set; }

        protected BaseLayout(SizeF size)
        {
            Surface = new RectangleF(new PointF(0, 0), size);
            QuadTree = new QuadTree<LayoutItem>(Surface);
            Center = new PointF(Surface.X + size.Width / 2, Surface.Y + size.Height / 2);
        }

        public void Arrange(IEnumerable<ITerm> terms, IGraphicEngine graphicEngine)
        {
            if (terms == null)
            {
                throw new ArgumentNullException("terms");
            }

            foreach (ITerm term in terms)
            {
                SizeF size = graphicEngine.Measure(term.Text, term.Occurrences);
                RectangleF freeRectangle;
                if (!TryFindFreeRectangle(size, out freeRectangle))
                {
                    return;
                }
                LayoutItem item = new LayoutItem(freeRectangle, term);
                QuadTree.Insert(item);
            }
        }

        public abstract bool TryFindFreeRectangle(SizeF size, out RectangleF foundRectangle);

        public IEnumerable<LayoutItem> GetTermsInArea(RectangleF area)
        {
            return QuadTree.Query(area);
        }

        protected bool IsInsideSurface(RectangleF targetRectangle)
        {
            return IsInside(Surface, targetRectangle);
        }

        private static bool IsInside(RectangleF outer, RectangleF inner)
        {
            return
                inner.X >= outer.X &&
                inner.Y >= outer.Y &&
                inner.Bottom <= outer.Bottom &&
                inner.Right <= outer.Right;
        }
    }
}
