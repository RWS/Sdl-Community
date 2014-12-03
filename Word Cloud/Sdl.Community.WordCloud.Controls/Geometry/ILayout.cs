using System.Collections.Generic;
using System.Drawing;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Processing;

namespace Sdl.Community.WordCloud.Controls.Geometry
{
    public interface ILayout
    {
        void Arrange(IEnumerable<IWord> words, IGraphicEngine graphicEngine);
        IEnumerable<LayoutItem> GetWordsInArea(RectangleF area);
    }
}