using System;
using System.Windows.Forms;
using NLog;
using Sdl.Community.YourProductivity.API;
using Sdl.Community.YourProductivity.Services;
using Sdl.Community.YourProductivity.Persistence;
using Sdl.Community.YourProductivity.UI;
using Sdl.Community.YourProductivity.Persistance;

namespace Sdl.Community.YourProductivity.Util
{
    public class FormFactory
    {
        public static void CreateTweetForm(Logger logger)
        {
            try
            {
                var twitterPersistenceService = new TwitterPersistenceService(logger);
               
                if (!ProductivityUiHelper.IsTwitterAccountConfigured(twitterPersistenceService, logger))
                {
                    MessageBox.Show(
                        PluginResources
                            .ProductivityShareViewPartAction_Execute_In_order_to_share_the_result_you_need_to_configure_your_twitter_account);
                    return;
                }
                var trackInfoDb = new TrackInfoDb();
                var productivityService = new ProductivityService(logger, trackInfoDb);

                if (productivityService.TotalNumberOfCharacters < Constants.MinimumNumberOfCharacters)
                {
                    MessageBox.Show(
                        string.Format(
                            PluginResources
                                .ProductivityShareViewPartAction_Execute_In_order_to_share_your_score_you_need_to_translate_at_least__0__characters,
                            Constants.MinimumNumberOfCharacters.ToString("N0")));
                    return;
                }

                var leaderboardApi = new LeaderboardApi(twitterPersistenceService);
                var versioningService = new VersioningService(leaderboardApi);
                var tweetMessageService = new TweetMessageService(versioningService);
                var leaderBoardShareService = new LeaderboardShareService(leaderboardApi, twitterPersistenceService);
                var twitterShareService = new TwitterShareService(twitterPersistenceService, tweetMessageService);
                var shareService = new ShareService(productivityService, twitterShareService, leaderBoardShareService,
                    versioningService);

                if (!leaderboardApi.IsAlive())
                {
                    MessageBox.Show(PluginResources.TweetFactory_CreateTweet_SDL_Leaderboard_could_not_be_reached__Please_check_your_internet_connectivity_and_try_again);
                    return;
                }

                if (!twitterShareService.IsAlive())
                {
                    MessageBox.Show(PluginResources.TweetFactory_CreateTweet_Twitter_could_not_be_reached__Please_check_your_internet_connectivity_and_try_again_);
                    return;
                }

                if (!versioningService.IsPluginVersionCompatibleWithLeaderboardVersion())
                {
                    MessageBox.Show(
                        string.Format(
                            "In order to share you score you need to update the plugin to the version {0}. Please download the latest version from Open Exchange.",
                            shareService.LeaderboardVersion));
                    return;
                }

                using (var tf = new TweetForm(shareService,tweetMessageService, productivityService))
                {
                    tf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Unexpected exception when opening the share score");
                throw;
            }
        }

        public static void CreateProductivityForm(Logger logger)
        {
            try
            {
                var trackInfoDb = new TrackInfoDb();
                var productivityService = new ProductivityService(logger, trackInfoDb);
                var twitterPersistanceService = new TwitterPersistenceService(logger);
                var leaderboardApi = new LeaderboardApi(twitterPersistanceService);
                var versioningService = new VersioningService(leaderboardApi);
                var tweetMessageService = new TweetMessageService(versioningService);
                var twitterShareService = new TwitterShareService(twitterPersistanceService, tweetMessageService);
                using (var pForm = new ProductivityForm(productivityService, twitterShareService))
                {
                    pForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Unexpected exception when opening the productivity score");
                throw;
            }
        }
    }
}
