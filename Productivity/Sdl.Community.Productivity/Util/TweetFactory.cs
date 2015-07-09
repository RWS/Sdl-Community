using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.API;
using Sdl.Community.Productivity.Services;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Community.Productivity.UI;

namespace Sdl.Community.Productivity.Util
{
    public class TweetFactory
    {
        public static void CreateTweet(Logger logger)
        {
            try
            {
                var twitterPersistenceService = new TwitterPersistenceService(logger);
                var leaderboardApi = new LeaderboardApi(twitterPersistenceService);
                var tweetMessageService = new TweetMessageService();
                if (!ProductivityUiHelper.IsTwitterAccountConfigured(twitterPersistenceService, logger))
                {
                    MessageBox.Show(
                        PluginResources
                            .ProductivityShareViewPartAction_Execute_In_order_to_share_the_result_you_need_to_configure_your_twitter_account);
                    return;
                }
                var productivityService = new ProductivityService(logger);

                if (productivityService.TotalNumberOfCharacters < Constants.MinimumNumberOfCharacters)
                {
                    MessageBox.Show(
                        string.Format(
                            PluginResources
                                .ProductivityShareViewPartAction_Execute_In_order_to_share_your_score_you_need_to_translate_at_least__0__characters,
                            Constants.MinimumNumberOfCharacters.ToString("N0")));
                    return;
                }

                var shareService = new ShareService(productivityService, twitterPersistenceService, tweetMessageService,
                    leaderboardApi, logger);

                if (!shareService.IsLeaderboardAvailable())
                {
                    MessageBox.Show(PluginResources.TweetFactory_CreateTweet_SDL_Leaderboard_could_not_be_reached__Please_check_your_internet_connectivity_and_try_again);
                    return;
                }

                if (!shareService.IsTwitterAvailable())
                {
                    MessageBox.Show(PluginResources.TweetFactory_CreateTweet_Twitter_could_not_be_reached__Please_check_your_internet_connectivity_and_try_again_);
                    return;
                }

                if (!shareService.CanShareOnLeaderBoadrd())
                {
                    MessageBox.Show(
                        string.Format(
                            "In order to share you score you need to update the plugin to the version {0}. Please download the latest version from Open Exchange.",
                            shareService.LeaderboardVersion));
                    return;
                }

                using (var tf = new TweetForm(shareService))
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
    }
}
