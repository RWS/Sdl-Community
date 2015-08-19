using System;

namespace Sdl.Community.YourProductivity.Services
{
    public class TweetMessageService
    {
        private const string TweetMessage1 = "I was this productive today using #{0} – {1} #YourProductivity {2} via @sdltrados";
        private const string TweetMessage2 = "My productivity today using SDL Trados #{0} was {1} #YourProductivity {2} via @sdltrados";
        private const string TweetMessage3 = "Check out my productivity score on the SDL Trados #{0} leaderboard #YourProductivity {1} via @sdltrados";
        private readonly string _studioVersion;
        private readonly string _leaderboardLink;

        public TweetMessageService(VersioningService versioningService)
        {
            _studioVersion = versioningService.GetStudioVersionFriendly();
            _leaderboardLink = PluginResources.Leaderboard_Link;
        }

        

        public string GetTwitterMessage(double score)
        {
            string tweetMessage;
            var number = new Random().Next(1, 4);
            switch (number)
            {
                case 1:
                    tweetMessage = string.Format(TweetMessage1, _studioVersion, score, _leaderboardLink);
                    break;
                case 2:
                    tweetMessage = string.Format(TweetMessage2, _studioVersion, score, _leaderboardLink);
                    break;
                case 3:
                    tweetMessage = string.Format(TweetMessage3, _studioVersion, _leaderboardLink);
                    break;
                default:
                    tweetMessage = string.Format(TweetMessage1, _studioVersion, score, _leaderboardLink);
                    break;
            }

            return tweetMessage;
        }
    }
}
