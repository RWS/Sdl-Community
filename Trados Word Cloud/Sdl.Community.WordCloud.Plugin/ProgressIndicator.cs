using Sdl.Community.WordCloud.Controls.TextAnalyses.Extractors;

namespace Sdl.Community.WordCloud.Plugin
{
    class ProgressIndicator : IProgressIndicator
    {
        public int Maximum
        {
            get;
            set;
        }

        public void Increment(int value)
        {
            
        }
    }
}
