using System.Drawing;

namespace Sdl.Community.WordCloud.Controls.Geometry
{
    public class TypewriterLayout : BaseLayout
    {
        public TypewriterLayout(SizeF size) : base(size)
        {
            m_Carret = new PointF(size.Width, 0);
        }

        private PointF m_Carret;
        private float m_LineHeight;
 
        public override bool TryFindFreeRectangle(SizeF size, out RectangleF foundRectangle)
        {
            foundRectangle = new RectangleF(m_Carret, size);
            if (HorizontalOverflow(foundRectangle))
            {
                foundRectangle = LineFeed(foundRectangle);
                if (!IsInsideSurface(foundRectangle))
                {
                    return false;
                }
            }
            m_Carret = new PointF(foundRectangle.Right, foundRectangle.Y);
            
            return true;
        }

        private RectangleF LineFeed(RectangleF rectangle)
        {
            RectangleF result = new RectangleF(new PointF(0, m_Carret.Y + m_LineHeight), rectangle.Size);
            m_LineHeight = rectangle.Height;
            return result;
        }

        private bool HorizontalOverflow(RectangleF rectangle)
        {
            return rectangle.Right > Surface.Right;
        }
    }
}
