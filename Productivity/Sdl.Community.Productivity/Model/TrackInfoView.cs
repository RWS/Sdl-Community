using System;
using System.Collections.Generic;

namespace Sdl.Community.Productivity.Model
{
    public class TrackInfoView
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public string Language { get; set; }

        public string FileType { get; set; }

        public double FileProductivityScore { get; set; }

        public string SegmentId { get; set; }

        public int NumberOfKeys { get; set; }

        public string Text { get; set; }

        public bool Translated { get; set; }

        public double SegmentProductivityScore { get; set; }

        public DateTime UtcDateTime { get; set; }

        public static List<TrackInfoView> CreateFromTrackInfos(List<TrackInfo> trackInfos)
        {
            var trackInfoViews = new List<TrackInfoView>();
                 
            trackInfos.ForEach(trackInfo => trackInfo.SegmentTrackInfos.ForEach(segmentTrackInfo =>
            {
                var trackInfoView = new TrackInfoView()
                {
                    FileId = trackInfo.FileId,
                    FileName = trackInfo.FileName,
                    FileType = trackInfo.FileType,
                    Language = trackInfo.Language,
                    FileProductivityScore = trackInfo.ProductivityScore,
                    SegmentId = segmentTrackInfo.SegmentId,
                    NumberOfKeys = segmentTrackInfo.NumberOfKeys,
                    Text = segmentTrackInfo.Text,
                    Translated =segmentTrackInfo.Translated,
                    SegmentProductivityScore = segmentTrackInfo.ProductivityScore,
                    UtcDateTime = segmentTrackInfo.UtcDateTime
                };
                trackInfoViews.Add(trackInfoView);
            }));

            return trackInfoViews;
        }
    }
}
