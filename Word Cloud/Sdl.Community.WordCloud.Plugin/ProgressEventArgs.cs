using System.ComponentModel;

namespace Sdl.Community.WordCloud.Plugin
{
    public class ProgressEventArgs : CancelEventArgs
    {
        public ProgressEventArgs()
        {
        }

        public int PercentComplete { get; internal set; }
        public string StatusMessage { get; internal set; }
    }
}
