using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.YourProductivity.Model
{
    public class TrackInfoView
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string Language { get; set; }

        public string FileType { get; set; }

        public double Efficiency { get; set; }

        public long KeystrokesSaved { get; set; }

        public long InsertedCharacters { get; set; }


       
    }
}
