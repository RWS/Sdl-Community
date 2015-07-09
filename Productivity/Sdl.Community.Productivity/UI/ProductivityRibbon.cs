using System;
using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.API;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services;
using Sdl.Community.Productivity.Services.Persistence;
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
            TweetFactory.CreateTweet(logger);
        }


    }
}
