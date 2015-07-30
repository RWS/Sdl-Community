using System;
using NLog;
using Sdl.Community.Productivity.Model;

namespace Sdl.Community.Productivity.Services
{
    public class ShareService
    {
        private readonly ProductivityService _productivityService;
        private readonly VersioningService _versioningService;
        private readonly TwitterShareService _twitterShareService;
        private readonly LeaderboardShareService _leaderboardShareService;

        public Version LeaderboardVersion;
        public Version PluginVersion;

        public ShareService(ProductivityService productivityService,
            TwitterShareService twitterShareService,
            LeaderboardShareService leaderboardShareService,
            VersioningService versioningService)
        {
            _productivityService = productivityService;
            _twitterShareService = twitterShareService;
            _leaderboardShareService = leaderboardShareService;
            _twitterShareService = twitterShareService;
            _versioningService = versioningService;
        }
      
        public void Share()
        {
            if (CanShare()) return;

            var leaderboardInfo = new LeaderboardInfo
            {
                TwitterHandle = _twitterShareService.GetUserProfile().ScreenName,
                Score = _productivityService.Score,
                Language = _productivityService.Language,
                LastTranslationAt = _productivityService.LastTranslationDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                AppVersion = _versioningService.PluginVersion
            };

            var isSharedOnTheLeaderboard = _leaderboardShareService.ShareOnLeaderboard(leaderboardInfo);

            if (isSharedOnTheLeaderboard)
            {

            #if !DEBUG
                {
                    _twitterShareService.ShareOnTwitter(leaderboardInfo);
                }
            #endif

            }
        }

        private bool CanShare()
        {
            if (!_versioningService.IsPluginVersionCompatibleWithLeaderboardVersion()) return true;
            if (!_leaderboardShareService.IsAlive()) return true;
            if (!_twitterShareService.IsAlive()) return true;
            return false;
        }

    }
}
