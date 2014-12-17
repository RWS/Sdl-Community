using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.BringBackTheButton
{
    [RibbonGroup("Sdl.Community.BringBackTheButton", Name = "Don't push it!")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.ViewRibbonTabLocation))]
    class BringBackTheButtonRibbon: AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.BringBackTheButton", Name = "", Icon = "icon", Description = "Don't push it!")]
    [ActionLayout(typeof(BringBackTheButtonRibbon), 20, DisplayType.Large)]
    class BringBackTheButtonViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            var bbtb = new BringBackTheButton();
            bbtb.ShowDialog();
            MessageBox.Show(@"Brought to you by SDL in sympathy for any memoQ user who upgraded to 2014 R2", "",
                MessageBoxButtons.OK);
        }
    }
}
