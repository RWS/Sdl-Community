using Sdl.Community.BetaAPIs.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;

namespace Sdl.Community.BetaAPIs
{
    [RibbonGroup("Sdl.Community.StudioBetaAPIs",Name ="Studio Beta APIs")]
    [RibbonGroupLayout(LocationByType =typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class BetaAPIsRibbon: AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.StudioBetaAPIs",Icon ="", Name ="Available Beta APIs",Description ="Enable or disable Studio Beta APIs")]
    [ActionLayout(typeof(BetaAPIsRibbon),20,DisplayType.Normal)]
    public class BetaAPIsViewPartAction:AbstractAction
    {
        protected override void Execute()
        {
            StudioBetaAPIsMainWindow window = new StudioBetaAPIsMainWindow();
            
            window.ShowDialog();

            //Application app = new Application();
            //app.Run(window);
        }
    }
}
