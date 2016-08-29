using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Sdl.Community.YourProductivity.Model;
using Sdl.Community.YourProductivity.Persistence;
using Sdl.Community.YourProductivity.Persistance.Model;

namespace Sdl.Community.YourProductivity.Services
{
    public class ProductivityService
    {
        private readonly TrackInfoDb _db;
        private IList<TrackInfo> _trackingInfos;
        private readonly Logger _logger;
        
        public double ProductivityScore { get; set; }
        public long TotalNumberOfCharacters { get; set; }
        public long Bonus { get; set; }
        public long Score { get; set; }
        public string Language { get; set; }
        public DateTime LastTranslationDate { get; set; }
        public List<TrackInfoView> TrackInfoViews { get; set; }

        public ProductivityService(Logger logger, TrackInfoDb db)
        {
            _logger = logger;
            _db = db;
            TrackInfoViews = new List<TrackInfoView>();
            Initialize();
        }


        public void Refresh()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _trackingInfos = _db.GetTrackInfosAsync().Result;
                _trackingInfos = _trackingInfos.Where(x => x.SegmentTrackInfos.Count > 0).ToList();
                if (_trackingInfos.Count == 0)
                {
                    ProductivityScore = 0;
                    TotalNumberOfCharacters = 0;
                    Bonus = 0;
                    Score = 0;
                    Language = string.Empty;
                    return;
                }

                ProductivityScore = CalculateProductivityScore(_trackingInfos);
                TotalNumberOfCharacters = CalculateTotalNumberOfCharacters(_trackingInfos);
                Bonus = CalculateBonus(_trackingInfos);
                Score = CalculateScore();
                Language = GetMostUsedTargetLanguage(_trackingInfos);
                LastTranslationDate = GetLastTranslationDate();
                TrackInfoViews = GetTrackInfoView(_trackingInfos);
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, "Error when initialize productivity service");
            }
        }

        private string GetMostUsedTargetLanguage(IEnumerable<TrackInfo> trackingInfos)
        {
            return trackingInfos.Select(x => x.Language).Mode();
        }

        private long CalculateBonus(IEnumerable<TrackInfo> trackingInfos)
        {
            return trackingInfos.Where(x => x.HasBonus).Sum(x => 200);
        }

        private long CalculateTotalNumberOfCharacters(IEnumerable<TrackInfo> trackingInfos)
        {
            return trackingInfos.Sum(x => x.SegmentTrackInfos.Sum(y => y.Text.Length));
        }

        private long CalculateScore()
        {
            return Convert.ToInt64(TotalNumberOfCharacters*ProductivityScore) + Bonus;
        }

        private DateTime GetLastTranslationDate()
        {
            return _trackingInfos.Max(x => x.SegmentTrackInfos.Select(y => y.UtcDateTime).DefaultIfEmpty().Max());
        }

        private double CalculateProductivityScore(IEnumerable<TrackInfo> trackingInfos)
        {
            return Math.Round(trackingInfos.DefaultIfEmpty(new TrackInfo()).Average(x => Math.Round(x.ProductivityScore, 0)), 0);
        }

        private List<TrackInfoView> GetTrackInfoView(IList<TrackInfo> trackingInfos)
        {
            return CreateFromTrackInfos(trackingInfos);
        }
        public List<TrackInfoView> CreateFromTrackInfos(IList<TrackInfo> trackInfos)
        {
            var trackInfoViews = new List<TrackInfoView>();


            trackInfos.ToList().ForEach(trackInfo =>
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
                    KeystrokesSaved = trackInfo.SegmentTrackInfos.Sum(x => x.InsertedCharacters - x.NumberOfKeys < 0 ? 0 : x.InsertedCharacters - x.NumberOfKeys),
                    InsertedCharacters = trackInfo.SegmentTrackInfos.Sum(x => x.InsertedCharacters),
                };
                trackInfoViews.Add(trackInfoView);
            });

            return trackInfoViews;
        }
    }
}
