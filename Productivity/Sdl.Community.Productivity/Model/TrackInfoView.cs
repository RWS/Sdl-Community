using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.Productivity.Model
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

        public Int64 KeystrokesSaved { get; set; }

        public Int64 InsertedCharacters { get; set; }


        public static List<TrackInfoView> CreateFromTrackInfos(List<TrackInfo> trackInfos)
        {
            var trackInfoViews = new List<TrackInfoView>();


            trackInfos.ForEach(trackInfo =>
            {
                var trackInfoView = new TrackInfoView()
                {
                    FileId = trackInfo.FileId,
                    FileName = trackInfo.FileName,
                    ProjectName = trackInfo.ProjectName,
                    ProjectId = trackInfo.ProjectId,
                    FileType = trackInfo.FileType,
                    Language = trackInfo.Language,
                    Efficiency = trackInfo.ProductivityScore,
                    KeystrokesSaved = trackInfo.SegmentTrackInfos.Sum(x => x.NumberOfKeys),
                    InsertedCharacters = trackInfo.SegmentTrackInfos.Sum(x => x.InsertedCharacters),
                };
                trackInfoViews.Add(trackInfoView);
            });

            return trackInfoViews;
        }
    }
}
