using System.ComponentModel;

namespace Sdl.Community.ProjectTerms.Plugin.Utils
{
    public class ProgressEventArgs : CancelEventArgs
    {
        public ProgressEventArgs() { }

        public int Percent { get; internal set; }
        public string StatusMessage { get; internal set; }
    }
}
