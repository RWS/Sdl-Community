using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Drawing;

namespace Sdl.Community.ProjectTerms.Controls.Geometry
{
    public class LayoutItem
    {
        public LayoutItem(RectangleF rectangle, ITerm term)
        {
            this.Rectangle = rectangle;
            Term = term;
        }

        public RectangleF Rectangle { get; private set; }
        public ITerm Term { get; private set; }

        public LayoutItem Clone()
        {
            return new LayoutItem(this.Rectangle, this.Term);
        }
    }
}
