using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.TMOptimizerLib
{
    public class ProcessorBase
    {
        public event EventHandler<ProgressEventArgs> Progress;

        protected bool ReportProgress(int progress)
        {
            if (Progress != null)
            {
                ProgressEventArgs e = new ProgressEventArgs();
                e.Progress = progress;
                Progress(this, e);
                return !e.Cancel;
            }

            return true;
        }
    }
}
