using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.FragmentAlignmentAutomation
{
    public class ProcessingModel
    {        
        public enum ProgressType
        {
            IsWaiting = 0,
            IsProcessing,
            IsComplete
        }

        public FileInfo File { get; set; }

        public ProgressType Progress { get; set; }

        public DateTime? StartProcessing { get; set; }
        public DateTime? EndProcessing { get; set; }

        public ProgressEventArgs ProgressArgs { get; set; }
        public bool HasError { get; set; }

        public List<string> ProgressMessages { get; set; }

        public ProcessingModel()
        {
            File = null;
            Progress = ProgressType.IsWaiting;
            StartProcessing = null;
            EndProcessing = null;
            ProgressArgs = null;
            HasError = false;
            ProgressMessages = new List<string>();
        }
    }
}
