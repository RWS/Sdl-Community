using System;
using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.API;
using Sdl.Community.Productivity.Services;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Productivity.UI
{
    [RibbonGroup("Sdl.Community.Productivity", Name = "Community Productivity", ContextByType = typeof(EditorController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class ProductivityRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.Productivity", Icon = "icon",Name = "Productivity score", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    class ProductivityViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
            try
            {
                var twitterPersistenceService = new TwitterPersistenceService(logger);

                if (!ProductivityUiHelper.IsTwitterAccountConfigured(twitterPersistenceService, logger))
                {
                    MessageBox.Show(
                        PluginResources
                            .ProductivityViewPartAction_Execute_You_need_to_configure_the_twitter_account_to_see_your_score);
                    return;
                }
                using (var pForm = new ProductivityForm())
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

    [Action("Sdl.Community.ProductivityShare",  Icon = "twitter", Name = "Share", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    [Shortcut(Keys.Alt | Keys.S)]
    class ProductivityShareViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
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
