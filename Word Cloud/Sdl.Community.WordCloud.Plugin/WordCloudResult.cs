using System;
using System.Collections.Generic;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Processing;

namespace Sdl.Community.WordCloud.Plugin
{
    public class WordCloudResult
    {
        public IEnumerable<IWord> WeightedWords
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }
    }
}
