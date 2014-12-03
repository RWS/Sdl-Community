using System;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Processing
{
    public interface IWord : IComparable<IWord>
    {
        string Text { get; }
        int Occurrences { get; }
        string GetCaption();
    }
}