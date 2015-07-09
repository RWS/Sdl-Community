using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Sdl.Community.Productivity.Model
{
    public class TrackInfo 
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string Language { get; set; }

        public List<SegmentTrackInfo> SegmentTrackInfos { get; set; }

        public string FileType { get; set; }

        public bool HasBonus
        {
            get { return SegmentTrackInfos.All(x => x.Translated); }
        }

        public double ProductivityScore
        {
            get
            {
                return
                    Math.Round(
                        SegmentTrackInfos.Where(x => x.Translated && x.UtcDateTime > DateTime.UtcNow.AddDays(-30))
                            .DefaultIfEmpty(new SegmentTrackInfo() {ProductivityScore = 100})
                            .Average(x => Math.Round(x.ProductivityScore, 0)), 0);
            }
        }


    }
}
