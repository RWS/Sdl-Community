﻿using System.Windows.Forms;
using NLog;
using Sdl.Community.YourProductivity.Persistance;
using Sdl.Community.YourProductivity.Util;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.YourProductivity.UI
{
	[RibbonGroup("Sdl.Community.Productivity", Name = "#YourProductivity")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class ProductivityRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.Productivity", Icon = "score", Name = "Productivity score", Description = "#YourProductivity")]
    [ActionLayout(typeof(ProductivityRibbon), 10, DisplayType.Large)]
    class ProductivityViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            if (!ProductivityUiHelper.IsGreaterThanCu10())
            {
                MessageBox.Show(
                    PluginResources.ProductivityViewPartAction_Execute_This_plugin_is_compatible_with_Trados_Studio_2014_CU10_or_later__In_order_to_enjoy_this_plugin_please_upgrade_to_a_newer_version_);
                return;
            }
            Application.EnableVisualStyles();
            var logger = LogManager.GetLogger("log");
            RavenContext.Current.CurrentSession.SaveChanges();
            FormFactory.CreateProductivityForm(logger);
        }
    }
}
