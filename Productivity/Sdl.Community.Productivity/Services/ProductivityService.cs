using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;

namespace Sdl.Community.Productivity.Services
{
    public class ProductivityService
    {
        private readonly TrackInfoPersistanceService _persistance;
        private List<TrackInfo> _trackingInfos;
        private readonly Logger _logger;
        
        public double ProductivityScore { get; set; }
        public long TotalNumberOfCharacters { get; set; }
        public long Bonus { get; set; }
        public long Score { get; set; }
        public string Language { get; set; }
        public DateTime LastTranslationDate { get; set; }
        public List<TrackInfoView> TrackInfoViews { get; set; }

        public ProductivityService(Logger logger)
        {
            _logger = logger;
            _persistance = new TrackInfoPersistanceService(_logger);
            TrackInfoViews = new List<TrackInfoView>();
            Initialize();
        }

        public ProductivityService():this(LogManager.GetLogger("log"))
        {
            
        }

        public void Refresh()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _trackingInfos = _persistance.Load();

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
            return _trackingInfos.Max(x => x.SegmentTrackInfos.Max(y => y.UtcDateTime));
        }

        private double CalculateProductivityScore(IEnumerable<TrackInfo> trackingInfos)
        {
            return Math.Round(trackingInfos.DefaultIfEmpty(new TrackInfo()).Average(x => Math.Round(x.ProductivityScore, 0)), 0);
        }

        private List<TrackInfoView> GetTrackInfoView(List<TrackInfo> trackingInfos)
        {
            return TrackInfoView.CreateFromTrackInfos(trackingInfos);
        }

    }
}
