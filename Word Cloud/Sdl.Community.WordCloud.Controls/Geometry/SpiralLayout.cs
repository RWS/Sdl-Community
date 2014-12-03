using System;
using System.Drawing;

namespace Sdl.Community.WordCloud.Controls.Geometry
{
    public class SpiralLayout : BaseLayout
    {
        public SpiralLayout(SizeF size)
            : base(size)
        {
        }

        public override bool TryFindFreeRectangle(SizeF size, out RectangleF foundRectangle)
        {
            foundRectangle = RectangleF.Empty;
            double alpha = GetPseudoRandomStartAngle(size);
            const double stepAlpha = Math.PI / 60;

            const double pointsOnSpital = 500;


            Math.Min(Center.Y, Center.X);
            for (int pointIndex = 0; pointIndex < pointsOnSpital; pointIndex++)
            {
                double dX = pointIndex / pointsOnSpital * Math.Sin(alpha) * Center.X;
                double dY = pointIndex / pointsOnSpital * Math.Cos(alpha) * Center.Y;
                foundRectangle = new RectangleF((float)(Center.X + dX) - size.Width / 2, (float)(Center.Y + dY) - size.Height / 2, size.Width, size.Height);

                alpha += stepAlpha;
                if (!IsInsideSurface(foundRectangle))
                {
                    return false;
                }

                if (!QuadTree.HasContent(foundRectangle))
                {
                    return true;
                }
            }

            return false;
        }

        private static float GetPseudoRandomStartAngle(SizeF size)
        {
            return size.Height*size.Width;
        }
    }
}