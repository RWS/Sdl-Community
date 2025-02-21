using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ReindexTms
{
    public class ProgressEventArgs
    {
        public enum ProcessorType
        {
            None = 0,
            TmExporter,
            TmImporter,
            ModelBuilder,
            FragmentAligner
        }
        public int TotalUnits { get; set; }

        public int CurrentProgress { get; set; }

        public ProcessorType Type { get; set; }
        public string Description { get; set; }

        public ProgressEventArgs()
        {
            Type = ProcessorType.None;
            TotalUnits = 0;
            CurrentProgress = 0;
            Description = string.Empty;

        }
    }
}
