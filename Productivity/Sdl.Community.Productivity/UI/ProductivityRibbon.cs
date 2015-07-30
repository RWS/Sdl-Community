using System.Diagnostics;
using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.Util;
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

    [Action("Sdl.Community.Productivity", Icon = "score",Name = "Productivity score", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    class ProductivityViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            if (!ProductivityUiHelper.IsGreaterThanCu10())
            {
                MessageBox.Show(
                    PluginResources.ProductivityViewPartAction_Execute_This_plugin_is_compatible_with_SDL_Studio_2014_CU10_or_later__In_order_to_enjoy_this_plugin_please_upgrade_to_a_newer_version_);
                return;
            }
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
            FormFactory.CreateProductivityForm(logger);
        }
    }

    [Action("Sdl.Community.ProductivityShare",  Icon = "twitter", Name = "Share", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    [Shortcut(Keys.Alt | Keys.S)]
    class ProductivityShareViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            if (!ProductivityUiHelper.IsGreaterThanCu10())
            {
                MessageBox.Show(
                    PluginResources.ProductivityViewPartAction_Execute_This_plugin_is_compatible_with_SDL_Studio_2014_CU10_or_later__In_order_to_enjoy_this_plugin_please_upgrade_to_a_newer_version_);
                return;
            }
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
            FormFactory.CreateTweetForm(logger);
        }


    }

    [Action("Sdl.Community.ProductivityLeaderboard", Icon = "leaderboard", Name = "Leaderboard", Description = "Community Productivity")]
    [ActionLayout(typeof(ProductivityRibbon), 20, DisplayType.Normal)]
    class ProductivityLeaderboardViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            if (!ProductivityUiHelper.IsGreaterThanCu10())
            {
                MessageBox.Show(
                    PluginResources.ProductivityViewPartAction_Execute_This_plugin_is_compatible_with_SDL_Studio_2014_CU10_or_later__In_order_to_enjoy_this_plugin_please_upgrade_to_a_newer_version_);
                return;
            }
            Application.EnableVisualStyles();
            var sInfo = new ProcessStartInfo(PluginResources.Leaderboard_Link);
            Process.Start(sInfo);
        }


    }
}
