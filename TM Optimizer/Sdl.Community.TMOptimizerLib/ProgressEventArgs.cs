using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
