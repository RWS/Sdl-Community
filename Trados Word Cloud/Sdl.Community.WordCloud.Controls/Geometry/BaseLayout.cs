using System;
using System.Collections.Generic;
using System.Drawing;
using Sdl.Community.WordCloud.Controls.Geometry.DataStructures;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Processing;

namespace Sdl.Community.WordCloud.Controls.Geometry
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

        public void Arrange(IEnumerable<IWord> words, IGraphicEngine graphicEngine)
        {
            if (words == null)
            {
                throw new ArgumentNullException("words");
            }

            foreach (IWord word in words)
            {
                SizeF size = graphicEngine.Measure(word.Text, word.Occurrences);
                RectangleF freeRectangle;
                if (!TryFindFreeRectangle(size, out freeRectangle))
                {
                    return;
                }
                LayoutItem item = new LayoutItem(freeRectangle, word);
                QuadTree.Insert(item);
            }
        }

        public abstract bool TryFindFreeRectangle(SizeF size, out RectangleF foundRectangle);

        public IEnumerable<LayoutItem> GetWordsInArea(RectangleF area)
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
