using System;

namespace Sdl.Community.FragmentAlignmentAutomation
{
    public class ProgressEventArgs : EventArgs
    {
        public enum ProcessorType
        {
            None=0,
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
