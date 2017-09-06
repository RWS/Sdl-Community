using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace Sdl.Community.ProjectTerms.Controls.Geometry
{
    public interface ILayout
    {
        void Arrange(IEnumerable<ITerm> terms, IGraphicEngine graphicEngine);
        IEnumerable<LayoutItem> GetTermsInArea(RectangleF area);
    }
}