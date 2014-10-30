using Sdl.Community.ControlledMTProviders.Provider;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.ControlledMTProviders.Integration
{
    [RibbonGroup("SDL.Community.ManageMTRibbonGroup", Name = "Controled MT Providers")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorAdvancedRibbonTabLocation))]
    class ControlledMTProvidersRibbon : AbstractRibbonGroup
    {
    }


    [Action("SDL.Community.ManageMT", Name = "Enable/Disable Machine Translation", Icon = "icon", Description = "Enable or disable configured machine translation providers")]
    [ActionLayout(typeof(ControlledMTProvidersRibbon), 60, DisplayType.Large)]
    [Shortcut(Keys.Alt | Keys.Shift | Keys.M)]
    class ControlledMTViewPartAction : AbstractAction
    {
        public ControlledMTViewPartAction()
        {
        }

        public override void Initialize()
        {
            string text = CascadeLanguageDirection.DisableMT ? "Enable MT" : "Disable MT";
            this.Text = text;
        }

        protected override void Execute()
        {
            CascadeLanguageDirection.DisableMT = !CascadeLanguageDirection.DisableMT;
            string text = CascadeLanguageDirection.DisableMT ? "Enable MT" : "Disable MT";
            this.Text = text;
        }
    }
}

