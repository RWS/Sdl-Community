using System.ComponentModel;

namespace Sdl.Community.TMOptimizerLib
{
	public class ProgressEventArgs : CancelEventArgs
    {
        public int Progress
        {
            get;
            set;
        }
    }
}